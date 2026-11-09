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
        public static XYZ ExtendLineToPlane(Line line, Plane plane)
        {
            // Lấy điểm đầu và hướng của line
            XYZ pointOnLine = line.GetEndPoint(0);
            XYZ direction = line.Direction;

            // Vector từ điểm trên mặt phẳng đến điểm trên line
            XYZ vector = pointOnLine - plane.Origin;

            // Tính khoảng cách từ điểm đến mặt phẳng theo pháp tuyến
            double denominator = direction.DotProduct(plane.Normal);

            // A required construction line without an intersection cannot produce valid rebar geometry.
            if (Math.Abs(denominator) < 1e-9)
                throw new InvalidOperationException("Đường thẳng song song với mặt phẳng, không thể dựng hình học thép.");

            double t = -vector.DotProduct(plane.Normal) / denominator;

            // Điểm giao
            XYZ intersection = pointOnLine + t * direction;
            return intersection;
        }
    }
}
