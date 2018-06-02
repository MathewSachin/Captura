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
        readonly List<int> _silence = new List<int>();
        readonly List<int> _loopback = new List<int>();
        readonly List<int> _recording = new List<int>();
        readonly int _mixer;

        /// <summary>
        /// Creates a new instance of <see cref="MixedAudioProvider"/>.
        /// </summary>
        /// <param name="RecordingDevices">Indices of Recording Devices.</param>
        /// <param name="LoopbackDevices">Indices of Loopback Devices.</param>
        public MixedAudioProvider(IReadOnlyCollection<int> RecordingDevices, IReadOnlyCollection<int> LoopbackDevices)
        {
            if (RecordingDevices == null || LoopbackDevices == null)
                throw new ArgumentNullException();

            if (RecordingDevices.Count + LoopbackDevices.Count == 0)
                throw new InvalidOperationException("Nothing to Record.");

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

        void InitRecordingDevice(int RecordingDevice)
        {
            Bass.RecordInit(RecordingDevice);
            Bass.CurrentRecordingDevice = RecordingDevice;

            var info = Bass.RecordingInfo;

            var handle = Bass.RecordStart(info.Frequency, info.Channels, BassFlags.Float | BassFlags.RecordPause, null);

            _recording.Add(handle);

            BassMix.MixerAddChannel(_mixer, handle, BassFlags.MixerDownMix);
        }

        void InitLoopbackDevice(int LoopbackDevice)
        {
            var playbackDevice = FindPlaybackDevice(LoopbackDevice);

            Bass.Init(playbackDevice);
            Bass.CurrentDevice = playbackDevice;

            _silence.Add(Bass.CreateStream(44100, 2, BassFlags.Float, ManagedBass.Extensions.SilenceStreamProcedure));

            Bass.RecordInit(LoopbackDevice);
            Bass.CurrentRecordingDevice = LoopbackDevice;

            var info = Bass.RecordingInfo;

            var handle = Bass.RecordStart(info.Frequency, info.Channels, BassFlags.Float | BassFlags.RecordPause, null);

            _loopback.Add(handle);

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

            _recording.ForEach(M => Bass.StreamFree(M));
            _loopback.ForEach(M => Bass.StreamFree(M));

            _silence.ForEach(M => Bass.StreamFree(M));
        }

        /// <summary>
        /// Start Recording.
        /// </summary>
        public void Start()
        {
            _silence.ForEach(M => Bass.ChannelPlay(M));

            _recording.ForEach(M => Bass.ChannelPlay(M));
            _loopback.ForEach(M => Bass.ChannelPlay(M));

            Bass.ChannelPlay(_mixer);
        }

        /// <summary>
        /// Stop Recording.
        /// </summary>
        public void Stop()
        {
            Bass.ChannelPause(_mixer);

            _recording.ForEach(M => Bass.ChannelPause(M));
            _loopback.ForEach(M => Bass.ChannelPause(M));

            _silence.ForEach(M => Bass.ChannelPause(M));
        }
    }
}
