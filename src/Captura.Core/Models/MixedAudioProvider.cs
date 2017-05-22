using System;
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
        readonly int _silence, _loopback, _recording, _mixer;
        
        /// <summary>
        /// Creates a new instance of <see cref="MixedAudioProvider"/>.
        /// </summary>
        /// <param name="RecordingDevice">Index of Recording Device. null = No Microphone Recording.</param>
        /// <param name="LoopbackDevice">Index of Loopback Device. null = No Loopback Recording.</param>
        /// <exception cref="InvalidOperationException">Can't Record when both <paramref name="LoopbackDevice"/> and <paramref name="RecordingDevice"/> are null.</exception>
        public MixedAudioProvider(int? RecordingDevice, int? LoopbackDevice)
        {
            if (RecordingDevice == null && LoopbackDevice == null)
                throw new InvalidOperationException("Nothing to Record.");

            _mixer = BassMix.CreateMixerStream(44100, 2, BassFlags.Default);
            
            if (RecordingDevice != null)
            {
                Bass.RecordInit(RecordingDevice.Value);
                Bass.CurrentRecordingDevice = RecordingDevice.Value;

                var info = Bass.RecordingInfo;
                
                _recording = Bass.RecordStart(info.Frequency, info.Channels, BassFlags.Float | BassFlags.RecordPause, null);

                BassMix.MixerAddChannel(_mixer, _recording, BassFlags.MixerDownMix);
            }

            if (LoopbackDevice != null)
            {
                var playbackDevice = FindPlaybackDevice(LoopbackDevice.Value);

                Bass.Init(playbackDevice);
                Bass.CurrentDevice = playbackDevice;

                _silence = Bass.CreateStream(44100, 2, BassFlags.Float, ManagedBass.Extensions.SilenceStreamProcedure);

                Bass.RecordInit(LoopbackDevice.Value);
                Bass.CurrentRecordingDevice = LoopbackDevice.Value;

                var info = Bass.RecordingInfo;
                
                _loopback = Bass.RecordStart(info.Frequency, info.Channels, BassFlags.Float | BassFlags.RecordPause, null);

                BassMix.MixerAddChannel(_mixer, _loopback, BassFlags.MixerDownMix);
            }
            
            // mute the mixer
            Bass.ChannelSetAttribute(_mixer, ChannelAttribute.Volume, 0);

            Bass.ChannelSetDSP(_mixer, Procedure);
        }

        int FindPlaybackDevice(int LoopbackDevice)
        {
            var driver = Bass.RecordGetDeviceInfo(LoopbackDevice).Driver;

            for (int i = 0; Bass.GetDeviceInfo(i, out var info); ++i)
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
        /// Indicates that all recorded data has now been received.
        /// </summary>
        public event EventHandler<EndEventArgs> RecordingStopped;

        /// <summary>
        /// Frees up the resources used by this instant.
        /// </summary>
        public void Dispose()
        {
            Bass.StreamFree(_mixer);

            Bass.StreamFree(_recording);
            Bass.StreamFree(_loopback);

            Bass.StreamFree(_silence);
        }

        /// <summary>
        /// Start Recording.
        /// </summary>
        public void Start()
        {
            Bass.ChannelPlay(_silence);

            Bass.ChannelPlay(_recording);
            Bass.ChannelPlay(_loopback);

            Bass.ChannelPlay(_mixer);
        }

        /// <summary>
        /// Stop Recording.
        /// </summary>
        public void Stop()
        {
            Bass.ChannelPause(_mixer);

            Bass.ChannelPause(_recording);
            Bass.ChannelPause(_loopback);

            Bass.ChannelPause(_silence);

            RecordingStopped?.Invoke(this, new EndEventArgs(null));
        }
    }
}
