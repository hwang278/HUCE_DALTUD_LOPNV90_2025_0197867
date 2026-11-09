using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Rebar.RebarStair.View;
using RebarStair.RebarStair.Model;
using RebarStair.RebarStair.Services;
using RebarStair.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Window = System.Windows.Window;


namespace RebarStair.RebarStair.ViewModel
{
    public partial class RebarStairViewModel : ObservableObject
    {
       
        public List<StairType> StairTypes { get; }
        public StairModel StairModel { get; }

        public StairType SelectedType
        {
            get => _selectedType;
            set
            {
                if (Equals(value, _selectedType)) return;
                _selectedType = value;
                OnPropertyChanged();
                HasPreview = false;
                UpdateSection();
            }
        }
        private StairType _selectedType;
        private readonly RebarStairServices _services;
        private List<ElementId> _previewRebarIds = [];
        private RebarPlacementService? _placement;
        private bool _hasPreview;

        public object? PreviewContent => _services.Preview.Content;
        public bool IsAccepted { get; private set; }
        internal bool IsPreviewRequested { get; private set; }
        internal bool IsWorkPlaneRequested { get; private set; }
        public bool HasPreview
        {
            get => _hasPreview;
            private set
            {
                if (_hasPreview == value) return;
                _hasPreview = value;
                OnPropertyChanged();
            }
        }
        public List<RebarBarType> RebarBarTypes { get; }

        public RelayCommand OkCommand { get; }
        public RelayCommand PreviewCommand { get; }

        public RelayCommand CloseCommand { get; }

        public Window? CurrentWindow { get; set; }
        private readonly Document _document;

        public RelayCommand PickWorkPlaneCommand { get; }

        public List<RebarOption> RebarOptions { get; }

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
                    ApplyRebarHighlight();
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
                HasPreview = false;
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
                HasPreview = false;
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
                HasPreview = false;
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
                HasPreview = false;
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
                HasPreview = false;
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
                HasPreview = false;
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
                HasPreview = false;
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
                HasPreview = false;
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
                HasPreview = false;
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
                HasPreview = false;
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
                HasPreview = false;
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
                HasPreview = false;
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
        internal RebarStairViewModel(RebarStairServices services, Element stair)
        {
            _services = services;
            _document = services.Document;
            StairModel = new StairModel(stair);
            _services.Preview.ContentChanged += (_, _) => OnPropertyChanged(nameof(PreviewContent));
            
            RebarBarTypes = LoadRebarBarTypes();
            if (RebarBarTypes.Count == 0)
            {
                throw new InvalidOperationException("Dự án chưa có Rebar Bar Type.");
            }
            
            StairTypes = new List<StairType>
            {
                new StairType { Name = "Type1" },
                new StairType { Name = "Type2" },
                new StairType { Name = "Type3" },
                new StairType { Name = "Chiếu Nghỉ" },
            };
            _selectedType = StairTypes[0];


            RebarOptions = new List<RebarOption>
            {
                new(1, "1", FindBarType("D8")),
                new(2, "2", FindBarType("D10")),
                new(3, "3", FindBarType("D8")),
                new(4, "4", FindBarType("D10")),
                new(5, "5", FindBarType("D6"))
            };

            _selectedRebarOption = RebarOptions[0];
            _selectedRebarOption.IsChecked = true;

            foreach (var option in RebarOptions)
            {
                option.Selected += SelectRebarOption;
                option.PropertyChanged += (_, args) =>
                {
                    if (args.PropertyName is nameof(RebarOption.Diameter) or nameof(RebarOption.DuongKinh))
                    {
                        HasPreview = false;
                    }
                };
            }

            PickWorkPlaneCommand = new RelayCommand(() =>
            {
                IsWorkPlaneRequested = true;
                CurrentWindow?.Close();
            });
            PreviewCommand = new RelayCommand(() =>
            {
                IsPreviewRequested = true;
                CurrentWindow?.Close();
            });
            OkCommand = new RelayCommand(() =>
            {
                if (!HasPreview)
                {
                    _services.Dialogs.ShowInfo("Hãy bố trí và kiểm tra preview thép trước khi xác nhận.");
                    return;
                }

                IsAccepted = true;
                CurrentWindow?.Close();
            });
            CloseCommand = new RelayCommand(() => CurrentWindow?.Close());

        }

        public void ClearViewport()
        {
            _services.Preview.ReleaseContent();
            OnPropertyChanged(nameof(PreviewContent));
        }

        internal void AttachWindow(Window window)
        {
            CurrentWindow = window;
            IsPreviewRequested = false;
            IsWorkPlaneRequested = false;
            _services.Preview.ResetContent();
            ApplyRebarHighlight();
        }

        private void SelectRebarOption(RebarOption selectedOption)
        {
            if (_isSyncingSelection) return;

            _isSyncingSelection = true;
            try
            {
                foreach (var option in RebarOptions)
                {
                    option.IsChecked = ReferenceEquals(option, selectedOption);
                }

                SelectedRebarOption = selectedOption;
            }
            finally
            {
                _isSyncingSelection = false;
            }
        }

        /// <summary>Gom các tham số đang bind trên giao diện thành dữ liệu cho service dựng thép.</summary>
        private RebarParameters BuildRebarParameters() => new()
        {
            Cover = Cover,
            Spacing = KhoangCach,
            StairWidth = R1,
            LandingWidth = D,
            F1 = F1,
            F2 = F2,
            L1 = L1CN,
            L2 = L2CN,
            BeamWidth = BDam,
            BeamHeight = HDam,
            AnchorageLength = DoDaiNeo,
            ExtensionLength = DoVuonCuaThep
        };

        private List<RebarBarType> LoadRebarBarTypes()        {
            return new FilteredElementCollector(_document).OfClass(typeof(RebarBarType))
                .Cast<RebarBarType>()
                .OrderBy(x => x.Name)
                .ToList();
        }

        private RebarBarType FindBarType(string namePart) => RebarBarTypes
            .FirstOrDefault(type => type.Name.Contains(namePart, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Không tìm thấy Rebar Bar Type chứa '{namePart}'.");

        internal void CreateRebarPreview()
        {
            GeometryInspector.LogEvent("Create rebar preview", $"Started for {SelectedType.Name}.");
            try
            {
                _services.Points.Select(StairModel, SelectedType.Name);

                GeometryInspector.LogEvent("Create rebar preview", "Point selection completed.");

                _placement = _services.CreatePlacement(StairModel, BuildRebarParameters(), RebarOptions);
                _previewRebarIds = _placement.Build(SelectedType.Name, _previewRebarIds).ToList();

                _services.Preview.ShowRebars(_previewRebarIds);
                HasPreview = _previewRebarIds.Count > 0;
                ApplyRebarHighlight();
                GeometryInspector.LogEvent(
                    "Create rebar preview",
                    $"Completed with {_previewRebarIds.Count} rebar elements.");
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                GeometryInspector.LogEvent("Create rebar preview", "Point selection cancelled by user.");
            }
            catch (OperationCanceledException)
            {
                GeometryInspector.LogEvent("Create rebar preview", "Point validation cancelled the operation.");
            }
            catch (Exception exception)
            {
                GeometryInspector.LogException("Create rebar preview", exception);
                _services.Dialogs.ShowError(exception.ToString());
            }
        }

        internal void PickWorkPlane()
        {
            try
            {
                var planarFace = _services.WorkPlane.PickAndApply();
                if (planarFace is null) return;

                // Mặt cắt 2D lấy theo work plane vừa chọn, lùi vào trong thân thang một đoạn.
                SetWorkPlaneSection(planarFace);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
            }
        }
    }
}
