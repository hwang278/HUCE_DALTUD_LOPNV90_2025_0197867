using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
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
using Window = System.Windows.Window;
using RebarStair.Filter;


namespace RebarStair.RebarStair.ViewModel
{
    public partial class RebarStairViewModel : ObservableObject
    {
       
        public List<StairType> StairTypes { get; set; }
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
        public List<RebarBarType> RebarBarTypes { get; set; }

        public RelayCommand OkCommand { get; set; }

        public RelayCommand CloseCommand { get; set; }

        public Window CurrentWindow { get; set; }
        private readonly Document _document;

        private readonly UIDocument _uiDocument;
        public ExternalEvent PickWorkPlaneEvent { get; set; }

        private readonly ExternalEvent _runRebarEvent;

        private PickWorkPlaneHandler _pickWorkPlaneHandler;
        public Action ShowWindowAgain { get; set; }
        public RelayCommand PickWorkPlaneCommand { get; set; }

        private readonly RunRebarHandler _runRebarHandler;

        public List<RebarOption> RebarOptions { get; set; }

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

        public double Spacing
        {
            get => KhoangCach;
            set => KhoangCach = value;
        }

        public double KhoangCach
        {
            get => _khoangCach;
            set
            {
                if (value.Equals(_khoangCach)) return;
                _khoangCach = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Spacing));
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



        public double BeamWidth
        {
            get => BDam;
            set => BDam = value;
        }

        public double BDam
        {
            get => _bDam;
            set
            {
                if (value.Equals(_bDam)) return;
                _bDam = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BeamWidth));
            }
        }

        public double BeamHeight
        {
            get => HDam;
            set => HDam = value;
        }

        public double HDam
        {
            get => _hDam;
            set
            {
                if (value.Equals(_hDam)) return;
                _hDam = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BeamHeight));
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

        public double AnchorageLength
        {
            get => DoDaiNeo;
            set => DoDaiNeo = value;
        }

        public double DoDaiNeo
        {
            get => _doDaiNeo;
            set
            {
                if (value.Equals(_doDaiNeo)) return;
                _doDaiNeo = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AnchorageLength));
            }
        }

        public double ExtensionLength
        {
            get => DoVuonCuaThep;
            set => DoVuonCuaThep = value;
        }

        public double DoVuonCuaThep
        {
            get => _doVuonCuaThep;
            set
            {
                if (value.Equals(_doVuonCuaThep)) return;
                _doVuonCuaThep = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ExtensionLength));
            }
        }
        private double _cover = 20;
        private double _khoangCach = 150;

        private double _bDam = 200;

        private double _hDam = 300;

        

        private double _d = 1000;

        private double _doVuonCuaThep = 150;

        private double _doDaiNeo = 150;
        private double _r1 = 1000;
        private double _l1Cn = 150;
        private double _l2Cn = 300;

        private double _f1 = 300;

        private double _f2 = 300;
        public RebarStairViewModel(Document document, UIDocument uiDocument)
        {   
            _document = document;
            _uiDocument = uiDocument;
            
            LoadRebarBarTypes();
            
            StairTypes = new List<StairType>
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
                    Name = "Type2", ImagePath = "/RebarStair;component/Images/Type2(1).png",
                    ImagePath1 = "/RebarStair;component/Images/typ2s1.png",
                    ImagePath2 = "/RebarStair;component/Images/typ2s2.png",
                    ImagePath3 = "/RebarStair;component/Images/pallet.png"
                },
                new StairType
                {
                    Name = "Type3", ImagePath = "/RebarStair;component/Images/newtype2.png",
                    ImagePath1 = "/RebarStair;component/Images/typ2s1.png",
                    ImagePath2 = "/RebarStair;component/Images/typ2s2.png",
                    ImagePath3 = "/RebarStair;component/Images/pallet.png"
                },
                new StairType
                {
                    Name = "Chiếu Nghỉ", ImagePath = "/RebarStair;component/Images/Chieunghi.png",
                    ImagePath1 = "/RebarStair;component/Images/Chieunghi.png",
                    ImagePath2 = "/RebarStair;component/Images/PalletColour.png",
                    ImagePath3 = "/RebarStair;component/Images/pallet.png"
                },
            };
            SelectedType = StairTypes.FirstOrDefault();


            RebarOptions = new List<RebarOption>
            {
                new RebarOption
                    { OptionId = 1, Name = "1", DuongKinh = RebarBarTypes.FirstOrDefault(x => x.Name.Contains("D8")) },
                new RebarOption
                    { OptionId = 2, Name = "2", DuongKinh = RebarBarTypes.FirstOrDefault(x => x.Name.Contains("D10")) },
                new RebarOption
                    { OptionId = 3, Name = "3", DuongKinh = RebarBarTypes.FirstOrDefault(x => x.Name.Contains("D8")) },
                new RebarOption
                    { OptionId = 4, Name = "4", DuongKinh = RebarBarTypes.FirstOrDefault(x => x.Name.Contains("D10")) },
                new RebarOption
                    { OptionId = 5, Name = "5", DuongKinh = RebarBarTypes.FirstOrDefault(x => x.Name.Contains("D6")) }
            };

            var defaultOption = RebarOptions.FirstOrDefault();
            if (defaultOption != null)
            {
                defaultOption.IsChecked = true;
                SelectedRebarOption = defaultOption;
            }

            foreach (var option in RebarOptions)
            {
                option.Selected += SelectRebarOption;
            }

            _pickWorkPlaneHandler = new PickWorkPlaneHandler { UiDoc = uiDocument };
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
                _runRebarEvent.Raise();
            });
            CloseCommand = new RelayCommand(() => CurrentWindow?.Close());

            _runRebarHandler = new RunRebarHandler { ViewModel = this };
            _runRebarEvent = ExternalEvent.Create(_runRebarHandler);

        }

        private void SelectRebarOption(RebarOption selectedOption)
        {
            foreach (var option in RebarOptions)
            {
                option.IsChecked = ReferenceEquals(option, selectedOption);
            }

            SelectedRebarOption = selectedOption;
        }

        private void LoadRebarBarTypes()
        {
            RebarBarTypes = new FilteredElementCollector(_document).OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .OrderBy(x => x.Name)
                .ToList();
        }

        public void Run()
        {
            var reference = _uiDocument.Selection
                .PickObject(ObjectType.Element, new StairSelectionFilter(), "Select Stair");

            if (reference == null) return;

            var stair = _document.GetElement(reference);
            if (stair == null) return;

            StairModel = new StairModel(stair);

            switch (SelectedType.Name)
            {
                case "Type1":
                    SelectPointsForType1();
                    break;
                case "Type2":
                    SelectPointsForType2();
                    break;
                case "Type3":
                    SelectPointsForType3();
                    break;
                case "Chiếu Nghỉ":
                case "Landing":
                    SelectPointsForLanding();
                    break;

            }

            using (var tx = new Transaction(_document, "Create Rebar"))
            {
                tx.Start();

                switch (SelectedType.Name)
                {
                    case "Type1":
                        CreateType1Rebar();
                        CreateType1Stirrups();
                        break;
                    case "Type2":
                        CreateType2Rebar();
                        CreateType2Stirrups();
                        break;

                    case "Type3":
                        CreateType3Rebar();
                        CreateType3Stirrups();
                        break;
                    case "Chiếu Nghỉ":
                    case "Landing":
                        CreateLandingRebar();
                        break;
                }
                tx.Commit();
            }            
        }
    }
}
