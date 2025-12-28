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
        public List<StairType> StairType { get; set; } //= ClassBienToanCuc.StairModels;
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

        public RebarStairViewModel (UIApplication uiApp, Document doc, UIDocument uiDoc)
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
            OkCommand = new RelayCommand(() =>
            {
                CurrentWindow?.Hide();
                runRebarEvent?.Raise();
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
                case "Type3":
                    SelectPointsForType3();
                    break;
                case "Chiếu Nghỉ":
                    SelectPointsForChieuNghi();
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
                    case "Type3":
                        CreateRebarT3();
                        CreateStirrupT3();
                        break;
                    case "Chiếu Nghỉ":
                        RebarCN();
                        break;
                }
                tx.Commit();
            }            
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

        void SelectPointsForType3()
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
            XYZ v1 = (StairModel.P2 - StairModel.P1).Normalize();
            XYZ v2 = (StairModel.P11 - StairModel.P1).Normalize();

            double dot = v1.DotProduct(v2);
            if (Math.Abs(dot) > 1e-6)
            {
                throw new OperationCanceledException();
            }
        }

        void SelectPointsForChieuNghi()
        {
            StairModel.P1 = uiDoc.Selection.PickPoint();
            StairModel.P2 = uiDoc.Selection.PickPoint();
            StairModel.P3 = uiDoc.Selection.PickPoint();
            StairModel.P4 = uiDoc.Selection.PickPoint();
            XYZ v1 = (StairModel.P1 - StairModel.P3).Normalize();
            XYZ v2 = (StairModel.P3 - StairModel.P4).Normalize();

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
            RebarParameterSetter.SetRebarParameters(rebar1, StairModel.Stair);
            RebarParameterSetter.SetRebarParameters(rebar2, StairModel.Stair);
            RebarParameterSetter.SetRebarParameters(rebar3, StairModel.Stair);
            RebarParameterSetter.SetRebarParameters(rebar45, StairModel.Stair);
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
            RebarParameterSetter.SetRebarParameters(rebar1, StairModel.Stair);
            RebarParameterSetter.SetRebarParameters(rebar2, StairModel.Stair);
        }

        void CreateRebarT3()
        {
            var bar1 = RebarOptionList.FirstOrDefault(x => x.OptionId == 1).DuongKinh;
            var bar2 = RebarOptionList.FirstOrDefault(x => x.OptionId == 2).DuongKinh;
            var bar3 = RebarOptionList.FirstOrDefault(x => x.OptionId == 3).DuongKinh;
            var bar4 = RebarOptionList.FirstOrDefault(x => x.OptionId == 4).DuongKinh;
            var bar5 = RebarOptionList.FirstOrDefault(x => x.OptionId == 5).DuongKinh;
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
            var rebar1 = Autodesk.Revit.DB.Structure.Rebar.CreateFromCurves(
                doc,
                RebarStyle.Standard,
                bar2,
                null,
                null,
                StairModel.Stair,
                normal1,
                curves1,
                RebarHookOrientation.Left,
                RebarHookOrientation.Left,
                false,
                true
            );
            var curves2 = new List<Curve>() { line4, line5, line6 };
            var rebar2 = Autodesk.Revit.DB.Structure.Rebar.CreateFromCurves(
                doc,
                RebarStyle.Standard,
                bar4,
                null,
                null,
                StairModel.Stair,
                normal1,
                curves2,
                RebarHookOrientation.Left,
                RebarHookOrientation.Left,
                false,
                true
            );
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

            var rebar3 = Autodesk.Revit.DB.Structure.Rebar.CreateFromCurves(
                doc,
                RebarStyle.Standard,
                bar2,
                null,
                null,
                StairModel.Stair,
                normal3,
                new List<Curve>() { line7, line8, line8neo1, line8neo2 },
                RebarHookOrientation.Left,
                RebarHookOrientation.Left,
                false,
                true
            );
            //THEP TREN
            Plane plane4 = Plane.CreateByNormalAndOrigin(vecX, P6);
            var P7 = StairModel.P7 - norV56 * offsetDistance + vec56 * 250.MmToFeet();
            var P8 = P7 - vec56 * ((StairModel.P6 - StairModel.P5).GetLength() + 200.MmToFeet());
            var P9t = StairModel.P5 + XYZ.BasisZ * (Cover.MmToFeet() + 1.5 * DuongKinhThepChinh.BarNominalDiameter +
                                                      DuongKinhThepDai.BarNominalDiameter);
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

            var rebar4 = Autodesk.Revit.DB.Structure.Rebar.CreateFromCurves(
                doc,
                RebarStyle.Standard,
                bar4,
                null,
                null,
                StairModel.Stair,
                -normal3,
                new List<Curve>() { line9neo1, line9neo2, line9, line10 },
                RebarHookOrientation.Left,
                RebarHookOrientation.Left,
                false,
                true
            );
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
            var rebar5 = Autodesk.Revit.DB.Structure.Rebar.CreateFromCurves(
                doc,
                RebarStyle.Standard,
                bar5,
                null,
                null,
                StairModel.Stair,
                normal3,
                new List<Curve>() { lineGC1, lineGC2, lineGC3 },
                RebarHookOrientation.Left,
                RebarHookOrientation.Left,
                false,
                true
            );
            // VI TRI REBAR
            var finalRebar = new List<Autodesk.Revit.DB.Structure.Rebar>();
            finalRebar.Add(rebar1);
            finalRebar.Add(rebar2);
            finalRebar.Add(rebar3);
            finalRebar.Add(rebar4);
            finalRebar.Add(rebar5);
            var arrayLength = R1.MmToFeet() - 2 * Cover.MmToFeet();
            ElementTransformUtils.MoveElements(doc, finalRebar.Select(x => x.Id).ToList(), -vecY * Cover.MmToFeet());
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
            RebarParameterSetter.SetRebarParameters(rebar1, StairModel.Stair);
            RebarParameterSetter.SetRebarParameters(rebar2, StairModel.Stair);
            RebarParameterSetter.SetRebarParameters(rebar3, StairModel.Stair);
            RebarParameterSetter.SetRebarParameters(rebar4, StairModel.Stair);
            RebarParameterSetter.SetRebarParameters(rebar5, StairModel.Stair);

        }

        void CreateStirrupT3()
        {
            var bar1 = RebarOptionList.FirstOrDefault(x => x.OptionId == 1).DuongKinh;
            var bar2 = RebarOptionList.FirstOrDefault(x => x.OptionId == 2).DuongKinh;
            var bar3 = RebarOptionList.FirstOrDefault(x => x.OptionId == 3).DuongKinh;
            var bar4 = RebarOptionList.FirstOrDefault(x => x.OptionId == 4).DuongKinh;
            var bar5 = RebarOptionList.FirstOrDefault(x => x.OptionId == 5).DuongKinh;
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
            var rebar1 = Autodesk.Revit.DB.Structure.Rebar.CreateFromCurves(
                doc,
                RebarStyle.Standard,
                bar3,
                null,
                null,
                StairModel.Stair,
                v34,
                new List<Curve>() { line1 },
                RebarHookOrientation.Left,
                RebarHookOrientation.Left,
                false,
                true
            );

            //Thep Tren
            var v56 = (StairModel.P6 - StairModel.P5).Normalize();
            XYZ normalXZ1 = v56.CrossProduct(vecY).Normalize();
            var P5 = StairModel.P5 + v56 * 2 * offsetDistance + normalXZ1 * offsetDistance;
            var P5F = P5 - vecY * (R1.MmToFeet() - 2 * Cover.MmToFeet());
            Line line2 = Line.CreateBound(P5, P5F);
            var rebar3 = Autodesk.Revit.DB.Structure.Rebar.CreateFromCurves(
                doc,
                RebarStyle.Standard,
                bar3,
                null,
                null,
                StairModel.Stair,
                v56,
                new List<Curve>() { line2 },
                RebarHookOrientation.Left,
                RebarHookOrientation.Left,
                false,
                true
            );
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

            var rebar5 = Autodesk.Revit.DB.Structure.Rebar.CreateFromCurves(
                doc,
                RebarStyle.Standard,
                bar3,
                null,
                null,
                StairModel.Stair,
                vecX,
                new List<Curve>() { line3 },
                RebarHookOrientation.Left,
                RebarHookOrientation.Left,
                false,
                true
            );
            var rebar6 = Autodesk.Revit.DB.Structure.Rebar.CreateFromCurves(
                doc,
                RebarStyle.Standard,
                bar3,
                null,
                null,
                StairModel.Stair,
                vecX,
                new List<Curve>() { line4, line5, line6 },
                RebarHookOrientation.Left,
                RebarHookOrientation.Left,
                false,
                true
            );
            //Vi Tri Thep 
            var rebar2Copy = ElementTransformUtils.CopyElement(doc, rebar1.Id,
                normalXZ * (a - 2 * Cover.MmToFeet() - 2 * bar2.BarNominalDiameter - bar3.BarNominalDiameter));
            var rebar4Copy = ElementTransformUtils.CopyElement(doc, rebar3.Id,
                normalXZ * (a - 2 * Cover.MmToFeet() - 2 * bar2.BarNominalDiameter - bar3.BarNominalDiameter));
            var lstRebar = new List<Autodesk.Revit.DB.Structure.Rebar>();
            var rebar2 = doc.GetElement(rebar2Copy.First()) as Autodesk.Revit.DB.Structure.Rebar;
            var rebar4 = doc.GetElement(rebar4Copy.First()) as Autodesk.Revit.DB.Structure.Rebar;
            lstRebar.Add(rebar1);
            lstRebar.Add(rebar2);
            lstRebar.Add(rebar3);
            lstRebar.Add(rebar4);
            ElementTransformUtils.MoveElements(doc, lstRebar.Select(x => x.Id).ToList(), -vecY * Cover.MmToFeet());
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
            RebarParameterSetter.SetRebarParameters(rebar1, StairModel.Stair);
            RebarParameterSetter.SetRebarParameters(rebar2, StairModel.Stair);
            RebarParameterSetter.SetRebarParameters(rebar3, StairModel.Stair);
            RebarParameterSetter.SetRebarParameters(rebar4, StairModel.Stair);
            RebarParameterSetter.SetRebarParameters(rebar5, StairModel.Stair);
            RebarParameterSetter.SetRebarParameters(rebar6, StairModel.Stair);

        }

        void RebarCN()
        {
            var bar1 = RebarOptionList.FirstOrDefault(x => x.OptionId == 1).DuongKinh;
            var bar2 = RebarOptionList.FirstOrDefault(x => x.OptionId == 2).DuongKinh;
            var bar3 = RebarOptionList.FirstOrDefault(x => x.OptionId == 3).DuongKinh;
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

                doc,
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
                doc,
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
            RebarParameterSetter.SetRebarParameters(rebar1, StairModel.Stair);
            RebarParameterSetter.SetRebarParameters(rebar2, StairModel.Stair);

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