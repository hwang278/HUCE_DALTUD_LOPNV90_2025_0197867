using Autodesk.Revit.DB;
using RebarStair.helpers;
using RebarStair.RebarStair.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RebarStair.RebarStair.ViewModel
{
    /// <summary>
    /// Phần ViewModel điều phối xem trước: tô đỏ số hiệu đang chọn và cập nhật dữ liệu mặt cắt.
    /// Không dựng hình học, không biết WPF.
    /// </summary>
    public partial class RebarStairViewModel
    {
        private bool _highlightPending;
        private bool _isSyncingSelection;
        private SectionDrawing? _section;
        private string _sectionPlaceholder = "Hãy Pick WorkPlane để xem mặt cắt.";

        public SectionDrawing? Section
        {
            get => _section;
            private set
            {
                _section = value;
                OnPropertyChanged();
            }
        }

        public string SectionPlaceholder
        {
            get => _sectionPlaceholder;
            private set
            {
                if (_sectionPlaceholder == value) return;
                _sectionPlaceholder = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Mặt cắt lấy theo work plane vừa chọn, lùi vào trong thân thang một đoạn.</summary>
        internal void SetWorkPlaneSection(PlanarFace face)
        {
            _services.Section.SetFace(face);
            UpdateSection();
        }

        internal void UpdateSection()
        {
            Section = _services.Section.Build(StairModel.Stair, GetHighlightedRebarIds());

            SectionPlaceholder = Section is not null
                ? "Hãy Pick WorkPlane để xem mặt cắt."
                : _services.Section.HasPlane
                    ? "Work plane đã chọn không cắt qua vế thang."
                    : "Hãy Pick WorkPlane để xem mặt cắt.";
        }

        /// <summary>Tô đỏ thanh thép của số hiệu đang chọn trong khung 3D.</summary>
        private void ApplyRebarHighlight()
        {
            UpdateSection();
            if (!HasPreview || _highlightPending) return;

            var dispatcher = CurrentWindow?.Dispatcher ?? System.Windows.Application.Current?.Dispatcher;
            if (dispatcher is null)
            {
                ApplyRebarHighlightCore();
                return;
            }

            // Chạy sau khi WPF xử lý xong sự kiện chọn: tránh sửa view Revit ngay trong
            // handler của RadioButton (dễ gây đệ quy khi PreviewControl vẽ lại).
            _highlightPending = true;
            dispatcher.BeginInvoke(
                new Action(() =>
                {
                    _highlightPending = false;
                    ApplyRebarHighlightCore();
                }),
                System.Windows.Threading.DispatcherPriority.Background);
        }

        private void ApplyRebarHighlightCore()
        {
            if (!HasPreview) return;

            try
            {
                _services.Preview.HighlightRebars(GetHighlightedRebarIds());
            }
            catch (Exception exception)
            {
                helpers.GeometryInspector.LogException("Highlight rebar in preview", exception);
            }
        }

        private List<ElementId> GetHighlightedRebarIds()
        {
            if (_selectedRebarOption is null || _placement is null)
            {
                return [];
            }

            return _placement.GetRebarIds(_selectedRebarOption.OptionId).ToList();
        }
    }
}
