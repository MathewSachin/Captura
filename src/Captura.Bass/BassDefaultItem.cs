using ManagedBass;

namespace Captura.Audio
{
    class BassDefaultItem : BassItem
    {
        BassDefaultItem(bool IsLoopback) : base("Default", IsLoopback) { }

        public override int Id
        {
            get
            {
                for (var i = 0; Bass.RecordGetDeviceInfo(i, out var info); ++i)
                {
                    if (info.IsLoopback == IsLoopback && info.IsDefault)
                        return i;
                }

                return -1;
            }
        }

        public static BassItem DefaultSpeaker { get; } = new BassDefaultItem(true);

        public static BassItem DefaultMicrophone { get; } = new BassDefaultItem(false);
    }
}