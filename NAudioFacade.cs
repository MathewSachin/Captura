using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Collections.Generic;
using System;

namespace Captura
{
    class NAudioFacade : IDisposable
    {
        WaveFormat WaveFormat;
        IWaveIn AudioSource;
        WaveFileWriter WaveWriter;
        IWavePlayer SilencePlayer;
        public string AudioSourceId { get; private set; }

        public bool IsLoopback { get; private set; }

        public int AudioBitRate { get; private set; }

        public int Channels { get { return WaveFormat.Channels; } }

        public int SampleRate { get { return WaveFormat.SampleRate; } }

        public int BitsPerSample { get { return WaveFormat.BitsPerSample; } }

        public NAudioFacade(string AudioSourceId, bool Stereo, int EncodeBitRate = 160)
        {
            this.AudioSourceId = AudioSourceId;
            this.AudioBitRate = EncodeBitRate;

            int val;
            IsLoopback = !int.TryParse(AudioSourceId, out val);
            if (IsLoopback) LoopbackDevice = new MMDeviceEnumerator().GetDevice(AudioSourceId);

            WaveFormat = IsLoopback ? LoopbackDevice.AudioClient.MixFormat : new WaveFormat(44100, 16, Stereo ? 2 : 1);
        }

        MMDevice LoopbackDevice;

        public static IEnumerable<KeyValuePair<string, string>> EnumerateAudioDevices()
        {
            for (var i = 0; i < WaveInEvent.DeviceCount; i++)
                yield return new KeyValuePair<string, string>(i.ToString(), WaveInEvent.GetCapabilities(i).ProductName);

            foreach (var device in new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
                yield return new KeyValuePair<string, string>(device.ID, device.FriendlyName + " (Loopback)");
        }

        public void InitSilencePlayer()
        {
            SilencePlayer = new WasapiOut(LoopbackDevice, AudioClientShareMode.Shared, false, 100);

            SilencePlayer.Init(new SilenceProvider(WaveFormat));
        }

        public void PlaySilence()
        {
            if (SilencePlayer != null) SilencePlayer.Play();
        }

        public void PauseSilence()
        {
            if (SilencePlayer != null) SilencePlayer.Pause();
        }

        public void StartRecording() { AudioSource.StartRecording(); }

        public void StopRecording() { AudioSource.StopRecording(); }

        public void WaveWrite(byte[] data, int length) { WaveWriter.Write(data, 0, length); }

        public void InitLoopback()
        {
            AudioSource = new WasapiLoopbackCapture(LoopbackDevice) { ShareMode = AudioClientShareMode.Shared };
        }

        public void InitRecording(int FramesPerSecond)
        {
            int Id = int.Parse(AudioSourceId);

            AudioSource = new WaveInEvent
            {
                DeviceNumber = Id,
                WaveFormat = WaveFormat,
                // Buffer size to store duration of 1 frame
                BufferMilliseconds = (int)Math.Ceiling(1000 / (decimal)FramesPerSecond),
                NumberOfBuffers = 3,
            };

            AudioSource.DataAvailable += OnDataAvailable;
        }

        void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            if (DataAvailable != null)
                DataAvailable(e.Buffer, e.BytesRecorded);
        }

        public void CreateWaveWriter(string FileName)
        {
            WaveWriter = new WaveFileWriter(FileName, WaveFormat);
        }

        public event Action<byte[], int> DataAvailable;

        public void Dispose()
        {
            if (AudioSource != null)
            {
                AudioSource.StopRecording();
                AudioSource.DataAvailable -= OnDataAvailable;
                AudioSource.Dispose();
                AudioSource = null;
            }

            if (WaveWriter != null)
            {
                WaveWriter.Dispose();
                WaveWriter = null;
            }

            if (SilencePlayer != null)
            {
                SilencePlayer.Stop();
                SilencePlayer.Dispose();
                SilencePlayer = null;
            }
        }
    }
}
