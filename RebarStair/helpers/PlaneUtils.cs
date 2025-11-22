using Autodesk.Revit.DB;

namespace RebarStair.helpers
{
    public static class PlaneUtils
    {
        public static XYZ ProjectPointOntoPlane(this XYZ point, Plane plane)
        {
            XYZ planeNormal = plane.Normal;
            XYZ planeOrigin = plane.Origin;
            XYZ vectorFromPlaneToPoint = point - planeOrigin;
            double distanceToPlane = vectorFromPlaneToPoint.DotProduct(planeNormal);
            XYZ projectedPoint = point - distanceToPlane * planeNormal;

            return projectedPoint;
        }
    }
}
