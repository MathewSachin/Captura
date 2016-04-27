using System.Collections.Generic;
using NAudio.Wave;

namespace Screna.NAudio
{
    public class WaveInDevice
    {
        public int DeviceNumber { get; }

        public WaveInDevice(int DeviceNumber)
        {
            this.DeviceNumber = DeviceNumber;
        }

        public string Name => WaveIn.GetCapabilities(DeviceNumber).ProductName;

        public static int DeviceCount => WaveIn.DeviceCount;

        public bool SupportsWaveFormat(SupportedWaveFormat WaveFormat) => WaveIn.GetCapabilities(DeviceNumber).SupportsWaveFormat(WaveFormat);

        public static IEnumerable<WaveInDevice> Enumerate()
        {
            var n = DeviceCount;

            for (var i = 0; i < n; ++i)
                yield return new WaveInDevice(i);
        } 

        public static WaveInDevice DefaultDevice => new WaveInDevice(0);
        
        public override string ToString() => Name;
    }
}
