using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Autodesk.Revit.DB.Structure;
using System.Text;
using System.Threading.Tasks;

namespace RebarStair.RebarStair.Model
{
    public class RebarOptions : INotifyPropertyChanged
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
        public RebarBarType DuongKinh
        {
            get => duongKinh;
            set
            {
                if (duongKinh != value)
                {
                    duongKinh = value;
                    OnPropertyChanged(nameof(DuongKinh));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
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

        public static event Action<RebarOptions> Selected;
        public string Name { get; set; }

    }
}
