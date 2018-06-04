using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Captura.ViewModels
{
    public abstract class CustomOverlaysBaseViewModel<T> where T : new()
    {
        public CustomOverlaysBaseViewModel(ObservableCollection<T> Collection)
        {
            _collection = Collection;

            this.Collection = new ReadOnlyObservableCollection<T>(_collection);

            AddCommand = new DelegateCommand(() => _collection.Add(new T()));

            RemoveCommand = new DelegateCommand(OnRemoveExecute);
        }

        void OnRemoveExecute(object O)
        {
            if (O is T setting)
            {
                _collection.Remove(setting);
            }
        }

        readonly ObservableCollection<T> _collection;

        public ReadOnlyObservableCollection<T> Collection { get; }

        public ICommand AddCommand { get; }

        public ICommand RemoveCommand { get; }
    }
}