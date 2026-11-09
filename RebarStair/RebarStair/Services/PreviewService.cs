using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RebarStair.helpers;
using System;
using System.Collections.Generic;

namespace RebarStair.RebarStair.Services
{
    /// <summary>Nội dung xem trước 3D: tạo control, hiện thép và tô đỏ thanh đang chọn.</summary>
    public interface IPreviewService
    {
        /// <summary>Nội dung để view host. Kiểu object để ViewModel không phụ thuộc WPF.</summary>
        object? Content { get; }

        event EventHandler? ContentChanged;

        /// <summary>Tạo lại control cho cửa sổ vừa mở.</summary>
        void ResetContent();

        void ShowRebars(IReadOnlyList<ElementId> rebarIds);

        void HighlightRebars(IReadOnlyCollection<ElementId>? rebarIds);

        /// <summary>Giải phóng control khi đóng cửa sổ.</summary>
        void ReleaseContent();
    }

    internal sealed class PreviewService : IPreviewService
    {
        private readonly RevitPreviewSession _session;
        private PreviewControl? _control;

        public PreviewService(RevitPreviewSession session) =>
            _session = session ?? throw new ArgumentNullException(nameof(session));

        public object? Content => _control;

        public event EventHandler? ContentChanged;

        public void ResetContent()
        {
            _control?.Dispose();
            _control = _session.CreateControl();
            ContentChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ShowRebars(IReadOnlyList<ElementId> rebarIds) => _session.ShowRebars(rebarIds);

        public void HighlightRebars(IReadOnlyCollection<ElementId>? rebarIds) =>
            _session.HighlightRebars(rebarIds);

        public void ReleaseContent()
        {
            _control?.Dispose();
            _control = null;
            _session.Dispose();
        }
    }
}
