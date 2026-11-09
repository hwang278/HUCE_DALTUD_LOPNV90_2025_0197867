using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RebarStair.RebarStair.Model;
using RebarStair.RebarStair.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RebarStair.RebarStair.Services
{
    /// <summary>
    /// Dựng toàn bộ thép cho một vế thang: quản lý transaction, tạo Rebar và ghi lại
    /// thép nào thuộc số hiệu nào. Không phụ thuộc WPF, không giữ state giao diện.
    /// </summary>
    public partial class RebarPlacementService
    {
        private readonly Document _document;
        private readonly Dictionary<int, List<Autodesk.Revit.DB.Structure.Rebar>> _rebarSetsByOption = [];

        public RebarPlacementService(
            Document document,
            StairModel stairModel,
            RebarParameters parameters,
            IReadOnlyList<RebarOption> options)
        {
            _document = document ?? throw new ArgumentNullException(nameof(document));
            StairModel = stairModel ?? throw new ArgumentNullException(nameof(stairModel));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            RebarOptions = options ?? throw new ArgumentNullException(nameof(options));
        }

        public StairModel StairModel { get; }

        public RebarParameters Parameters { get; }

        public IReadOnlyList<RebarOption> RebarOptions { get; }

        // Tên tham số dùng trong các công thức dựng hình (giữ đúng như bản cũ).
        public double Cover => Parameters.Cover;
        public double KhoangCach => Parameters.Spacing;
        public double R1 => Parameters.StairWidth;
        public double D => Parameters.LandingWidth;
        public double F1 => Parameters.F1;
        public double F2 => Parameters.F2;
        public double L1CN => Parameters.L1;
        public double L2CN => Parameters.L2;
        public double BDam => Parameters.BeamWidth;
        public double HDam => Parameters.BeamHeight;
        public double DoDaiNeo => Parameters.AnchorageLength;
        public double DoVuonCuaThep => Parameters.ExtensionLength;

        /// <summary>
        /// Xoá bản xem trước cũ và dựng lại thép cho loại thang đang chọn.
        /// Trả về id của toàn bộ thép thuộc bản xem trước.
        /// </summary>
        public IReadOnlyList<ElementId> Build(string stairTypeName, IReadOnlyList<ElementId> previousPreviewIds)
        {
            var permanentRebarIds = new FilteredElementCollector(_document)
                .OfClass(typeof(Autodesk.Revit.DB.Structure.Rebar))
                .ToElementIds()
                .Where(id => !previousPreviewIds.Contains(id))
                .ToHashSet();

            using (var transaction = new Transaction(_document, "Update rebar preview"))
            {
                transaction.Start();
                foreach (var id in previousPreviewIds.Where(id => _document.GetElement(id) != null))
                {
                    _document.Delete(id);
                }

                _rebarSetsByOption.Clear();

                switch (stairTypeName)
                {
                    case "Type1":
                        CreateType1Rebar();
                        CreateType1Stirrups();
                        break;
                    case "Type2":
                        CreateType2Rebar();
                        CreateType2Stirrups();
                        break;
                    case "Type3":
                        CreateType3Rebar();
                        CreateType3Stirrups();
                        break;
                    case "Chiếu Nghỉ":
                    case "Landing":
                        CreateLandingRebar();
                        break;
                }

                transaction.Commit();
            }

            return new FilteredElementCollector(_document)
                .OfClass(typeof(Autodesk.Revit.DB.Structure.Rebar))
                .ToElementIds()
                .Where(id => !permanentRebarIds.Contains(id))
                .ToList();
        }

        /// <summary>Các thanh thép thuộc một số hiệu (1..5) trong bản xem trước hiện tại.</summary>
        public IReadOnlyList<ElementId> GetRebarIds(int optionId) =>
            _rebarSetsByOption.TryGetValue(optionId, out var sets)
                ? sets.Where(set => _document.GetElement(set.Id) != null).Select(set => set.Id).ToList()
                : [];

        private RebarBarType GetBarType(int optionId) => RebarOptions
            .SingleOrDefault(option => option.OptionId == optionId)?.DuongKinh
            ?? throw new InvalidOperationException($"Không tìm thấy loại thép cho vị trí {optionId}.");

        private void RegisterRebar(int optionId, Autodesk.Revit.DB.Structure.Rebar rebar)
        {
            if (!_rebarSetsByOption.TryGetValue(optionId, out var sets))
            {
                sets = [];
                _rebarSetsByOption[optionId] = sets;
            }

            sets.Add(rebar);
        }

        private Autodesk.Revit.DB.Structure.Rebar CreateRebar(
            int optionId,
            XYZ normal,
            IList<Curve> curves,
            bool useExistingShapeIfPossible = false)
        {
            var rebar = CreateRebar(GetBarType(optionId), normal, curves, useExistingShapeIfPossible);
            RegisterRebar(optionId, rebar);
            return rebar;
        }

        private Autodesk.Revit.DB.Structure.Rebar CreateRebar(
            RebarBarType barType,
            XYZ normal,
            IList<Curve> curves,
            bool useExistingShapeIfPossible = false)
        {
            if (curves.Count == 0)
            {
                throw new ArgumentException("Cần ít nhất một đường để tạo thép.", nameof(curves));
            }

            using var terminations = new BarTerminationsData(_document);
            return Autodesk.Revit.DB.Structure.Rebar.CreateFromCurves(
                _document,
                RebarStyle.Standard,
                barType,
                StairModel.Stair,
                normal,
                curves,
                terminations,
                useExistingShapeIfPossible,
                true)
                ?? throw new InvalidOperationException("Revit không thể tạo thanh thép từ hình học đã chọn.");
        }

        /// <summary>Giao điểm của hai đường thẳng dùng khi dựng hình học thép.</summary>
        public static XYZ FindIntersection(Line line1, Line line2)
        {
            var p1 = line1.GetEndPoint(0);
            var d1 = line1.Direction;
            var p2 = line2.GetEndPoint(0);
            var d2 = line2.Direction;

            var r = p2 - p1;
            var d1xd2 = d1.CrossProduct(d2);
            var denominator = d1xd2.DotProduct(d1xd2);

            if (denominator < 1e-10)
            {
                throw new InvalidOperationException("Hai đường thẳng không có giao điểm duy nhất để dựng hình học thép.");
            }

            var t = r.CrossProduct(d2).DotProduct(d1xd2) / denominator;
            return p1 + d1 * t;
        }
    }
}
