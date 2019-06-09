using NAudio.CoreAudioApi;

namespace Captura.Audio
{
    class NAudioItem : IAudioItem
    {
        public virtual MMDevice Device { get; }

        public bool IsLoopback { get; }

        public string Name { get; }

        public NAudioItem(MMDevice Device, bool IsLoopback)
            : this(Device.FriendlyName, IsLoopback)
        {
            this.Device = Device;
        }

        protected NAudioItem(string Name, bool IsLoopback)
        {
            this.IsLoopback = IsLoopback;
            this.Name = Name;
        }

        public override string ToString() => Name;
    }
}