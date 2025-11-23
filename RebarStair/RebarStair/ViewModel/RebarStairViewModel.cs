using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Rebar.RebarStair.View;
using RebarStair.RebarStair.ExternalEvents;
using RebarStair.RebarStair.Model;
using RebarStair.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Line = Autodesk.Revit.DB.Line;
using Rebar = Autodesk.Revit.DB.Structure.Rebar;
using Window = System.Windows.Window;
using RebarStair.Filter;


namespace RebarStair.RebarStair.ViewModel
{
    public class RebarStairViewModel : ObservableObject
    {
       
        public RebarStairView RebarStairView { get; set; }
        public List<StairType> StairType { get; set; }
        public StairModel StairModel { get; set; }

        public StairType SelectedType
        {
            get => _selectedType;
            set
            {
                if (Equals(value, _selectedType)) return;
                _selectedType = value;
                OnPropertyChanged();
            }
        }
        private StairType _selectedType;
        public List<RebarBarType> DuongKinhThep { get; set; }

        public RelayCommand OkCommand { get; set; }

        public RelayCommand CloseCommand { get; set; }

        public Window CurrentWindow { get; set; }
        private Document doc;

        private UIDocument uiDoc;
        public ExternalEvent PickWorkPlaneEvent { get; set; }

        private ExternalEvent runRebarEvent;

        private PickWorkPlaneHandler _pickWorkPlaneHandler;
        public Action ShowWindowAgain { get; set; }
        public RelayCommand PickWorkPlaneCommand { get; set; }

        private RunRebarHandler runRebarHandler;

        private UIApplication uiApp;
        public List<RebarOption> RebarOptionList { get; set; }

        private RebarOption _selectedRebarOption;

        public RebarOption SelectedRebarOption
        {
            get => _selectedRebarOption;
            set
            {
                if (_selectedRebarOption != value)
                {
                    _selectedRebarOption = value;
                    OnPropertyChanged(nameof(SelectedRebarOption));
                }
            }
        }

        public double KhoangCach
        {
            get => _khoangCach;
            set
            {
                if (value.Equals(_khoangCach)) return;
                _khoangCach = value;
                OnPropertyChanged();
            }
        }

        public double Cover
        {
            get => _cover;
            set
            {
                if (value.Equals(_cover)) return;
                _cover = value;
                OnPropertyChanged();
            }
        }
        public RebarBarType DuongKinhThepChinh

        {
            get => _duongKinhThepChinh;
            set
            {
                if (Equals(value, _duongKinhThepChinh)) return;
                _duongKinhThepChinh = value;
                OnPropertyChanged();
            }
        }

        public RebarBarType DuongKinhThepGiaCuong
        {
            get => _duongKinhThepGiaCuong;
            set
            {
                if (Equals(value, _duongKinhThepGiaCuong)) return;
                _duongKinhThepGiaCuong = value;
                OnPropertyChanged();
            }
        }

        public RebarBarType DuongKinhThepDai
        {
            get => _duongKinhThepDai1;
            set
            {
                if (Equals(value, _duongKinhThepDai1)) return;
                _duongKinhThepDai1 = value;
                OnPropertyChanged();
            }
        }

        public double R1
        {
            get => _r1;
            set
            {
                if (value.Equals(_r1)) return;
                _r1 = value;
                OnPropertyChanged();
            }
        }
        public double F1
        {
            get => _f1;
            set
            {
                if (value.Equals(_f1)) return;
                _f1 = value;
                OnPropertyChanged();
            }
        }
        public double L1CN
        {
            get => _l1Cn;
            set
            {
                if (value.Equals(_l1Cn)) return;
                _l1Cn = value;
                OnPropertyChanged();
            }
        }

        public double L2CN
        {
            get => _l2Cn;
            set
            {
                if (value.Equals(_l2Cn)) return;
                _l2Cn = value;
                OnPropertyChanged();
            }
        }
        public double F2
        {
            get => _f2;
            set
            {
                if (value.Equals(_f2)) return;
                _f2 = value;
                OnPropertyChanged();
            }
        }



        public double BDam
        {
            get => _bDam;
            set
            {
                if (value.Equals(_bDam)) return;
                _bDam = value;
                OnPropertyChanged();
            }
        }

        public double HDam
        {
            get => _hDam;
            set
            {
                if (value.Equals(_hDam)) return;
                _hDam = value;
                OnPropertyChanged();
            }
        }


        public double D
        {
            get => _d;
            set
            {
                if (value.Equals(_d)) return;
                _d = value;
                OnPropertyChanged();
            }
        }

        public double DoDaiNeo
        {
            get => _doDaiNeo;
            set
            {
                if (value.Equals(_doDaiNeo)) return;
                _doDaiNeo = value;
                OnPropertyChanged();
            }
        }

        public double DoVuonCuaThep
        {
            get => _doVuonCuaThep;
            set
            {
                if (value.Equals(_doVuonCuaThep)) return;
                _doVuonCuaThep = value;
                OnPropertyChanged();
            }
        }
        private double _cover = 20;
        private RebarBarType _duongKinhThepChinh;

        private RebarBarType _duongKinhThepGiaCuong;

        private RebarBarType _duongKinhThepDai;
        private double _khoangCach = 150;

        private double _bDam = 200;

        private double _hDam = 300;

        

        private double _d = 1000;

        private double _doVuonCuaThep = 150;

        private double _doDaiNeo = 150;
        private double _r1 = 1000;
        private double _l1Cn = 150;
        private double _bThang = 100;
        private double _l2Cn = 300;

        private double _f1 = 300;

        private double _f2 = 300;
        private RebarBarType _duongKinhThepDai1;

        public RebarStairViewModel(UIApplication uiApp, Document doc, UIDocument uiDoc)
        {

            this.uiApp = uiApp;
            this.doc = doc;
            this.uiDoc = uiDoc;
            GetData();
            StairType = new List<StairType>
            {
                new StairType
                {
                    Name = "Type1", ImagePath = "/RebarStair;component/Images/newtype1.png",
                    ImagePath1 = "/RebarStair;component/Images/typ1s1.png",
                    ImagePath2 = "/RebarStair;component/Images/typ1s2.png",
                    ImagePath3 = "/RebarStair;component/Images/pallet.png"
                },
                new StairType
                {
                    Name = "Type2", ImagePath = "/RebarStair;component/Images/newtype2.png",
                    ImagePath1 = "/RebarStair;component/Images/typ2s1.png",
                    ImagePath2 = "/RebarStair;component/Images/typ2s2.png",
                    ImagePath3 = "/RebarStair;component/Images/pallet.png"
                },
                new StairType
                {
                    Name = "Chiếu Nghỉ", ImagePath = "/RebarStair;component/Images/Chieunghi.jpg",
                    ImagePath1 = "/RebarStair;component/Images/Chieunghi.png",
                    ImagePath2 = "/RebarStair;component/Images/PalletColour.png",
                    ImagePath3 = "/RebarStair;component/Images/pallet.png"
                },
            };
            SelectedType = StairType.FirstOrDefault();



            RebarOptionList = new List<RebarOption>
            {
                new RebarOption
                    { OptionId = 1, Name = "1", DuongKinh = DuongKinhThep.FirstOrDefault(x => x.Name.Contains("D8")) },
                new RebarOption
                    { OptionId = 2, Name = "2", DuongKinh = DuongKinhThep.FirstOrDefault(x => x.Name.Contains("D10")) },
                new RebarOption
                    { OptionId = 3, Name = "3", DuongKinh = DuongKinhThep.FirstOrDefault(x => x.Name.Contains("D8")) },
                new RebarOption
                    { OptionId = 4, Name = "4", DuongKinh = DuongKinhThep.FirstOrDefault(x => x.Name.Contains("D10")) },
                new RebarOption
                    { OptionId = 5, Name = "5", DuongKinh = DuongKinhThep.FirstOrDefault(x => x.Name.Contains("D6")) }
            };

            _pickWorkPlaneHandler = new PickWorkPlaneHandler { UiDoc = uiDoc };
            _pickWorkPlaneHandler.CallbackAfterPick = _ => { ShowWindowAgain?.Invoke(); };

            PickWorkPlaneEvent = ExternalEvent.Create(_pickWorkPlaneHandler);

            PickWorkPlaneCommand = new RelayCommand(() =>
            {
                CurrentWindow?.Hide();
                PickWorkPlaneEvent.Raise();
            });

            runRebarHandler = new RunRebarHandler { ViewModel = this };
            runRebarEvent = ExternalEvent.Create(runRebarHandler);

        }

        void GetData()
        {
            DuongKinhThep = new FilteredElementCollector(doc).OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .OrderBy(x => x.Name)
                .ToList();

            // Use FeetToMm for conversion
            DuongKinhThepChinh = DuongKinhThep.FirstOrDefault(x => x.BarNominalDiameter.FeetToMm() >= 6);
            DuongKinhThepGiaCuong = DuongKinhThep.FirstOrDefault(x => x.BarNominalDiameter.FeetToMm() > 6);

            DuongKinhThepDai = DuongKinhThep.FirstOrDefault(x => x.BarNominalDiameter.FeetToMm() > 6);
        }

        public void Run()
        {
            var reference = uiDoc.Selection
                .PickObject(ObjectType.Element, new StairSelectionFilter(), "Select Stair");

            if (reference == null) return;

            var stair = doc.GetElement(reference);
            if (stair == null) return;

            StairModel = new StairModel(stair);

            switch (SelectedType.Name)
            {
                case "Type1":
                    SelectPointsForType();
                    break;
                case "Type4":
                    PickpointType4();
                    break;

            }

            using (var tx = new Transaction(doc, "Create Rebar"))
            {
                tx.Start();

                switch (SelectedType.Name)
                {
                    case "Type1":
                        CreateRebarT1(stair);
                        CreateStirrupT1();
                        break;


                }

                tx.Commit();
            }

            void SelectPointsForType()
            {
                StairModel.P1 = uiDoc.Selection.PickPoint("P1");
                StairModel.P2 = uiDoc.Selection.PickPoint("P2");
                StairModel.P3 = uiDoc.Selection.PickPoint("P3");
                StairModel.P4 = uiDoc.Selection.PickPoint("P4");
                StairModel.P5 = uiDoc.Selection.PickPoint("P5");
                StairModel.P6 = uiDoc.Selection.PickPoint("P6");
                StairModel.P7 = uiDoc.Selection.PickPoint("P7");
                StairModel.P8 = uiDoc.Selection.PickPoint("P8");
                StairModel.P9 = uiDoc.Selection.PickPoint("P9");
                XYZ v1 = (StairModel.P2 - StairModel.P1).Normalize();
                XYZ v2 = (StairModel.P9 - StairModel.P1).Normalize();

                double dot = v1.DotProduct(v2);
                if (Math.Abs(dot) > 1e-6)
                {
                    throw new OperationCanceledException();

                }
            }

            void PickpointType4()
            {
                StairModel.P1 = uiDoc.Selection.PickPoint("P1");
                StairModel.P2 = uiDoc.Selection.PickPoint("P2");
                StairModel.P3 = uiDoc.Selection.PickPoint("P3");
                StairModel.P4 = uiDoc.Selection.PickPoint("P4");
                StairModel.P5 = uiDoc.Selection.PickPoint("P5");
                StairModel.P6 = uiDoc.Selection.PickPoint("P6");
                StairModel.P7 = uiDoc.Selection.PickPoint("P7");
                StairModel.P8 = uiDoc.Selection.PickPoint("P8");
                StairModel.P9 = uiDoc.Selection.PickPoint("P9");
                StairModel.P10 = uiDoc.Selection.PickPoint("P10");
                StairModel.P11 = uiDoc.Selection.PickPoint("P11");
                StairModel.P12 = uiDoc.Selection.PickPoint("P12");
                XYZ v1 = (StairModel.P2 - StairModel.P1).Normalize();
                XYZ v2 = (StairModel.P12 - StairModel.P1).Normalize();

                double dot = v1.DotProduct(v2);
                if (Math.Abs(dot) > 1e-6)
                {
                    throw new OperationCanceledException();
                }
            }

            void CreateRebarT1(Element stair)
            {
                var bar1 = RebarOptionList.FirstOrDefault(x => x.OptionId == 2).DuongKinh;
                var bar2 = RebarOptionList.FirstOrDefault(x => x.OptionId == 4).DuongKinh;
                var barGC = RebarOptionList.FirstOrDefault(x => x.OptionId == 5).DuongKinh;
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
                var rebar3 = Autodesk.Revit.DB.Structure.Rebar.CreateFromCurves(
                    doc,
                    RebarStyle.Standard,
                    bar2,
                    null,
                    null,
                    stair,
                    normal2,
                    curves3,
                    RebarHookOrientation.Left,
                    RebarHookOrientation.Left,
                    false,
                    true
                );

                var rebar1 = Autodesk.Revit.DB.Structure.Rebar.CreateFromCurves(
                    doc,
                    RebarStyle.Standard,
                    bar1,
                    null,
                    null,
                    stair,
                    normal1,
                    curves1,
                    RebarHookOrientation.Left,
                    RebarHookOrientation.Left,
                    false,
                    true
                );
                var rebar45 = Autodesk.Revit.DB.Structure.Rebar.CreateFromCurves(
                    doc,
                    RebarStyle.Standard,
                    barGC,
                    null,
                    null,
                    stair,
                    normal2,
                    curves4,
                    RebarHookOrientation.Left,
                    RebarHookOrientation.Left,
                    false,
                    true
                );

                var rebar2 = Autodesk.Revit.DB.Structure.Rebar.CreateFromCurves(
                    doc,
                    RebarStyle.Standard,
                    barGC,
                    null,
                    null,
                    stair,
                    normal2,
                    curves2,
                    RebarHookOrientation.Left,
                    RebarHookOrientation.Left,
                    false,
                    true
                );

                var finalRebar = new List<Autodesk.Revit.DB.Structure.Rebar>();
                finalRebar.Add(rebar1);
                finalRebar.Add(rebar2);
                finalRebar.Add(rebar3);
                var arrayLength = R1.MmToFeet() - 2 * Cover.MmToFeet();
                ElementTransformUtils.MoveElements(doc, finalRebar.Select(x => x.Id).ToList(), vecX * Cover.MmToFeet());
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
            }

            void CreateStirrupT1()
            {
                var bar1 = RebarOptionList.FirstOrDefault(x => x.OptionId == 1).DuongKinh;
                var bar2 = RebarOptionList.FirstOrDefault(x => x.OptionId == 2).DuongKinh;
                var bar3 = RebarOptionList.FirstOrDefault(x => x.OptionId == 3).DuongKinh;
                var bar4 = RebarOptionList.FirstOrDefault(x => x.OptionId == 4).DuongKinh;
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
                var rebar1 = Autodesk.Revit.DB.Structure.Rebar.CreateFromCurves(
                    doc,
                    RebarStyle.Standard,
                    bar1,
                    null,
                    null,
                    StairModel.Stair,
                    -v34,
                    new List<Curve> { line1 },
                    RebarHookOrientation.Right,
                    RebarHookOrientation.Right,
                    true,
                    true
                );
                var rebar2 = Autodesk.Revit.DB.Structure.Rebar.CreateFromCurves(
                    doc,
                    RebarStyle.Standard,
                    bar3,
                    null,
                    null,
                    StairModel.Stair,
                    -v34,
                    new List<Curve> { line2 },
                    RebarHookOrientation.Right,
                    RebarHookOrientation.Right,
                    true,
                    true
                );
                ElementTransformUtils.MoveElement(doc, rebar1.Id,
                    -norVec1 * offsetDistance + v34 * Cover.MmToFeet() + vecX * Cover.MmToFeet());
                ElementTransformUtils.MoveElement(doc, rebar2.Id,
                    norVec1 * offsetDistance2 + v34 * Cover.MmToFeet() + vecX * Cover.MmToFeet());
                RebarShapeDrivenAccessor accessor1 = rebar1.GetShapeDrivenAccessor();
                accessor1.SetLayoutAsMaximumSpacing(
                    kc,
                    arraylength - 6 * bar2.BarNominalDiameter - 2 * DuongKinhThepChinh.BarNominalDiameter,
                    false,
                    true,
                    true
                );
                RebarShapeDrivenAccessor accessor2 = rebar2.GetShapeDrivenAccessor();
                accessor2.SetLayoutAsMaximumSpacing(
                    kc,
                    arraylength - 6 * bar4.BarNominalDiameter - 2 * DuongKinhThepChinh.BarNominalDiameter,
                    false,
                    true,
                    true
                );
            }
        }

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
                return null; // Song song hoặc trùng nhau

            double t = r.CrossProduct(d2).DotProduct(d1xd2) / denom;
            XYZ intersection = p1 + d1 * t;

            return intersection;
        }



    }
}