using Axis2.WPF.Mvvm;
using Axis2.WPF.Models;
using System;

namespace Axis2.WPF.ViewModels
{
    public class LogSourceFilterViewModel : ObservableObject
    {
        private bool _isSelected;

        public LogSource Source { get; }
        public string Name => Source.ToString();

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty(ref _isSelected, value))
                {
                    OnIsSelectedChanged?.Invoke();
                }
            }
        }

        public event Action OnIsSelectedChanged;

        public LogSourceFilterViewModel(LogSource source, bool isSelected = true)
        {
            Source = source;
            _isSelected = isSelected;
        }
    }
}
