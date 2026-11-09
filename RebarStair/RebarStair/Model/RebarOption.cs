using System;
using System.ComponentModel;
using Autodesk.Revit.DB.Structure;

namespace RebarStair.RebarStair.Model
{
    public class RebarOption : INotifyPropertyChanged
    {
        public RebarOption(int optionId, string name, RebarBarType diameter)
        {
            OptionId = optionId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            duongKinh = diameter ?? throw new ArgumentNullException(nameof(diameter));
        }

        public int OptionId { get; }

        private RebarBarType duongKinh;
        public RebarBarType Diameter
        {
            get => duongKinh;
            set
            {
                // Bỏ qua null: ComboBox có thể tạm gán null khi đổi SelectedRebarOption,
                // nếu nhận vào sẽ làm mất preview và hỏng việc tạo thép.
                if (value is null || ReferenceEquals(duongKinh, value)) return;

                duongKinh = value;
                OnPropertyChanged(nameof(Diameter));
                OnPropertyChanged(nameof(DuongKinh));
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
        public string Name { get; }

    }
}
