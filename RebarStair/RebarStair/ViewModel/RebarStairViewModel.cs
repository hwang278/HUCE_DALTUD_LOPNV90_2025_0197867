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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace RebarStair.RebarStair.ViewModel
{
    public class RebarStairViewModel : ObservableObject
    {
        private StairType _selectedType;
        public RebarStairView RebarStairView { get; set; }
        public List<StairType> StairType { get; set; }

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

        public List<RebarBarType> DuongKinhThep { get; set; }

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

        }

        public void Run()
        {

        }
    }
}