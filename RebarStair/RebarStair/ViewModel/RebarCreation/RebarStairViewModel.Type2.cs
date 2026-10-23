using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RebarStair.helpers;
using System.Collections.Generic;
using System.Linq;

namespace RebarStair.RebarStair.ViewModel
{
    public partial class RebarStairViewModel
    {
        void CreateType2Rebar()
        {
            var bar1 = RebarOptions.FirstOrDefault(x => x.OptionId == 1).DuongKinh;
            var bar2 = RebarOptions.FirstOrDefault(x => x.OptionId == 2).DuongKinh;
            var bar4 = RebarOptions.FirstOrDefault(x => x.OptionId == 4).DuongKinh;
            var bar5 = RebarOptions.FirstOrDefault(x => x.OptionId == 5).DuongKinh;

            var kc = KhoangCach.MmToFeet();
            //Thep Duoi
            XYZ v1 = (StairModel.P1 - StairModel.P2).Normalize();
            XYZ v2 = (StairModel.P3 - StairModel.P2).Normalize();
            var VecY = (-StairModel.P2 + StairModel.P1).Normalize();
            var VecZ = (-StairModel.P6 + StairModel.P1).Normalize();
            var VecX = VecZ.CrossProduct(VecY).Normalize();
            XYZ perpendicularDir1 = v2.CrossProduct(VecX).Normalize();

            double offsetDistance = Cover.MmToFeet() + bar2.BarNominalDiameter / 2;

            XYZ P2t = StairModel.P2 + v2 * 2 * Cover.MmToFeet() + perpendicularDir1 * (offsetDistance);
            XYZ P3t = StairModel.P3 + perpendicularDir1 * offsetDistance;
            Line line1temp = Line.CreateBound(P2t, P3t);
            XYZ P2t2 = StairModel.P2 + XYZ.BasisZ * offsetDistance;
            XYZ P1t = StairModel.P1 + XYZ.BasisZ * offsetDistance;
            Line line2temp = Line.CreateBound(P1t, P2t2);
            var P2 = FindIntersection(line1temp, line2temp);
            XYZ finalP3 = P3t + v2 * DoVuonCuaThep.MmToFeet();
            XYZ P3neo = finalP3 + VecZ * DoDaiNeo.MmToFeet();

            Plane BPlane = Plane.CreateByNormalAndOrigin(v1, StairModel.P1);
            XYZ finalP1 = P1t + VecY * (DoVuonCuaThep.MmToFeet() - bar2.BarNominalDiameter / 2);
            XYZ P1neo = finalP1 - XYZ.BasisZ * (DoDaiNeo.MmToFeet() - ((StairModel.P6 - StairModel.P1).GetLength() - 2 * Cover.MmToFeet() - bar2.BarNominalDiameter / 2 - bar5.BarNominalDiameter / 2));
            Line line0 = Line.CreateBound(P1neo, finalP1);
            Line line1 = Line.CreateBound(finalP1, P2);
            Line line2 = Line.CreateBound(P2, finalP3);
            Line line3 = Line.CreateBound(finalP3, P3neo);
            var curves = new List<Curve> { line0, line1, line2, line3 };

            XYZ normal1 = (P2 - finalP1).CrossProduct(finalP3 - P2).Normalize();
            var rebar1 = Autodesk.Revit.DB.Structure.Rebar.CreateFromCurves(
                _document,
                RebarStyle.Standard,
                bar2,
                null,
                null,
                StairModel.Stair,
                normal1,
                curves,
                RebarHookOrientation.Left,
                RebarHookOrientation.Left,
                false,
                true
            );
            //Thep Tren
            double offsetDistance2 = Cover.MmToFeet() + bar4.BarNominalDiameter / 2;
            XYZ dirX3 = (StairModel.P4 - StairModel.P5).Normalize();
            XYZ P5temp = StairModel.P5 - perpendicularDir1 * offsetDistance2;

            XYZ tempP4 = StairModel.P4 - perpendicularDir1 * offsetDistance2;
            Line line4temp = Line.CreateBound(tempP4, P5temp);
            XYZ P5temp2 = StairModel.P5 - XYZ.BasisZ * offsetDistance2;
            XYZ newP6 = P5temp2.ProjectPointOntoPlane(BPlane);
            Line line5temp = Line.CreateBound(newP6, P5temp2);
            XYZ P5F = FindIntersection(line4temp, line5temp);
            XYZ finalP5 = P5F + dirX3 * -5 * bar4.BarNominalDiameter;
            XYZ finalP6 = finalP5 + VecY * F1.MmToFeet();
            Plane plane3 = Plane.CreateByNormalAndOrigin(VecY, finalP3);
            var P4temp2 = tempP4 - dirX3 * 500.MmToFeet();
            var linetemp23 = Line.CreateBound(finalP5, P4temp2);
            var P4 = helpers.PlaneUtils.ExtendLineToPlane(linetemp23, plane3);
            XYZ P4neo = P4 - XYZ.BasisZ * DoDaiNeo.MmToFeet();
            Line line4 = Line.CreateBound(finalP6, finalP5);
            Line line5 = Line.CreateBound(finalP5, P4);
            Line line6 = Line.CreateBound(P4, P4neo);
            var curves2 = new List<Curve> { line4, line5, line6 };
            XYZ normal2 = (finalP5 - finalP6).CrossProduct(P4 - finalP5).Normalize();
            var rebar2 = Autodesk.Revit.DB.Structure.Rebar.CreateFromCurves(
                _document,
                RebarStyle.Standard,
                bar4,
                null,
                null,
                StairModel.Stair,
                normal2,
                curves2,
                RebarHookOrientation.Left,
                RebarHookOrientation.Left,
                false,
                true
            );


            //ThepGiaCuong
            double offsetDistance3 = Cover.MmToFeet() + bar5.BarNominalDiameter / 2;

            var GCP3 = StairModel.P6 + VecZ * offsetDistance3;
            var GCP32 = GCP3 - VecY * 10000.MmToFeet();
            var lineGC2 = Line.CreateBound(GCP32, GCP3);
            var GCP2 = StairModel.P2 + perpendicularDir1 * (Cover.MmToFeet() + bar2.BarNominalDiameter +
                                                              bar1.BarNominalDiameter + bar5.BarNominalDiameter / 2);
            var GCP1F = GCP2 + v2 * 370.MmToFeet();
            var lineGC1 = Line.CreateBound(GCP1F, GCP2);
            var finalGCP12 = FindIntersection(lineGC1, lineGC2);
            XYZ finalGCP3 = GCP3 + VecY * (DoVuonCuaThep.MmToFeet() - bar5.BarNominalDiameter / 2);
            XYZ GCP3neo = finalGCP3 - XYZ.BasisZ * L2CN.MmToFeet();
            Line line7 = Line.CreateBound(finalGCP12 + dirX3 * F1.MmToFeet(), finalGCP12);
            Line line8 = Line.CreateBound(finalGCP12, finalGCP3);
            Line line9 = Line.CreateBound(finalGCP3, GCP3neo);
            var curves3 = new List<Curve> { line7, line8, line9 };
            var rebar3 = Autodesk.Revit.DB.Structure.Rebar.CreateFromCurves(
                _document,
                RebarStyle.Standard,
                bar5,
                null,
                null,
                StairModel.Stair,
                normal2,
                curves3,
                RebarHookOrientation.Left,
                RebarHookOrientation.Left,
                false,
                true
            );
            //Vi tri thep
            var finalRebar = new List<Autodesk.Revit.DB.Structure.Rebar>();
            finalRebar.Add(rebar1);
            finalRebar.Add(rebar2);
            finalRebar.Add(rebar3);
            var arrayLength = R1.MmToFeet() - 2 * Cover.MmToFeet();
            ElementTransformUtils.MoveElements(_document, finalRebar.Select(x => x.Id).ToList(), -VecX * Cover.MmToFeet());
            RebarShapeDrivenAccessor accessor1 = rebar1.GetShapeDrivenAccessor();
            accessor1.SetLayoutAsMaximumSpacing(
                kc,
                arrayLength,
                true,
                true,
                true);
            RebarShapeDrivenAccessor accessor2 = rebar2.GetShapeDrivenAccessor();
            accessor2.SetLayoutAsMaximumSpacing(
                kc,
                arrayLength,
                true,
                true,
                true
            );
            RebarShapeDrivenAccessor accessor3 = rebar3.GetShapeDrivenAccessor();
            accessor3.SetLayoutAsMaximumSpacing(
                kc,
                arrayLength,
                true,
                true,
                true
            );
            RebarParameterSetter.SetRebarParameters(
                StairModel.Stair,
                RebarTypeGroup.Type2,
                0,
                rebar1,
                rebar2,
                rebar3);
        }

        void CreateType2Stirrups()
        {
            var VecY = (-StairModel.P2 + StairModel.P1).Normalize();
            var VecZ = (-StairModel.P6 + StairModel.P1).Normalize();
            var VecX = VecZ.CrossProduct(VecY).Normalize();
            var kc = KhoangCach.MmToFeet();
            var bar1 = RebarOptions.FirstOrDefault(x => x.OptionId == 1).DuongKinh;
            var bar3 = RebarOptions.FirstOrDefault(x => x.OptionId == 3).DuongKinh;
            var bar2 = RebarOptions.FirstOrDefault(x => x.OptionId == 2).DuongKinh;
            var bar4 = RebarOptions.FirstOrDefault(x => x.OptionId == 4).DuongKinh;
            var offsetDistance = Cover.MmToFeet() + bar2.BarNominalDiameter + bar1.BarNominalDiameter / 2;
            var w = (StairModel.P6 - StairModel.P1).GetLength();
            XYZ dirX2 = (StairModel.P3 - StairModel.P2).Normalize();
            XYZ perpendicularDir1 = dirX2.CrossProduct(VecX).Normalize();
            XYZ offsetVec1 = perpendicularDir1 * offsetDistance;
            XYZ newP2 = StairModel.P2 + offsetVec1 - VecX * Cover.MmToFeet();
            XYZ finalP2 = newP2 + dirX2 * 7 * bar2.BarNominalDiameter;
            XYZ finalP21 = finalP2 - VecX * (R1.MmToFeet() - 2 * Cover.MmToFeet());
            XYZ finalP3 = finalP2 + perpendicularDir1 * (w - 2 * Cover.MmToFeet() - bar2.BarNominalDiameter - bar4.BarNominalDiameter - bar3.BarNominalDiameter);
            XYZ finalP31 = finalP3 - VecX * (R1.MmToFeet() - 2 * Cover.MmToFeet());
            Line line1 = Line.CreateBound(finalP21, finalP2);
            Line line2 = Line.CreateBound(finalP31, finalP3);
            var curve = new List<Curve> { line1 };
            var curve1 = new List<Curve> { line2 };
            var normal = -dirX2;
            var rebar1 = Autodesk.Revit.DB.Structure.Rebar.CreateFromCurves(
                _document,
                RebarStyle.Standard,
                bar1,
                null,
                null,
                StairModel.Stair,
                normal,
                curve,
                RebarHookOrientation.Left,
                RebarHookOrientation.Left,
                false,
                true
            );
            var rebar2 = Autodesk.Revit.DB.Structure.Rebar.CreateFromCurves(
                _document,
                RebarStyle.Standard,
                bar3,
                null,
                null,
                StairModel.Stair,
                normal,
                curve1,
                RebarHookOrientation.Left,
                RebarHookOrientation.Left,
                false,
                true
            );

            var length = (StairModel.P3 - (StairModel.P2 + offsetVec1)).GetLength();
            RebarShapeDrivenAccessor accessor1 = rebar1.GetShapeDrivenAccessor();
            accessor1.SetLayoutAsMaximumSpacing(
                kc,
                length - 2 * Cover.MmToFeet() - 2 * bar1.BarNominalDiameter,
                false,
                true,
                true
            );
            RebarShapeDrivenAccessor accessor2 = rebar2.GetShapeDrivenAccessor();
            accessor2.SetLayoutAsMaximumSpacing(
                kc,
                length - Cover.MmToFeet() - 2 * bar3.BarNominalDiameter,
                false,
                true,
                true
            );
            RebarParameterSetter.SetRebarParameters(
                StairModel.Stair,
                RebarTypeGroup.Type2,
                3,
                rebar1,
                rebar2);
        }
    }
}



