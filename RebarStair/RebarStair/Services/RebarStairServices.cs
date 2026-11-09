using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RebarStair.RebarStair.Model;
using System;
using System.Collections.Generic;

namespace RebarStair.RebarStair.Services
{
    /// <summary>
    /// Composition root cho ViewModel: gom Document và toàn bộ service cần dùng.
    /// ViewModel chỉ nhận đúng những gì nó cần qua constructor.
    /// </summary>
    public sealed class RebarStairServices
    {
        public RebarStairServices(Document document, UIDocument uiDocument, IPreviewService preview)
        {
            Document = document ?? throw new ArgumentNullException(nameof(document));
            Preview = preview ?? throw new ArgumentNullException(nameof(preview));

            if (uiDocument is null) throw new ArgumentNullException(nameof(uiDocument));

            Dialogs = new RevitDialogService();
            Points = new PointSelectionService(uiDocument);
            WorkPlane = new WorkPlanePicker(uiDocument);
            Section = new SectionService(document);
        }

        public Document Document { get; }

        public IPreviewService Preview { get; }

        public IDialogService Dialogs { get; }

        public PointSelectionService Points { get; }

        public WorkPlanePicker WorkPlane { get; }

        public ISectionService Section { get; }

        public RebarPlacementService CreatePlacement(
            StairModel stairModel,
            RebarParameters parameters,
            IReadOnlyList<RebarOption> options) =>
            new(Document, stairModel, parameters, options);
    }
}
