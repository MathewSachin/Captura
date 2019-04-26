namespace Captura.Models
{
    class BassItem : NotifyPropertyChanged, IAudioItem
    {
        public int Id { get; }

        public bool IsLoopback { get; }

        public BassItem(int Id, string Name, bool IsLoopback)
        {
            this.Id = Id;
            this.Name = Name;
            this.IsLoopback = IsLoopback;
        }

        public string Name { get; }

        bool _active;

        public bool Active
        {
            get => _active;
            set => Set(ref _active, value);
        }

        public override string ToString() => Name;
    }
}