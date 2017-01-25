using System;
using Screna;
using Screna.Audio;
using ManagedBass;
using ManagedBass.Mix;
using System.Runtime.InteropServices;

namespace Captura
{
    class MixedAudioProvider : IAudioProvider
    {
        readonly int _silence, _loopback, _recording, _mixer;
        
        public MixedAudioProvider(int? RecordingDevice, int? LoopbackDevice)
        {
            if (RecordingDevice == null && LoopbackDevice == null)
                throw new InvalidOperationException("Nothing to Record.");

            _mixer = BassMix.CreateMixerStream(44100, 2, BassFlags.Float);
            
            if (RecordingDevice != null)
            {
                Bass.RecordInit(RecordingDevice.Value);
                Bass.CurrentRecordingDevice = RecordingDevice.Value;
                
                _recording = Bass.RecordStart(44100, 2, BassFlags.Float | BassFlags.RecordPause, null);

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

                _loopback = Bass.RecordStart(44100, 2, BassFlags.Float | BassFlags.RecordPause, null);

                BassMix.MixerAddChannel(_mixer, _loopback, BassFlags.MixerDownMix);
            }
            
            // mute the mixer
            Bass.ChannelSetAttribute(_mixer, ChannelAttribute.Volume, 0);

            Bass.ChannelSetDSP(_mixer, Procedure);
        }

        int FindPlaybackDevice(int LoopbackDevice)
        {
            var driver = Bass.RecordGetDeviceInfo(LoopbackDevice).Driver;

            DeviceInfo info;
            for (int i = 0; Bass.GetDeviceInfo(i, out info); ++i)
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
        
        public Screna.Audio.WaveFormat WaveFormat => Screna.Audio.WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);

        public event EventHandler<DataAvailableEventArgs> DataAvailable;
        public event EventHandler<EndEventArgs> RecordingStopped;

        public void Dispose()
        {
            Bass.StreamFree(_mixer);

            Bass.StreamFree(_recording);
            Bass.StreamFree(_loopback);

            Bass.StreamFree(_silence);
        }

        public void Start()
        {
            Bass.ChannelPlay(_silence);

            Bass.ChannelPlay(_recording);
            Bass.ChannelPlay(_loopback);

            Bass.ChannelPlay(_mixer);
        }

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
