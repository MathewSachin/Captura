namespace Captura.Audio
{
    class BassItem : IAudioItem
    {
        public virtual int Id { get; }

        public bool IsLoopback { get; }

        public BassItem(int Id, string Name, bool IsLoopback)
            : this(Name, IsLoopback)
        {
            this.Id = Id;
        }

        protected BassItem(string Name, bool IsLoopback)
        {
            this.Name = Name;
            this.IsLoopback = IsLoopback;
        }

        public string Name { get; }

        public override string ToString() => Name;
    }
}