using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace RebarStair.helpers
{
    internal sealed class RevitPreviewSession : IDisposable
    {
        private static readonly Color HighlightColor = new(255, 80, 80);
        private const int HighlightLineWeight = 2;

        private readonly Document _document;
        private readonly ElementId _hostId;
        private PreviewControl? _control;
        private ElementId[] _rebarIds = [];
        private readonly HashSet<ElementId> _highlighted = [];
        private bool _isHighlighting;
        private bool _disposed;

        private RevitPreviewSession(Document document, ElementId hostId, ElementId viewId)
        {
            _document = document;
            _hostId = hostId;
            ViewId = viewId;
        }

        internal ElementId ViewId { get; }

        internal PreviewControl CreateControl()
        {
            _control?.Dispose();
            _control = new PreviewControl(_document, ViewId);
            return _control;
        }

        internal static RevitPreviewSession Create(Document document, Element host)
        {
            ElementId viewId;
            using (var transaction = new Transaction(document, "Create rebar preview view"))
            {
                transaction.Start();
                var viewType = new FilteredElementCollector(document)
                    .OfClass(typeof(ViewFamilyType))
                    .Cast<ViewFamilyType>()
                    .First(type => type.ViewFamily == ViewFamily.ThreeDimensional);
                var view = View3D.CreateIsometric(document, viewType.Id);
                view.DetailLevel = ViewDetailLevel.Fine;
                view.DisplayStyle = DisplayStyle.Wireframe;

                var bounds = host.get_BoundingBox(null);
                if (bounds != null)
                {
                    view.SetSectionBox(new BoundingBoxXYZ
                    {
                        Transform = bounds.Transform,
                        Min = bounds.Min - new XYZ(1, 1, 1),
                        Max = bounds.Max + new XYZ(1, 1, 1)
                    });
                }

                view.IsolateElementsTemporary([host.Id]);
                viewId = view.Id;
                transaction.Commit();
            }

            return new RevitPreviewSession(document, host.Id, viewId);
        }

        internal void ShowRebars(IEnumerable<ElementId> rebarIds)
        {
            _rebarIds = rebarIds.ToArray();
            _highlighted.Clear();
            using var transaction = new Transaction(_document, "Update rebar preview view");
            transaction.Start();
            var view = (View3D)_document.GetElement(ViewId);
            if (view.IsInTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate))
            {
                view.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);
            }

            view.IsolateElementsTemporary([_hostId, .. _rebarIds]);
            transaction.Commit();

            if (_control?.IsLoaded == true)
            {
                _control.UIView.ZoomToFit();
            }
        }

        /// <summary>
        /// Tô đỏ + tăng nét các thanh thép thuộc <paramref name="rebarIds"/>,
        /// trả các thanh còn lại về màu mặc định. Truyền null để bỏ tô.
        /// </summary>
        internal void HighlightRebars(IReadOnlyCollection<ElementId>? rebarIds)
        {
            if (_disposed || _isHighlighting) return;
            if (_document.GetElement(ViewId) is not View3D view || _rebarIds.Length == 0)
            {
                return;
            }

            var target = rebarIds is null ? [] : new HashSet<ElementId>(rebarIds);
            if (target.SetEquals(_highlighted)) return;

            var highlight = new OverrideGraphicSettings();
            highlight.SetProjectionLineColor(HighlightColor);
            highlight.SetProjectionLineWeight(HighlightLineWeight);
            var reset = new OverrideGraphicSettings();

            _isHighlighting = true;
            try
            {
                using var transaction = new Transaction(_document, "Highlight rebar in preview");
                transaction.Start();
                foreach (var id in target)
                {
                    if (_document.GetElement(id) is Autodesk.Revit.DB.Structure.Rebar)
                    {
                        view.SetElementOverrides(id, highlight);
                    }
                }

                foreach (var id in _highlighted.Where(id => !target.Contains(id)))
                {
                    if (_document.GetElement(id) is Autodesk.Revit.DB.Structure.Rebar)
                    {
                        view.SetElementOverrides(id, reset);
                    }
                }

                transaction.Commit();
                _highlighted.Clear();
                _highlighted.UnionWith(target);
            }
            finally
            {
                _isHighlighting = false;
            }
        }

        internal void DeleteView()
        {
            Dispose();
            if (_document.GetElement(ViewId) == null) return;

            using var transaction = new Transaction(_document, "Delete rebar preview view");
            transaction.Start();
            _document.Delete(ViewId);
            transaction.Commit();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _control?.Dispose();
            _control = null;
            _disposed = true;
            _rebarIds = [];
            _highlighted.Clear();
        }
    }
}
