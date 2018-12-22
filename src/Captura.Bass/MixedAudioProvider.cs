using System;
using System.Collections.Generic;
using ManagedBass;
using ManagedBass.Mix;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Wf = Captura.Audio.WaveFormat;
using Captura.Audio;

namespace Captura.Models
{
    /// <summary>
    /// Provides mixed audio from Microphone input and Speaker Output (Wasapi Loopback).
    /// Requires the presence of bass.dll and bassmix.dll.
    /// </summary>
    class MixedAudioProvider : IAudioProvider
    {
        class RecordingItem
        {
            public int DeviceId { get; set; }

            public int RecordingHandle { get; set; }

            public int SilenceHandle { get; set; }
        }

        readonly Dictionary<int, RecordingItem> _devices = new Dictionary<int, RecordingItem>();
        readonly int _mixer;
        readonly object _syncLock = new object();
        bool _running;

        /// <summary>
        /// Creates a new instance of <see cref="MixedAudioProvider"/>.
        /// </summary>
        public MixedAudioProvider(IEnumerable<BassItem> Devices)
        {
            if (Devices == null)
                throw new ArgumentNullException();

            for (var i = 0; i < BufferCount; ++i)
            {
                _buffers.Add(new byte[0]);
            }

            _mixer = BassMix.CreateMixerStream(44100, 2, BassFlags.MixerNonStop);

            foreach (var recordingDevice in Devices)
            {
                InitDevice(recordingDevice);
            }

            // mute the mixer
            Bass.ChannelSetAttribute(_mixer, ChannelAttribute.Volume, 0);

            Bass.ChannelSetDSP(_mixer, Procedure);
        }
        
        void InitDevice(BassItem Device)
        {
            _devices.Add(Device.Id, new RecordingItem
            {
                DeviceId = Device.Id
            });

            Device.PropertyChanged += (S, E) =>
            {
                if (E.PropertyName == nameof(Device.Active))
                {
                    if (Device.Active)
                        AddDevice(Device);
                    else RemoveDevice(Device);
                }
            };

            if (Device.Active)
                AddDevice(Device);
        }

        void RemoveDevice(BassItem Device)
        {
            lock (_syncLock)
            {
                if (_devices[Device.Id].RecordingHandle == 0)
                    return;

                var handle = _devices[Device.Id].RecordingHandle;

                BassMix.MixerRemoveChannel(handle);

                Bass.StreamFree(handle);

                _devices[Device.Id].RecordingHandle = 0;

                Bass.StreamFree(_devices[Device.Id].SilenceHandle);

                _devices[Device.Id].SilenceHandle = 0;
            }
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

        void AddDevice(BassItem Device)
        {
            lock (_syncLock)
            {
                if (_devices[Device.Id].RecordingHandle != 0)
                    return;

                Bass.RecordInit(Device.Id);

                var devInfo = Bass.RecordGetDeviceInfo(Device.Id);

                if (devInfo.IsLoopback)
                {
                    var playbackDevice = FindPlaybackDevice(devInfo);

                    if (playbackDevice != -1)
                    {
                        Bass.Init(playbackDevice);
                        Bass.CurrentDevice = playbackDevice;

                        var silence = Bass.CreateStream(44100, 2, BassFlags.Default, Extensions.SilenceStreamProcedure);

                        Bass.ChannelSetAttribute(silence, ChannelAttribute.Volume, 0);

                        _devices[Device.Id].SilenceHandle = silence;
                    }
                }

                Bass.CurrentRecordingDevice = Device.Id;

                var info = Bass.RecordingInfo;

                var handle = Bass.RecordStart(info.Frequency, info.Channels, BassFlags.RecordPause, null);

                _devices[Device.Id].RecordingHandle = handle;

                BassMix.MixerAddChannel(_mixer, handle, BassFlags.MixerDownMix | BassFlags.MixerLimit);

                if (_running)
                {
                    Bass.ChannelPlay(handle);
                }
            }
        }

        const int BufferCount = 3;

        int _bufferIndex;

        readonly List<byte[]> _buffers = new List<byte[]>();

        byte[] GetBuffer(int Length)
        {
            _bufferIndex = ++_bufferIndex % BufferCount;

            if (_buffers[_bufferIndex] == null || _buffers[_bufferIndex].Length < Length)
            {
                _buffers[_bufferIndex] = new byte[Length + 1000];

                Console.WriteLine($"New Audio Buffer Allocated: {Length}");
            }

            return _buffers[_bufferIndex];
        }

        void Procedure(int Handle, int Channel, IntPtr Buffer, int Length, IntPtr User)
        {
            var buffer = GetBuffer(Length);

            Marshal.Copy(Buffer, buffer, 0, Length);

            Task.Run(() => DataAvailable?.Invoke(this, new DataAvailableEventArgs(buffer, Length)));
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
                Bass.StreamFree(_mixer);

                foreach (var rec in _devices.Values)
                {
                    if (rec.RecordingHandle != 0)
                        Bass.StreamFree(rec.RecordingHandle);

                    if (rec.SilenceHandle != 0)
                        Bass.StreamFree(rec.SilenceHandle);
                }

                _running = false;

                _buffers.Clear();
            }
        }

        /// <summary>
        /// Start Recording.
        /// </summary>
        public void Start()
        {
            lock (_syncLock)
            {
                foreach (var rec in _devices.Values)
                {
                    if (rec.SilenceHandle != 0)
                        Bass.ChannelPlay(rec.SilenceHandle);

                    if (rec.RecordingHandle != 0)
                        Bass.ChannelPlay(rec.RecordingHandle);
                }

                Bass.ChannelPlay(_mixer);

                _running = true;
            }
        }

        /// <summary>
        /// Stop Recording.
        /// </summary>
        public void Stop()
        {
            lock (_syncLock)
            {
                Bass.ChannelPause(_mixer);

                foreach (var rec in _devices.Values)
                {
                    if (rec.RecordingHandle != 0)
                        Bass.ChannelPause(rec.RecordingHandle);

                    if (rec.SilenceHandle != 0)
                        Bass.ChannelPause(rec.SilenceHandle);
                }

                _running = false;
            }
        }
    }
}
