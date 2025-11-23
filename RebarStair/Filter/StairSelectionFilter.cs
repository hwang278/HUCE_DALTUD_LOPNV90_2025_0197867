using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;


namespace RebarStair.Filter
{
    public class StairSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem.Category?.Id.Value is (int)BuiltInCategory.OST_StructuralFraming or (int)BuiltInCategory.OST_Floors or (int)BuiltInCategory.OST_Stairs;

        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}
