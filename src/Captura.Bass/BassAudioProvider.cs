using System;
using ManagedBass;
using System.Runtime.InteropServices;
using Wf = Captura.Audio.WaveFormat;
using Captura.Audio;

namespace Captura.Models
{
    class BassAudioProvider : IAudioProvider
    {
        readonly int _recordingHandle;

        readonly int _silenceHandle;
 
        readonly object _syncLock = new object();

        public BassAudioProvider(BassItem Device)
        {
            if (Device == null)
                throw new ArgumentNullException();

            Bass.RecordInit(Device.Id);

            var devInfo = Bass.RecordGetDeviceInfo(Device.Id);

            if (devInfo.IsLoopback)
            {
                var playbackDevice = FindPlaybackDevice(devInfo);

                if (playbackDevice != -1)
                {
                    Bass.Init(playbackDevice);
                    Bass.CurrentDevice = playbackDevice;

                    _silenceHandle = Bass.CreateStream(44100, 2, BassFlags.Default, Extensions.SilenceStreamProcedure);

                    Bass.ChannelSetAttribute(_silenceHandle, ChannelAttribute.Volume, 0);
                }
            }

            Bass.CurrentRecordingDevice = Device.Id;

            var info = Bass.RecordingInfo;

            _recordingHandle = Bass.RecordStart(info.Frequency, info.Channels, BassFlags.RecordPause, RecordProcedure);
        }

        bool RecordProcedure(int Handle, IntPtr Ptr, int Length, IntPtr User)
        {
            var buffer = GetBuffer(Length);

            Marshal.Copy(Ptr, buffer, 0, Length);

            DataAvailable?.Invoke(this, new DataAvailableEventArgs(buffer, Length));

            return true;
        }

        static int FindPlaybackDevice(DeviceInfo LoopbackDeviceInfo)
        {
            for (var i = 0; Bass.GetDeviceInfo(i, out var info); ++i)
            {
                if (info.Driver == LoopbackDeviceInfo.Driver)
                    return i;
            }

            return -1;
        }

        byte[] _buffer;

        byte[] GetBuffer(int Length)
        {
            if (_buffer == null || _buffer.Length < Length)
            {
                _buffer = new byte[Length + 1000];

                Console.WriteLine($"New Audio Buffer Allocated: {Length}");
            }

            return _buffer;
        }
        
        /// <summary>
        /// Gets the WaveFormat of this <see cref="IAudioProvider"/>.
        /// </summary>
        public Wf WaveFormat => new Wf(44100, 16, 2);

        /// <summary>
        /// Indicates recorded data is available.
        /// </summary>
        public event EventHandler<DataAvailableEventArgs> DataAvailable;
        
        /// <summary>
        /// Frees up the resources used by this instant.
        /// </summary>
        public void Dispose()
        {
            lock (_syncLock)
            {
                Bass.StreamFree(_recordingHandle);

                if (_silenceHandle != 0)
                    Bass.StreamFree(_silenceHandle);

                _buffer = null;
            }
        }

        /// <summary>
        /// Start Recording.
        /// </summary>
        public void Start()
        {
            lock (_syncLock)
            {
                if (_silenceHandle != 0)
                    Bass.ChannelPlay(_silenceHandle);

                Bass.ChannelPlay(_recordingHandle);
            }
        }

        /// <summary>
        /// Stop Recording.
        /// </summary>
        public void Stop()
        {
            lock (_syncLock)
            {
                Bass.ChannelPause(_recordingHandle);

                if (_silenceHandle != 0)
                    Bass.ChannelPause(_silenceHandle);
            }
        }
    }
}
