using System;
using System.Collections.Generic;
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

        readonly List<LoopbackItem> _loopback = new List<LoopbackItem>();
        readonly List<RecordingItem> _recording = new List<RecordingItem>();
        readonly int _mixer;

        /// <summary>
        /// Creates a new instance of <see cref="MixedAudioProvider"/>.
        /// </summary>
        public MixedAudioProvider(IEnumerable<BassItem> RecordingDevices, IEnumerable<BassItem> LoopbackDevices)
        {
            if (RecordingDevices == null || LoopbackDevices == null)
                throw new ArgumentNullException();

            _mixer = BassMix.CreateMixerStream(44100, 2, BassFlags.Default);

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
            if (!RecordingDevice.Active)
                return;

            Bass.RecordInit(RecordingDevice.Id);
            Bass.CurrentRecordingDevice = RecordingDevice.Id;

            var info = Bass.RecordingInfo;

            var handle = Bass.RecordStart(info.Frequency, info.Channels, BassFlags.Float | BassFlags.RecordPause, null);
            
            _recording.Add(new RecordingItem
            {
                DeviceId = RecordingDevice.Id,
                RecordingHandle = handle
            });

            BassMix.MixerAddChannel(_mixer, handle, BassFlags.MixerDownMix);
        }

        void InitLoopbackDevice(BassItem LoopbackDevice)
        {
            if (!LoopbackDevice.Active)
                return;

            var playbackDevice = FindPlaybackDevice(LoopbackDevice.Id);

            Bass.Init(playbackDevice);
            Bass.CurrentDevice = playbackDevice;

            var silence = Bass.CreateStream(44100, 2, BassFlags.Float, ManagedBass.Extensions.SilenceStreamProcedure);

            Bass.RecordInit(LoopbackDevice.Id);
            Bass.CurrentRecordingDevice = LoopbackDevice.Id;

            var info = Bass.RecordingInfo;

            var handle = Bass.RecordStart(info.Frequency, info.Channels, BassFlags.Float | BassFlags.RecordPause, null);

            _loopback.Add(new LoopbackItem
            {
                DeviceId = LoopbackDevice.Id,
                RecordingHandle = handle,
                PlaybackDeviceId = playbackDevice,
                SilenceStreamHandle = silence
            });

            BassMix.MixerAddChannel(_mixer, handle, BassFlags.MixerDownMix);
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
            Bass.StreamFree(_mixer);

            _recording.ForEach(M =>
            {
                Bass.StreamFree(M.RecordingHandle);
            });

            _loopback.ForEach(M =>
            {
                Bass.StreamFree(M.RecordingHandle);
                Bass.StreamFree(M.SilenceStreamHandle);
            });
        }

        /// <summary>
        /// Start Recording.
        /// </summary>
        public void Start()
        {
            _loopback.ForEach(M =>
            {
                Bass.ChannelPlay(M.SilenceStreamHandle);
                Bass.ChannelPlay(M.RecordingHandle);
            });

            _recording.ForEach(M =>
            {
                Bass.ChannelPlay(M.RecordingHandle);
            });

            Bass.ChannelPlay(_mixer);
        }

        /// <summary>
        /// Stop Recording.
        /// </summary>
        public void Stop()
        {
            Bass.ChannelPause(_mixer);

            _recording.ForEach(M =>
            {
                Bass.ChannelPause(M.RecordingHandle);
            });

            _loopback.ForEach(M =>
            {
                Bass.ChannelPause(M.RecordingHandle);
                Bass.ChannelPause(M.SilenceStreamHandle);
            });
        }
    }
}
