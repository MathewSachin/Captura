using System.Collections.ObjectModel;
using System.Windows.Input;
using Reactive.Bindings;

namespace Captura.ViewModels
{
    public abstract class OverlayListViewModel<T> : NotifyPropertyChanged where T : class, new()
    {
        protected OverlayListViewModel(ObservableCollection<T> Collection)
        {
            _collection = Collection;

            this.Collection = new ReadOnlyObservableCollection<T>(_collection);

            AddCommand = new ReactiveCommand()
                .WithSubscribe(OnAddExecute);

            RemoveCommand = new ReactiveCommand()
                .WithSubscribe(OnRemoveExecute);

            if (Collection.Count > 0)
            {
                SelectedItem = Collection[0];
            }
        }

        void OnAddExecute()
        {
            var item = new T();

            _collection.Add(item);

            SelectedItem = item;
        }

        void OnRemoveExecute(object O)
        {
            if (O is T setting)
            {
                _collection.Remove(setting);
            }

            SelectedItem = _collection.Count > 0 ? _collection[0] : null;
        }

        readonly ObservableCollection<T> _collection;

        public ReadOnlyObservableCollection<T> Collection { get; }

        public ICommand AddCommand { get; }

        public ICommand RemoveCommand { get; }

        T _selectedItem;

        public T SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }
    }
}