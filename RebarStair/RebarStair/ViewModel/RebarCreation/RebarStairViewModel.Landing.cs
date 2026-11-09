using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RebarStair.helpers;
using System.Collections.Generic;
using System.Linq;

namespace RebarStair.RebarStair.ViewModel
{
    public partial class RebarStairViewModel
    {
        void CreateLandingRebar()
        {
            var bar1 = RebarOptions.FirstOrDefault(x => x.OptionId == 1).DuongKinh;
            var bar2 = RebarOptions.FirstOrDefault(x => x.OptionId == 2).DuongKinh;
            var bar3 = RebarOptions.FirstOrDefault(x => x.OptionId == 3).DuongKinh;
            var kc = KhoangCach.MmToFeet();
            var vecX = (StairModel.P1 - StairModel.P3).Normalize();
            var vecZ = (StairModel.P2 - StairModel.P1).Normalize();
            var vecY = -vecX.CrossProduct(vecZ).Normalize();
            var a = (StairModel.P1 - StairModel.P3).GetLength();
            var offsetDistance = Cover.MmToFeet() + bar2.BarNominalDiameter / 2;
            var offsetDistance2 = Cover.MmToFeet() + bar2.BarNominalDiameter + bar2.BarNominalDiameter / 2;
            var P1 = StairModel.P1 - vecX * offsetDistance - vecY * offsetDistance + vecZ * offsetDistance2;
            var P2 = StairModel.P2 - vecX * offsetDistance - vecY * offsetDistance - vecZ * offsetDistance2;
            var P3 = StairModel.P3 + vecX * offsetDistance - vecY * offsetDistance + vecZ * offsetDistance2;
            var P4 = StairModel.P4 + vecX * offsetDistance - vecY * offsetDistance - vecZ * offsetDistance2;
            var P1D = StairModel.P1 - vecX * (offsetDistance + bar2.BarNominalDiameter / 2) - vecY * offsetDistance +
                      vecZ * offsetDistance;
            var P2D = P1D - vecY * (D.MmToFeet() - 2 * Cover.MmToFeet());
            var line1 = Line.CreateBound(P1, P3);
            var line2 = Line.CreateBound(P2, P4);

            var rebar1 =  Autodesk.Revit.DB.Structure.Rebar.CreateFromCurves(

                _document,
                RebarStyle.Standard,
                bar1,
                null,
                null,
                StairModel.Stair,
                vecY,
                new List<Curve> { line1 },
                RebarHookOrientation.Left,
                RebarHookOrientation.Left,
                false,
                true
            );
            var rebar2 = Autodesk.Revit.DB.Structure.Rebar.CreateFromCurves(
                _document,
                RebarStyle.Standard,
                bar2,
                null,
                null,
                StairModel.Stair,
                vecY,
                new List<Curve> { line2 },
                RebarHookOrientation.Left,
                RebarHookOrientation.Left,
                false,
                true
            );

            RebarShapeDrivenAccessor accessor1 = rebar1.GetShapeDrivenAccessor();
            accessor1.SetLayoutAsMaximumSpacing(
                kc,
                D.MmToFeet() - 2 * Cover.MmToFeet() - 2 * bar2.BarNominalDiameter,
                false,
                true,
                true
            );
            RebarShapeDrivenAccessor accessor2 = rebar2.GetShapeDrivenAccessor();
            accessor2.SetLayoutAsMaximumSpacing(
                kc,
                D.MmToFeet() - 2 * Cover.MmToFeet() - 2 * bar2.BarNominalDiameter,
                false,
                true,
                true
            );
            RebarParameterSetter.SetRebarParameters(
                StairModel.Stair,
                RebarTypeGroup.CN,
                0,
                rebar1,
                rebar2);
        }
    }
}



