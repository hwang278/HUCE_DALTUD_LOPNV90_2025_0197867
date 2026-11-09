using Autodesk.Revit.DB;

namespace RebarStair.RebarStair.ViewModel
{
    public partial class RebarStairViewModel
    {
        void SelectPointsForType1()
        {
            StairModel.P1 = _uiDocument.Selection.PickPoint("P1");
            StairModel.P2 = _uiDocument.Selection.PickPoint("P2");
            StairModel.P3 = _uiDocument.Selection.PickPoint("P3");
            StairModel.P4 = _uiDocument.Selection.PickPoint("P4");
            StairModel.P5 = _uiDocument.Selection.PickPoint("P5");
            StairModel.P6 = _uiDocument.Selection.PickPoint("P6");
            StairModel.P7 = _uiDocument.Selection.PickPoint("P7");
            StairModel.P8 = _uiDocument.Selection.PickPoint("P8");
            StairModel.P9 = _uiDocument.Selection.PickPoint("P9");
            XYZ v1 = (StairModel.P2 - StairModel.P1).Normalize();
            XYZ v2 = (StairModel.P9 - StairModel.P1).Normalize();

            double dot = v1.DotProduct(v2);
            if (Math.Abs(dot) > 1e-6)
            {
                throw new OperationCanceledException();

            }
        }

        void SelectPointsForType2()
        {
            StairModel.P1 = _uiDocument.Selection.PickPoint("P1");
            StairModel.P2 = _uiDocument.Selection.PickPoint("P2");
            StairModel.P3 = _uiDocument.Selection.PickPoint("P3");
            StairModel.P4 = _uiDocument.Selection.PickPoint("P4");
            StairModel.P5 = _uiDocument.Selection.PickPoint("P5");
            StairModel.P6 = _uiDocument.Selection.PickPoint("P6");
            
            XYZ v1 = (StairModel.P2 - StairModel.P1).Normalize();
            XYZ v2 = (StairModel.P6 - StairModel.P1).Normalize();

            double dot = v1.DotProduct(v2);
            if (Math.Abs(dot) > 1e-6)
            {
                throw new OperationCanceledException();
            }
        }

        void SelectPointsForType3()
        {
            StairModel.P1 = _uiDocument.Selection.PickPoint("P1");
            StairModel.P2 = _uiDocument.Selection.PickPoint("P2");
            StairModel.P3 = _uiDocument.Selection.PickPoint("P3");
            StairModel.P4 = _uiDocument.Selection.PickPoint("P4");
            StairModel.P5 = _uiDocument.Selection.PickPoint("P5");
            StairModel.P6 = _uiDocument.Selection.PickPoint("P6");
            StairModel.P7 = _uiDocument.Selection.PickPoint("P7");
            StairModel.P8 = _uiDocument.Selection.PickPoint("P8");
            StairModel.P9 = _uiDocument.Selection.PickPoint("P9");
            StairModel.P10 = _uiDocument.Selection.PickPoint("P10");
            StairModel.P11 = _uiDocument.Selection.PickPoint("P11");
            XYZ v1 = (StairModel.P2 - StairModel.P1).Normalize();
            XYZ v2 = (StairModel.P11 - StairModel.P1).Normalize();

            double dot = v1.DotProduct(v2);
            if (Math.Abs(dot) > 1e-6)
            {
                throw new OperationCanceledException();
            }
        }

        void SelectPointsForLanding()
        {
            StairModel.P1 = _uiDocument.Selection.PickPoint();
            StairModel.P2 = _uiDocument.Selection.PickPoint();
            StairModel.P3 = _uiDocument.Selection.PickPoint();
            StairModel.P4 = _uiDocument.Selection.PickPoint();
            XYZ v1 = (StairModel.P1 - StairModel.P3).Normalize();
            XYZ v2 = (StairModel.P3 - StairModel.P4).Normalize();

            double dot = v1.DotProduct(v2);
            if (Math.Abs(dot) > 1e-6)
            {
                throw new OperationCanceledException();
            }
        }
    }
}



