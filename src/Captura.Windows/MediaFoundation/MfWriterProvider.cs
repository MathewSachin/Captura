using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Captura.Video;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace Captura.Windows.MediaFoundation
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MfWriterProvider : IVideoWriterProvider
    {
        readonly Device _device;

        public MfWriterProvider()
        {
            _device = new Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport);
        }

        public string Name => "MF";

        public IEnumerator<IVideoWriterItem> GetEnumerator()
        {
            yield return new MfItem(_device);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => Name;

        public IVideoWriterItem ParseCli(string Cli)
        {
            return Cli == "mf" ? this.First() : null;
        }

        public string Description => "Encode to Mp4: H.264 with AAC audio using Media Foundation Hardware encoder";
    }
}