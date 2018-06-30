using System;
using System.Collections.Generic;
using System.Linq;
using ManagedBass;
using ManagedBass.Mix;
using System.Runtime.InteropServices;
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
        }

        readonly Dictionary<int, RecordingItem> _devices = new Dictionary<int, RecordingItem>();
        readonly int _filler; // Fill when no audio source is selected
        bool _fillerAdded;
        readonly int _mixer;
        readonly object _syncLock = new object();
        bool _running;

        /// <summary>
        /// Creates a new instance of <see cref="MixedAudioProvider"/>.
        /// </summary>
        public MixedAudioProvider(IEnumerable<BassItem> Devices, int FrameRate, bool MuteOutput = true)
        {
            if (Devices == null)
                throw new ArgumentNullException();

            var updatePeriod = 1000 / FrameRate;

            Bass.UpdatePeriod = updatePeriod.Clip(5, 100);

            for (var i = 0; i < BufferCount; ++i)
            {
                _buffers.Add(new byte[0]);
            }

            _mixer = BassMix.CreateMixerStream(44100, 2, BassFlags.Default);
            _filler = Bass.CreateStream(44100, 2, BassFlags.Float | BassFlags.Decode, ManagedBass.Extensions.SilenceStreamProcedure);

            foreach (var recordingDevice in Devices)
            {
                InitDevice(recordingDevice);
            }

            if (MuteOutput)
            {
                // mute the mixer
                Bass.ChannelSetAttribute(_mixer, ChannelAttribute.Volume, 0);
            }

            Bass.ChannelSetDSP(_mixer, Procedure);
        }
        
        void InitDevice(BassItem Device)
        {
            _devices.Add(Device.Id, new RecordingItem
            {
                DeviceId = Device.Id
            });

            void AddDevice()
            {
                lock (_syncLock)
                {
                    if (_devices[Device.Id].RecordingHandle != 0)
                        return;

                    Bass.RecordInit(Device.Id);
                    Bass.CurrentRecordingDevice = Device.Id;

                    var info = Bass.RecordingInfo;

                    var handle = Bass.RecordStart(info.Frequency, info.Channels, BassFlags.Float | BassFlags.RecordPause, null);

                    _devices[Device.Id].RecordingHandle = handle;

                    BassMix.MixerAddChannel(_mixer, handle, BassFlags.MixerDownMix);

                    if (_running)
                    {
                        Bass.ChannelPlay(handle);
                    }
                }
            }

            void RemoveDevice()
            {
                lock (_syncLock)
                {
                    if (_devices[Device.Id].RecordingHandle == 0)
                        return;

                    var handle = _devices[Device.Id].RecordingHandle;

                    BassMix.MixerRemoveChannel(handle);

                    Bass.StreamFree(handle);

                    _devices[Device.Id].RecordingHandle = 0;
                }
            }

            Device.PropertyChanged += (S, E) =>
            {
                if (E.PropertyName == nameof(Device.Active))
                {
                    if (Device.Active)
                        AddDevice();
                    else RemoveDevice();
                }
            };

            if (Device.Active)
                AddDevice();
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

            DataAvailable?.Invoke(this, new DataAvailableEventArgs(buffer, Length));
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

                foreach (var rec in _devices.Values.Where(M => M.RecordingHandle != 0))
                {
                    Bass.StreamFree(rec.RecordingHandle);
                }

                Bass.StreamFree(_filler);

                _running = false;
            }
        }

        /// <summary>
        /// Start Recording.
        /// </summary>
        public void Start()
        {
            lock (_syncLock)
            {
                foreach (var rec in _devices.Values.Where(M => M.RecordingHandle != 0))
                {
                    Bass.ChannelPlay(rec.RecordingHandle);
                }

                Bass.ChannelPlay(_mixer);

                if (!_fillerAdded)
                {
                    // Add Filler only after mixer has started
                    BassMix.MixerAddChannel(_mixer, _filler, BassFlags.Default);

                    _fillerAdded = true;
                }

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

                foreach (var rec in _devices.Values.Where(M => M.RecordingHandle != 0))
                {
                    Bass.ChannelPause(rec.RecordingHandle);
                }

                _running = false;
            }
        }
    }
}
