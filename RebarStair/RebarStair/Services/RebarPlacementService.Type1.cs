using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RebarStair.helpers;
using RebarStair.RebarStair.Model;
using System.Collections.Generic;
using System.Linq;

namespace RebarStair.RebarStair.Services
{
    public partial class RebarPlacementService
    {
        void CreateType1Rebar()
        {
            var bar1 = GetBarType(2);
            var bar2 = GetBarType(4);
            var barGC = GetBarType(5);
            var kc = KhoangCach.MmToFeet();
            var vecY = (-StairModel.P1 + StairModel.P2).Normalize();
            var vecX = vecY.CrossProduct(XYZ.BasisZ).Normalize();
            var neodamtype1 = Line.CreateBound(StairModel.P5, StairModel.P4);
            var plane = Plane.CreateByNormalAndOrigin((StairModel.P5 - StairModel.P4).Normalize(), StairModel.P5);
            var plane1 =
                Plane.CreateByNormalAndOrigin((StairModel.P2 - StairModel.P3).Normalize(), StairModel.P2);
            var plane2 = Plane.CreateByNormalAndOrigin((StairModel.P5 - StairModel.P4).Normalize(), StairModel.P8);
            var offsetDistance = Cover.MmToFeet() + bar1.BarNominalDiameter / 2;
            var offsetDistance2 = Cover.MmToFeet() + bar2.BarNominalDiameter / 2;
            var offsetvec1 = (StairModel.P4 - StairModel.P3).Normalize();
            var norVec1 = offsetvec1.CrossProduct(vecX).Normalize();
            var P4temp = StairModel.P4 - norVec1 * offsetDistance;
            var P3temp = StairModel.P3 - norVec1 * offsetDistance + offsetvec1 * 100.MmToFeet();
            var P2temp = StairModel.P2 - vecY * offsetDistance;
            var P21temp = P2temp + XYZ.BasisZ * 300.MmToFeet();
            Line lineTemp1 = Line.CreateBound(P4temp, P3temp);
            Line lineTemp2 = Line.CreateBound(P2temp, P21temp);
            XYZ P3 = FindIntersection(lineTemp1, lineTemp2);
            var v34 = (StairModel.P4 - StairModel.P3).Normalize();
            var lengthv34 = (StairModel.P4 - StairModel.P3).GetLength();
            var P2 = P3.ProjectPointOntoPlane(plane1) - XYZ.BasisZ * F2.MmToFeet();
            var P4 = P3 + v34 * (lengthv34 + 50.MmToFeet());
            var P5 = P4 + vecY * 300.MmToFeet();

            var P5neo = P4.ProjectPointOntoPlane(plane) - XYZ.BasisZ * Cover.MmToFeet() +
                        vecY * (DoVuonCuaThep.MmToFeet() - barGC.BarNominalDiameter) + vecX * Cover.MmToFeet();
            var P4neo = P5neo.ProjectPointOntoPlane(plane2) + vecY * (Cover.MmToFeet() + barGC.BarNominalDiameter);
            var P7 = StairModel.P7 + vecY * offsetDistance - XYZ.BasisZ * offsetDistance;
            var P7neo = P7 - XYZ.BasisZ * L2CN.MmToFeet();
            var P6 = P7.ProjectPointOntoPlane(plane) + vecY * (DoVuonCuaThep.MmToFeet() - bar1.BarNominalDiameter);
            var P6neo = P6 - XYZ.BasisZ * L1CN.MmToFeet();

            Line line1 = Line.CreateBound(P2, P3);
            Line line2 = Line.CreateBound(P3, P4);
            Line line3 = Line.CreateBound(P4, P5);
            Line line4 = Line.CreateBound(P6neo, P6);
            Line line5 = Line.CreateBound(P6, P7);
            Line line6 = Line.CreateBound(P7, P7neo);
            Line line45 = Line.CreateBound(P5neo, P4neo);
            var curves1 = new List<Curve> { line1, line2, line3 };
            var curves2 = new List<Curve> { line4, line5, line6 };
            XYZ normal1 = (P3 - P2).CrossProduct(P4 - P3).Normalize();
            XYZ normal2 = (P6 - P6neo).CrossProduct(P7 - P6).Normalize();
            //Thep Tren
            var P8temp = StairModel.P8 + norVec1 * offsetDistance2;
            var P9temp = StairModel.P9 + norVec1 * offsetDistance2 + offsetvec1 * 100.MmToFeet();
            var P1temp = StairModel.P1 + vecY * offsetDistance2;
            var P11temp = P1temp + XYZ.BasisZ * 300.MmToFeet();

            Line lineTemp3 = Line.CreateBound(P8temp, P9temp);
            Line lineTemp4 = Line.CreateBound(P1temp, P11temp);

            XYZ P9 = FindIntersection(lineTemp3, lineTemp4);
            XYZ P1 = P9.ProjectPointOntoPlane(plane1) - XYZ.BasisZ * F2.MmToFeet();
            XYZ P8 = P9 + v34 * (lengthv34 + 200.MmToFeet());
            XYZ P82 = P5 - vecY * (-2 / 3 * (StairModel.P6 - StairModel.P7).GetLength());
            XYZ P81 = P82 - vecY * 1000.MmToFeet();

            Line line7 = Line.CreateBound(P1, P9);
            Line line8 = Line.CreateBound(P9, P8);
            Line line9 = Line.CreateBound(P81, P82);

            var P8Final = FindIntersection(line8, line9);
            var P8Final2 = P8Final + vecY * F1.MmToFeet();

            Line line8Final = Line.CreateBound(P9, P8Final);
            Line line9Final = Line.CreateBound(P8Final, P8Final2);

            var curves3 = new List<Curve> { line7, line8Final, line9Final };
            var curves4 = new List<Curve> { line45 };
            var rebar3 = CreateRebar(4, normal2, curves3);
            var rebar1 = CreateRebar(2, normal1, curves1);
            var rebar45 = CreateRebar(5, normal2, curves4);
            var rebar2 = CreateRebar(5, normal2, curves2);

            var finalRebar = new List<Autodesk.Revit.DB.Structure.Rebar>();
            finalRebar.Add(rebar1);
            finalRebar.Add(rebar2);
            finalRebar.Add(rebar3);
            var arrayLength = R1.MmToFeet() - 2 * Cover.MmToFeet();
            ElementTransformUtils.MoveElements(_document, finalRebar.Select(x => x.Id).ToList(), vecX * Cover.MmToFeet());
            RebarShapeDrivenAccessor accessor1 = rebar1.GetShapeDrivenAccessor();
            accessor1.SetLayoutAsMaximumSpacing(
                kc,
                arrayLength,
                false,
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
            RebarShapeDrivenAccessor accessor45 = rebar45.GetShapeDrivenAccessor();
            accessor45.SetLayoutAsMaximumSpacing(
                kc,
                arrayLength,
                true,
                true,
                true
            );
            RebarParameterSetter.SetRebarParameters(
                StairModel.Stair,
                RebarTypeGroup.Type1,
                0,
                rebar1,
                rebar2,
                rebar3,
                rebar45);
        }

        void CreateType1Stirrups()
        {
            var bar1 = GetBarType(1);
            var bar2 = GetBarType(2);
            var bar3 = GetBarType(3);
            var bar4 = GetBarType(4);
            var kc = KhoangCach.MmToFeet();
            var vecY = (-StairModel.P1 + StairModel.P2).Normalize();
            var vecX = vecY.CrossProduct(XYZ.BasisZ).Normalize();
            //Thep Duoi
            var offsetDistance = (Cover.MmToFeet() + bar2.BarNominalDiameter + bar1.BarNominalDiameter / 2);
            var offsetDistance2 = (Cover.MmToFeet() + bar4.BarNominalDiameter + bar3.BarNominalDiameter / 2);
            var v34 = (StairModel.P4 - StairModel.P3).Normalize();
            var norVec1 = v34.CrossProduct(vecX).Normalize();
            var P1 = StairModel.P3;
            var P2 = StairModel.P3 + vecX * (R1.MmToFeet() - 2 * Cover.MmToFeet());
            var line1 = Line.CreateBound(P1, P2);
            //Thep Tren
            var P3 = StairModel.P9;
            var P4 = StairModel.P9 + vecX * (R1.MmToFeet() - 2 * Cover.MmToFeet());
            var line2 = Line.CreateBound(P3, P4);
            var arraylength = (StairModel.P4 - StairModel.P3).GetLength();
            var rebar1 = CreateRebar(1, -v34, new List<Curve> { line1 }, true);
            var rebar2 = CreateRebar(3, -v34, new List<Curve> { line2 }, true);
            ElementTransformUtils.MoveElement(_document, rebar1.Id,
                -norVec1 * offsetDistance + v34 * Cover.MmToFeet() + vecX * Cover.MmToFeet());
            ElementTransformUtils.MoveElement(_document, rebar2.Id,
                norVec1 * offsetDistance2 + v34 * Cover.MmToFeet() + vecX * Cover.MmToFeet());
            RebarShapeDrivenAccessor accessor1 = rebar1.GetShapeDrivenAccessor();
            accessor1.SetLayoutAsMaximumSpacing(
                kc,
                arraylength - 6 * bar2.BarNominalDiameter,
                false,
                true,
                true
            );
            RebarShapeDrivenAccessor accessor2 = rebar2.GetShapeDrivenAccessor();
            accessor2.SetLayoutAsMaximumSpacing(
                kc,
                arraylength - 6 * bar4.BarNominalDiameter,
                false,
                true,
                true
            );
            RebarParameterSetter.SetRebarParameters(
                StairModel.Stair,
                RebarTypeGroup.Type1,
                4,
                rebar1,
                rebar2);
        }
    }
}



