namespace Captura.Models
{
    public static class IsActiveExtensions
    {
        class IsActiveWrapper<T> : NotifyPropertyChanged, IIsActive<T>
        {
            public IsActiveWrapper(T Item, bool IsActive)
            {
                this.Item = Item;

                this.IsActive = IsActive;
            }

            public T Item { get; }

            bool _isActive;

            public bool IsActive
            {
                get => _isActive;
                set => Set(ref _isActive, value);
            }
        }

        public static IIsActive<T> ToIsActive<T>(this T Item, bool IsActive = false) => new IsActiveWrapper<T>(Item, IsActive);
    }
}
