namespace Captura.Models
{
    class BassItem : NotifyPropertyChanged, IAudioItem
    {
        public int Id { get; }

        public BassItem(int Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
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