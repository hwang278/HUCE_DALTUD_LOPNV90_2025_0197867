using Autodesk.Revit.UI;

namespace RebarStair.RebarStair.Services
{
    /// <summary>Hiển thị thông báo cho người dùng, tách khỏi ViewModel để test/thay thế được.</summary>
    public interface IDialogService
    {
        void ShowInfo(string message);

        void ShowError(string message);
    }

    public sealed class RevitDialogService : IDialogService
    {
        private const string Title = "RebarStair";

        public void ShowInfo(string message) => TaskDialog.Show(Title, message);

        public void ShowError(string message) => TaskDialog.Show(Title, message);
    }
}
