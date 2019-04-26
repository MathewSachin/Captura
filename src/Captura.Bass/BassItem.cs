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

        public override string ToString() => Name;
    }
}