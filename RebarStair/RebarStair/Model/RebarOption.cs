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
   public  class RebarOption: INotifyPropertyChanged
    {
        private int _optionId;
        private RebarBarType _duongKinh;
        public event PropertyChangedEventHandler? PropertyChanged;

        public int OptionId
        {
            get => _optionId;
            set
            {
                if (value == _optionId) return;
                _optionId = value;
                OnPropertyChanged();
            }
        }

        public RebarBarType DuongKinh
        {
            get => _duongKinh;
            set
            {
                if (Equals(value, _duongKinh)) return;
                _duongKinh = value;
                OnPropertyChanged();
            }
        }

        public string Name { get; set; }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
