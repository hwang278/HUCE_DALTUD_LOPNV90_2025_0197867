using System;
using System.ComponentModel;
using Autodesk.Revit.DB.Structure;

namespace RebarStair.RebarStair.Model
{
    public class RebarOption : INotifyPropertyChanged
    {
        private int optionId;
        public int OptionId
        {
            get => optionId;
            set
            {
                if (optionId != value)
                {
                    optionId = value;
                    OnPropertyChanged(nameof(OptionId));
                }
            }
        }

        private RebarBarType duongKinh;
        public RebarBarType Diameter
        {
            get => duongKinh;
            set
            {
                if (duongKinh != value)
                {
                    duongKinh = value;
                    OnPropertyChanged(nameof(Diameter));
                    OnPropertyChanged(nameof(DuongKinh));
                }
            }
        }

        public RebarBarType DuongKinh
        {
            get => Diameter;
            set => Diameter = value;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                    if (value)
                        Selected?.Invoke(this);
                }
            }
        }

        public event Action<RebarOption>? Selected;
        public string Name { get; set; }

    }
}
