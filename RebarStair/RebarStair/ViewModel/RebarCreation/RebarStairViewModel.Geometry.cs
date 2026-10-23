using Autodesk.Revit.DB;

namespace RebarStair.RebarStair.ViewModel
{
    public partial class RebarStairViewModel
    {
        public static XYZ FindIntersection(Line line1, Line line2)
        {
            XYZ p1 = line1.GetEndPoint(0);
            XYZ d1 = line1.Direction;
            XYZ p2 = line2.GetEndPoint(0);
            XYZ d2 = line2.Direction;

            XYZ r = p2 - p1;
            XYZ d1xd2 = d1.CrossProduct(d2);
            double denom = d1xd2.DotProduct(d1xd2);

            if (denom < 1e-10)
                return null; // Parallel or coincident lines have no unique intersection.

            double t = r.CrossProduct(d2).DotProduct(d1xd2) / denom;
            XYZ intersection = p1 + d1 * t;

            return intersection;
        }
    }
}

