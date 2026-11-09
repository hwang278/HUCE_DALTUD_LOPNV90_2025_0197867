using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;


namespace RebarStair.Filter
{
    public class StairSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is not ElementType && elem.Category?.CategoryType == CategoryType.Model;

        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}
