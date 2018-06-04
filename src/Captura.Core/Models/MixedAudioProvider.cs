using System;
using System.Collections.Generic;
using System.Linq;
using ManagedBass;
using ManagedBass.Mix;
using System.Runtime.InteropServices;
using Wf = Screna.Audio.WaveFormat;
using Screna.Audio;
using Screna;

namespace Captura.Models
{
    /// <summary>
    /// Provides mixed audio from Microphone input and Speaker Output (Wasapi Loopback).
    /// Requires the presence of bass.dll and bassmix.dll.
    /// </summary>
    public class MixedAudioProvider : IAudioProvider
    {
        class RecordingItem
        {
            public int DeviceId { get; set; }

            public int RecordingHandle { get; set; }
        }

        class LoopbackItem : RecordingItem
        {
            public int PlaybackDeviceId { get; set; }

            public int SilenceStreamHandle { get; set; }
        }

        readonly Dictionary<int, LoopbackItem> _loopback = new Dictionary<int, LoopbackItem>();
        readonly Dictionary<int, RecordingItem> _recording = new Dictionary<int, RecordingItem>();
        readonly int _filler; // Fill when no audio source is selected
        bool _fillerAdded;
        readonly int _mixer;
        readonly object _syncLock = new object();
        bool _running;

        /// <summary>
        /// Creates a new instance of <see cref="MixedAudioProvider"/>.
        /// </summary>
        public MixedAudioProvider(IEnumerable<BassItem> RecordingDevices, IEnumerable<BassItem> LoopbackDevices)
        {
            if (RecordingDevices == null || LoopbackDevices == null)
                throw new ArgumentNullException();

            _mixer = BassMix.CreateMixerStream(44100, 2, BassFlags.Default);
            _filler = Bass.CreateStream(44100, 2, BassFlags.Float | BassFlags.Decode, ManagedBass.Extensions.SilenceStreamProcedure);

            foreach (var recordingDevice in RecordingDevices)
            {
                InitRecordingDevice(recordingDevice);
            }

            foreach (var loopbackDevice in LoopbackDevices)
            {
                InitLoopbackDevice(loopbackDevice);
            }
            
            // mute the mixer
            Bass.ChannelSetAttribute(_mixer, ChannelAttribute.Volume, 0);

            Bass.ChannelSetDSP(_mixer, Procedure);
        }
        
        void InitRecordingDevice(BassItem RecordingDevice)
        {
            _recording.Add(RecordingDevice.Id, new RecordingItem
            {
                DeviceId = RecordingDevice.Id
            });

            void AddDevice()
            {
                lock (_syncLock)
                {
                    if (_recording[RecordingDevice.Id].RecordingHandle != 0)
                        return;

                    Bass.RecordInit(RecordingDevice.Id);
                    Bass.CurrentRecordingDevice = RecordingDevice.Id;

                    var info = Bass.RecordingInfo;

                    var handle = Bass.RecordStart(info.Frequency, info.Channels, BassFlags.Float | BassFlags.RecordPause, null);

                    _recording[RecordingDevice.Id].RecordingHandle = handle;

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
                    if (_recording[RecordingDevice.Id].RecordingHandle == 0)
                        return;

                    var handle = _recording[RecordingDevice.Id].RecordingHandle;

                    BassMix.MixerRemoveChannel(handle);

                    Bass.StreamFree(handle);

                    _recording[RecordingDevice.Id].RecordingHandle = 0;
                }
            }

            RecordingDevice.PropertyChanged += (S, E) =>
            {
                if (E.PropertyName == nameof(RecordingDevice.Active))
                {
                    if (RecordingDevice.Active)
                        AddDevice();
                    else RemoveDevice();
                }
            };

            if (RecordingDevice.Active)
                AddDevice();
        }

        void InitLoopbackDevice(BassItem LoopbackDevice)
        {
            var playbackDevice = FindPlaybackDevice(LoopbackDevice.Id);

            _loopback.Add(LoopbackDevice.Id, new LoopbackItem
            {
                DeviceId = LoopbackDevice.Id,
                PlaybackDeviceId = playbackDevice
            });

            void AddDevice()
            {
                lock (_syncLock)
                {
                    if (_loopback[LoopbackDevice.Id].RecordingHandle != 0)
                        return;

                    Bass.Init(playbackDevice);
                    Bass.CurrentDevice = playbackDevice;

                    var silence = Bass.CreateStream(44100, 2, BassFlags.Float, ManagedBass.Extensions.SilenceStreamProcedure);

                    _loopback[LoopbackDevice.Id].SilenceStreamHandle = silence;

                    Bass.RecordInit(LoopbackDevice.Id);
                    Bass.CurrentRecordingDevice = LoopbackDevice.Id;

                    var info = Bass.RecordingInfo;

                    var handle = Bass.RecordStart(info.Frequency, info.Channels, BassFlags.Float | BassFlags.RecordPause, null);

                    _loopback[LoopbackDevice.Id].RecordingHandle = handle;

                    BassMix.MixerAddChannel(_mixer, handle, BassFlags.MixerDownMix);

                    if (_running)
                    {
                        Bass.ChannelPlay(silence);
                        Bass.ChannelPlay(handle);
                    }
                }
            }

            void RemoveDevice()
            {
                lock (_syncLock)
                {
                    if (_loopback[LoopbackDevice.Id].RecordingHandle == 0)
                        return;

                    var item = _loopback[LoopbackDevice.Id];

                    BassMix.MixerRemoveChannel(item.RecordingHandle);

                    Bass.StreamFree(item.RecordingHandle);

                    Bass.StreamFree(item.SilenceStreamHandle);

                    _loopback[LoopbackDevice.Id].RecordingHandle = 0;
                    _loopback[LoopbackDevice.Id].SilenceStreamHandle = 0;
                }
            }

            LoopbackDevice.PropertyChanged += (S, E) =>
            {
                if (E.PropertyName == nameof(LoopbackDevice.Active))
                {
                    if (LoopbackDevice.Active)
                        AddDevice();
                    else RemoveDevice();
                }
            };

            if (LoopbackDevice.Active)
                AddDevice();
        }

        static int FindPlaybackDevice(int LoopbackDevice)
        {
            var driver = Bass.RecordGetDeviceInfo(LoopbackDevice).Driver;

            for (var i = 0; Bass.GetDeviceInfo(i, out var info); ++i)
                if (info.Driver == driver)
                    return i;

            return 0;
        }

        byte[] _buffer;

        void Procedure(int Handle, int Channel, IntPtr Buffer, int Length, IntPtr User)
        {
            if (_buffer == null || _buffer.Length < Length)
                _buffer = new byte[Length];

            Marshal.Copy(Buffer, _buffer, 0, Length);

            DataAvailable?.Invoke(this, new DataAvailableEventArgs(_buffer, Length));
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

                foreach (var rec in _recording.Values.Where(M => M.RecordingHandle != 0))
                {
                    Bass.StreamFree(rec.RecordingHandle);
                }

                foreach (var loop in _loopback.Values.Where(M => M.RecordingHandle != 0))
                {
                    Bass.StreamFree(loop.RecordingHandle);
                    Bass.StreamFree(loop.SilenceStreamHandle);
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
                foreach (var loop in _loopback.Values.Where(M => M.RecordingHandle != 0))
                {
                    Bass.ChannelPlay(loop.SilenceStreamHandle);
                    Bass.ChannelPlay(loop.RecordingHandle);
                }

                foreach (var rec in _recording.Values.Where(M => M.RecordingHandle != 0))
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

                foreach (var rec in _recording.Values.Where(M => M.RecordingHandle != 0))
                {
                    Bass.ChannelPause(rec.RecordingHandle);
                }

                foreach (var loop in _loopback.Values.Where(M => M.RecordingHandle != 0))
                {
                    Bass.ChannelPause(loop.RecordingHandle);
                    Bass.ChannelPause(loop.SilenceStreamHandle);
                }

                _running = false;
            }
        }
    }
}
