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
        void CreateType3Rebar()
        {
            var bar1 = GetBarType(1);
            var bar2 = GetBarType(2);
            var bar3 = GetBarType(3);
            var bar4 = GetBarType(4);
            var bar5 = GetBarType(5);
            var vecX = (StairModel.P2 - StairModel.P1).Normalize();
            var vecY = XYZ.BasisZ.CrossProduct(vecX).Normalize();
            var kc = KhoangCach.MmToFeet();
            var a = (StairModel.P9 - StairModel.P8).GetLength();
            var plane = Plane.CreateByNormalAndOrigin((StairModel.P5 - StairModel.P4).Normalize(), StairModel.P5);
            var plane1 =
                Plane.CreateByNormalAndOrigin((StairModel.P2 - StairModel.P3).Normalize(), StairModel.P2);
            var offsetDistance = Cover.MmToFeet() + bar2.BarNominalDiameter / 2;
            var offsetDistance2 = Cover.MmToFeet() + bar4.BarNominalDiameter / 2;
            var P9temp = StairModel.P9.ProjectPointOntoPlane(plane);
            var hCN = (StairModel.P5 - P9temp).GetLength();
            var offsetvec1 = (StairModel.P11 - StairModel.P3).Normalize();
            var v34 = (StairModel.P4 - StairModel.P3).Normalize();
            var lengthv34 = (StairModel.P4 - StairModel.P3).GetLength();
            var v1011 = (StairModel.P10 - StairModel.P11).Normalize();
            var nor1011 = v1011.CrossProduct(vecY).Normalize();
            var nor34 = v34.CrossProduct(vecY).Normalize();
            var P2 = StairModel.P2 - vecX * offsetDistance - XYZ.BasisZ * F2.MmToFeet();
            var P3t1 = StairModel.P3 - vecX * offsetDistance;
            var P3t2 = StairModel.P3 + nor34 * offsetDistance;
            var P4t = StairModel.P4 + nor34 * offsetDistance;
            var linetempx1 = Line.CreateBound(P2, P3t1);
            var linetempx2 = Line.CreateBound(P3t2, P4t);
            var P3 = FindIntersection(linetempx1, linetempx2);
            var P4 = P3 + v34 * (lengthv34 + (hCN * 2 / 3));
            var P5 = P4 + vecX * F1.MmToFeet();

            var P11t1 = StairModel.P11 + vecX * offsetDistance2;
            var P11t2 = StairModel.P11 - nor1011 * offsetDistance2;
            var P1 = StairModel.P1 - XYZ.BasisZ * F2.MmToFeet() + vecX * offsetDistance2;
            var P10_temp = StairModel.P10 + v1011 * lengthv34 - nor1011 * offsetDistance2;
            var linetemp1 = Line.CreateBound(P1, P11t1);
            var linetemp2 = Line.CreateBound(P11t2, P10_temp);
            var P11 = FindIntersection(linetemp1, linetemp2);
            var plane10 = Plane.CreateByNormalAndOrigin(-XYZ.BasisZ, P5);
            var P10 = helpers.PlaneUtils.ExtendLineToPlane(linetemp2, plane10);
            var P9 = P10 + vecX * F1.MmToFeet();
            var line1 = Line.CreateBound(P2, P3);
            var line2 = Line.CreateBound(P3, P4);
            var line3 = Line.CreateBound(P4, P5);
            var line4 = Line.CreateBound(P1, P11);
            var line5 = Line.CreateBound(P11, P10);
            var line6 = Line.CreateBound(P10, P9);
            XYZ normal1 = (P3 - P2).CrossProduct(P4 - P3).Normalize();

            var curves1 = new List<Curve>() { line1, line2, line3 };
            var rebar1 = CreateRebar(2, normal1, curves1);
            var curves2 = new List<Curve>() { line4, line5, line6 };
            var rebar2 = CreateRebar(4, normal1, curves2);
            //THEP BEN TRAI
            //THEP DUOI
            var plane3 =
                Plane.CreateByNormalAndOrigin((StairModel.P4 - StairModel.P5).Normalize(), StairModel.P10);
            var P4temp = StairModel.P4 + XYZ.BasisZ * offsetDistance;
            var P4temp2 = P4temp.ProjectPointOntoPlane(plane3) + vecX * offsetDistance;
            var vec56 = (StairModel.P6 - StairModel.P5).Normalize();
            var norV56 = vec56.CrossProduct(vecY).Normalize();
            var P6 = StairModel.P6 + norV56 * offsetDistance + vec56 * 180.MmToFeet();
            var P5temp = P6 - vec56 * (StairModel.P6 - StairModel.P5).GetLength();
            var P5temp2 = P4temp.ProjectPointOntoPlane(plane);
            var P61 = P6 + vecX * 180.MmToFeet();
            var P6neo = P61 - XYZ.BasisZ * DoDaiNeo.MmToFeet();
            Line line56 = Line.CreateBound(P6, P5temp);
            Line line46 = Line.CreateBound(P4temp, P5temp2);
            XYZ P5F = FindIntersection(line46, line56);
            Line line7 = Line.CreateBound(P4temp2, P5F);
            Line line8 = Line.CreateBound(P5F, P6);
            Line line8neo1 = Line.CreateBound(P6, P61);
            Line line8neo2 = Line.CreateBound(P61, P6neo);
            XYZ normal3 = (StairModel.P6 - StairModel.P5).CrossProduct(vecX).Normalize();

            var rebar3 = CreateRebar(
                2,
                normal3,
                new List<Curve>() { line7, line8, line8neo1, line8neo2 });
            //THEP TREN
            Plane plane4 = Plane.CreateByNormalAndOrigin(vecX, P6);
            var P7 = StairModel.P7 - norV56 * offsetDistance + vec56 * 250.MmToFeet();
            var P8 = P7 - vec56 * ((StairModel.P6 - StairModel.P5).GetLength() + 200.MmToFeet());
            var P9t = StairModel.P5 + XYZ.BasisZ * (Cover.MmToFeet() + 1.5 * bar2.BarNominalDiameter +
                                                      bar3.BarNominalDiameter);
            var P10t = P9t.ProjectPointOntoPlane(plane3) + vecX * (a - 340.MmToFeet());
            Line line78 = Line.CreateBound(P7, P8);
            Line line90 = Line.CreateBound(P9t, P10t);
            XYZ P8F = FindIntersection(line78, line90);
            XYZ P7Ex = helpers.PlaneUtils.ExtendLineToPlane(line78, plane4);
            var P71 = P7Ex + vecX * 180.MmToFeet();
            var P7neo1 = P71 - XYZ.BasisZ * DoDaiNeo.MmToFeet();
            Line line9neo1 = Line.CreateBound(P7neo1, P71);
            Line line9neo2 = Line.CreateBound(P71, P7Ex);
            Line line9 = Line.CreateBound(P7Ex, P8F);
            Line line10 = Line.CreateBound(P8F, P10t);

            var rebar4 = CreateRebar(
                4,
                -normal3,
                new List<Curve>() { line9neo1, line9neo2, line9, line10 });
            // THEP GIA CUONG
            var offsetDistance3 = Cover.MmToFeet() + bar5.BarNominalDiameter / 2;
            var PGC2 = StairModel.P5 + norV56 * (Cover.MmToFeet() + bar2.BarNominalDiameter +
                                                   bar1.BarNominalDiameter + bar5.BarNominalDiameter / 2);
            var PGC1 = PGC2 + vec56 * (400.MmToFeet());
            var PGC4 = StairModel.P9 - XYZ.BasisZ * offsetDistance3 + vecX * offsetDistance3;
            var PGC3 = PGC4 + vecX * (1.5 * a);
            var PGC4neo = PGC4 - XYZ.BasisZ * 2 * hCN;
            var linegc1 = Line.CreateBound(PGC1, PGC2);
            var linegc2 = Line.CreateBound(PGC4, PGC3);
            var PGC2F = FindIntersection(linegc1, linegc2);
            var lineGC1 = Line.CreateBound(PGC1, PGC2F);
            var lineGC2 = Line.CreateBound(PGC2F, PGC4);
            var lineGC3 = Line.CreateBound(PGC4, PGC4neo);
            var rebar5 = CreateRebar(5, normal3, new List<Curve>() { lineGC1, lineGC2, lineGC3 });
            // VI TRI REBAR
            var finalRebar = new List<Autodesk.Revit.DB.Structure.Rebar>();
            finalRebar.Add(rebar1);
            finalRebar.Add(rebar2);
            finalRebar.Add(rebar3);
            finalRebar.Add(rebar4);
            finalRebar.Add(rebar5);
            var arrayLength = R1.MmToFeet() - 2 * Cover.MmToFeet();
            ElementTransformUtils.MoveElements(_document, finalRebar.Select(x => x.Id).ToList(), -vecY * Cover.MmToFeet());
            RebarShapeDrivenAccessor accessor1 = rebar1.GetShapeDrivenAccessor();
            accessor1.SetLayoutAsMaximumSpacing(
                kc,
                arrayLength,
                false,
                true,
                true
            );
            RebarShapeDrivenAccessor accessor2 = rebar2.GetShapeDrivenAccessor();
            accessor2.SetLayoutAsMaximumSpacing(
                kc,
                arrayLength,
                false,
                true,
                true
            );
            RebarShapeDrivenAccessor accessor3 = rebar3.GetShapeDrivenAccessor();
            accessor3.SetLayoutAsMaximumSpacing(
                kc,
                arrayLength,
                false,
                true,
                true
            );
            RebarShapeDrivenAccessor accessor4 = rebar4.GetShapeDrivenAccessor();
            accessor4.SetLayoutAsMaximumSpacing(
                kc,
                arrayLength,
                true,
                true,
                true
            );
            RebarShapeDrivenAccessor accessor5 = rebar5.GetShapeDrivenAccessor();
            accessor5.SetLayoutAsMaximumSpacing(
                kc,
                arrayLength,
                false,
                true,
                true
            );
            RebarParameterSetter.SetRebarParameters(
                StairModel.Stair,
                RebarTypeGroup.Type3,
                0,
                rebar1,
                rebar2,
                rebar3,
                rebar4,
                rebar5);

        }

        void CreateType3Stirrups()
        {
            var bar1 = GetBarType(1);
            var bar2 = GetBarType(2);
            var bar3 = GetBarType(3);
            var bar4 = GetBarType(4);
            var bar5 = GetBarType(5);
            // Thep Duoi
            var vecX = (StairModel.P2 - StairModel.P1).Normalize();
            var vecY = XYZ.BasisZ.CrossProduct(vecX).Normalize();
            var arraylength1 = (StairModel.P4 - StairModel.P3).GetLength() - 2 * Cover.MmToFeet() -
                               2 * bar3.BarNominalDiameter;
            var arraylength2 = (StairModel.P6 - StairModel.P5).GetLength() - 2 * Cover.MmToFeet() -
                               2 * bar3.BarNominalDiameter;
            var a = (StairModel.P1 - StairModel.P2).GetLength();
            var kc = KhoangCach.MmToFeet();
            var offsetDistance = Cover.MmToFeet() + bar2.BarNominalDiameter + bar3.BarNominalDiameter / 2;
            var v34 = (StairModel.P4 - StairModel.P3).Normalize();
            XYZ normalXZ = v34.CrossProduct(vecY).Normalize();
            var v23 = StairModel.P3 - StairModel.P2;
            var P2 = StairModel.P2 - vecX * offsetDistance;
            var P2temp = P2 + v23 * 500.MmToFeet();
            var P3 = StairModel.P3 + normalXZ * offsetDistance + v34 * 100.MmToFeet();
            var P4 = StairModel.P4 + normalXZ * offsetDistance;
            Line line2temp = Line.CreateBound(P4, P3);
            Line line1temp = Line.CreateBound(P2, P2temp);
            XYZ P3temp = FindIntersection(line1temp, line2temp);
            var vecDistance = (P3temp - StairModel.P3).Normalize();
            var P3F = P3temp + v34 * offsetDistance;
            var P3F1 = P3F - vecY * (R1.MmToFeet() - 2 * Cover.MmToFeet());
            Line line1 = Line.CreateBound(P3F, P3F1);
            var rebar1 = CreateRebar(3, v34, new List<Curve>() { line1 });

            //Thep Tren
            var v56 = (StairModel.P6 - StairModel.P5).Normalize();
            XYZ normalXZ1 = v56.CrossProduct(vecY).Normalize();
            var P5 = StairModel.P5 + v56 * 2 * offsetDistance + normalXZ1 * offsetDistance;
            var P5F = P5 - vecY * (R1.MmToFeet() - 2 * Cover.MmToFeet());
            Line line2 = Line.CreateBound(P5, P5F);
            var rebar3 = CreateRebar(3, v56, new List<Curve>() { line2 });
            //Thep Giua
            var a1 = (StairModel.P9 - StairModel.P8).GetLength()
                     - (2 * Cover.MmToFeet() + offsetDistance);
            var plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, StairModel.P5);
            var P5temp1 = StairModel.P8.ProjectPointOntoPlane(plane);

            var x = (StairModel.P8 - P5temp1).GetLength();

            var P5g = P5temp1
                      + XYZ.BasisZ * offsetDistance
                      - vecX * (Cover.MmToFeet() + bar3.BarNominalDiameter)
                      + vecY * (2.0 / 3.0 * BDam.MmToFeet());

            var P5g1 = P5g - vecY * (2.0 / 3.0 * BDam.MmToFeet() + R1.MmToFeet() - Cover.MmToFeet());

            var P8 = P5g +
                     XYZ.BasisZ * (x - 2 * Cover.MmToFeet() - bar2.BarNominalDiameter - bar3.BarNominalDiameter -
                                   bar5.BarNominalDiameter) + vecY * bar3.BarNominalDiameter / 2;

            var P8neo = P8 - XYZ.BasisZ * (2.0 / 3.0 * HDam.MmToFeet());

            var P8F = P8 - vecY * (2.0 / 3.0 * BDam.MmToFeet() + R1.MmToFeet() - Cover.MmToFeet());

            var P8Fneo = P8F - XYZ.BasisZ * (2.0 / 3.0 * x);

            Line line3 = Line.CreateBound(P5g, P5g1);
            Line line4 = Line.CreateBound(P8neo, P8);
            Line line5 = Line.CreateBound(P8, P8F);
            Line line6 = Line.CreateBound(P8F, P8Fneo);

            var rebar5 = CreateRebar(3, vecX, new List<Curve>() { line3 });
            var rebar6 = CreateRebar(3, vecX, new List<Curve>() { line4, line5, line6 });
            //Vi Tri Thep 
            var rebar2Copy = ElementTransformUtils.CopyElement(_document, rebar1.Id,
                normalXZ * (a - 2 * Cover.MmToFeet() - 2 * bar2.BarNominalDiameter - bar3.BarNominalDiameter));
            var rebar4Copy = ElementTransformUtils.CopyElement(_document, rebar3.Id,
                normalXZ * (a - 2 * Cover.MmToFeet() - 2 * bar2.BarNominalDiameter - bar3.BarNominalDiameter));
            var lstRebar = new List<Autodesk.Revit.DB.Structure.Rebar>();
            var rebar2 = _document.GetElement(rebar2Copy.Single()) as Autodesk.Revit.DB.Structure.Rebar
                ?? throw new InvalidOperationException("Không lấy được bản sao thép 2.");
            var rebar4 = _document.GetElement(rebar4Copy.Single()) as Autodesk.Revit.DB.Structure.Rebar
                ?? throw new InvalidOperationException("Không lấy được bản sao thép 4.");
            // Bản sao không đi qua CreateRebar nên phải gắn số hiệu thủ công.
            RegisterRebar(3, rebar2);
            RegisterRebar(3, rebar4);
            lstRebar.Add(rebar1);
            lstRebar.Add(rebar2);
            lstRebar.Add(rebar3);
            lstRebar.Add(rebar4);
            ElementTransformUtils.MoveElements(_document, lstRebar.Select(x => x.Id).ToList(), -vecY * Cover.MmToFeet());
            RebarShapeDrivenAccessor accessor1 = rebar1.GetShapeDrivenAccessor();
            accessor1.SetLayoutAsMaximumSpacing(
                kc,
                arraylength1,
                true,
                true,
                true
            );
            RebarShapeDrivenAccessor accessor2 = rebar2.GetShapeDrivenAccessor();
            accessor2.SetLayoutAsMaximumSpacing(
                kc,
                arraylength1,
                true,
                true,
                true
            );
            RebarShapeDrivenAccessor accessor3 = rebar3.GetShapeDrivenAccessor();
            accessor3.SetLayoutAsMaximumSpacing(
                kc,
                arraylength2,
                true,
                true,
                true
            );
            RebarShapeDrivenAccessor accessor4 = rebar4.GetShapeDrivenAccessor();
            accessor4.SetLayoutAsMaximumSpacing(
                kc,
                arraylength2,
                true,
                true,
                true
            );
            RebarShapeDrivenAccessor accessor5 = rebar5.GetShapeDrivenAccessor();
            accessor5.SetLayoutAsMaximumSpacing(
                kc,
                a1,
                false,
                true,
                true
            );
            RebarShapeDrivenAccessor accessor6 = rebar6.GetShapeDrivenAccessor();
            accessor6.SetLayoutAsMaximumSpacing(
                kc,
                a1,
                false,
                true,
                true
            );
            RebarParameterSetter.SetRebarParameters(
                StairModel.Stair,
                RebarTypeGroup.Type3,
                5,
                rebar1,
                rebar2,
                rebar3,
                rebar4,
                rebar5,
                rebar6);

        }
    }
}



