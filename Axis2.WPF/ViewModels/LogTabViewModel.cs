using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using Axis2.WPF.Models;
using Axis2.WPF.Mvvm;
using Axis2.WPF.Services;

namespace Axis2.WPF.ViewModels
{
    public class LogTabViewModel : ViewModelBase
    {
        private const int MaxLogMessages = 5000;
        private readonly ObservableCollection<LogEntry> _allLogMessages;
        private string _searchText;

        public ICollectionView LogMessagesView { get; }
        public ObservableCollection<LogSourceFilterViewModel> LogSources { get; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    LogMessagesView.Refresh();
                }
            }
        }

        public ICommand CopyCommand { get; }

        public LogTabViewModel()
        {
            _allLogMessages = new ObservableCollection<LogEntry>();
            LogMessagesView = CollectionViewSource.GetDefaultView(_allLogMessages);
            LogMessagesView.Filter = FilterLogs;

            LogSources = new ObservableCollection<LogSourceFilterViewModel>(
                Enum.GetValues(typeof(LogSource))
                    .Cast<LogSource>()
                    .Select(s => new LogSourceFilterViewModel(s))
            );

            foreach (var sourceFilter in LogSources)
            {
                sourceFilter.OnIsSelectedChanged += () => LogMessagesView.Refresh();
            }

            CopyCommand = new RelayCommand<IList>(ExecuteCopy, CanExecuteCopy);

            Logger.OnLogMessage += OnLogMessageReceived;
        }

        private void OnLogMessageReceived(LogEntry logEntry)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (_allLogMessages.Count >= MaxLogMessages)
                {
                    _allLogMessages.RemoveAt(0);
                }
                _allLogMessages.Add(logEntry);
            });
        }

        private bool FilterLogs(object item)
        {
            if (!(item is LogEntry logEntry)) return false;

            var selectedSource = LogSources.FirstOrDefault(s => s.Source == logEntry.Source);
            if (selectedSource == null || !selectedSource.IsSelected)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                return logEntry.Message.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
            }

            return true;
        }

        private bool CanExecuteCopy(IList selectedItems)
        {
            return selectedItems != null && selectedItems.Count > 0;
        }

        private void ExecuteCopy(IList selectedItems)
        {
            var sb = new StringBuilder();
            foreach (var item in selectedItems.OfType<LogEntry>())
            {
                sb.AppendLine(item.FormattedMessage);
            }
            System.Windows.Clipboard.SetText(sb.ToString());
        }
    }
}