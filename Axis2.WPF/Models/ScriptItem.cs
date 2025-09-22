using Axis2.WPF.Mvvm;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace Axis2.WPF.Models
{
    public class ScriptItem : BindableBase
    {
        private string _name;
        private string _path;
        private bool _isFolder;
        private bool? _isSelected;
        private ObservableCollection<ScriptItem> _children;
        private ScriptItem _parent;

        // Flag to prevent re-entrant updates
        private bool _suppressUpdate = false;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Path
        {
            get => _path;
            set => SetProperty(ref _path, value);
        }

        public bool IsFolder
        {
            get => _isFolder;
            set => SetProperty(ref _isFolder, value);
        }

        public bool? IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value || _suppressUpdate) return; // Prevent re-entrant updates

                _suppressUpdate = true; // Start suppressing updates
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));

                // Propagate change to children
                if (IsFolder && _isSelected.HasValue)
                {
                    foreach (var child in Children)
                    {
                        child.IsSelected = _isSelected; // This will call the setter on children
                    }
                }

                _suppressUpdate = false; // Stop suppressing updates

                // Propagate change to parent
                if (Parent != null)
                {
                    Parent.UpdateCheckState();
                }
            }
        }

        public ObservableCollection<ScriptItem> Children
        {
            get => _children;
            set => SetProperty(ref _children, value);
        }

        [JsonIgnore]
        public ScriptItem Parent
        {
            get => _parent;
            set => SetProperty(ref _parent, value);
        }

        public ScriptItem()
        {
            _name = string.Empty;
            _path = string.Empty;
            _children = new ObservableCollection<ScriptItem>();
            _children.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (ScriptItem item in e.NewItems)
                    {
                        item.Parent = this;
                        item.PropertyChanged += Child_PropertyChanged;
                    }

                if (e.OldItems != null)
                    foreach (ScriptItem item in e.OldItems)
                    {
                        item.Parent = null;
                        item.PropertyChanged -= Child_PropertyChanged;
                    }
            };
        }

        private void Child_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsSelected))
            {
                // When a child's IsSelected changes, update this parent's state
                this.UpdateCheckState();
            }
        }

        public void UpdateCheckState()
        {
            if (_suppressUpdate) return; // Prevent re-entrant updates

            if (!Children.Any())
            {
                return;
            }

            bool? newState;
            if (Children.All(c => c.IsSelected == true))
            {
                newState = true;
            }
            else if (Children.All(c => c.IsSelected == false))
            {
                newState = false;
            }
            else
            {
                newState = null; // Indeterminate
            }

            if (newState != _isSelected)
            {
                _suppressUpdate = true; // Start suppressing updates
                _isSelected = newState;
                OnPropertyChanged(nameof(IsSelected));
                _suppressUpdate = false; // Stop suppressing updates

                if (Parent != null)
                    Parent.UpdateCheckState();
            }
        }
    }
}