﻿using System.Linq;
using Captura.Audio;
using Captura.Models;
using NAudio.CoreAudioApi;

namespace Captura.NAudio
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NAudioSource : AudioSource
    {
        public NAudioSource()
        {
            Refresh();
        }

        protected override void OnRefresh()
        {
            var enumerator = new MMDeviceEnumerator();

            var loopbackDevs = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            foreach (var loopback in loopbackDevs)
            {
                LoopbackSources.Add(new NAudioItem(loopback));
            }

            var recordingDevs = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

            foreach (var recording in recordingDevs)
            {
                RecordingSources.Add(new NAudioItem(recording));
            }
        }

        public override IAudioProvider GetMixedAudioProvider()
        {
            throw new System.NotImplementedException();
        }

        public override IAudioProvider[] GetMultipleAudioProviders()
        {
            var rec = AvailableRecordingSources.Where(M => M.Active)
                .Cast<NAudioItem>()
                .Select(M => new WasapiCaptureProvider(M.Device))
                .Cast<IAudioProvider>();

            var loop = AvailableLoopbackSources.Where(M => M.Active)
                .Cast<NAudioItem>()
                .Select(M => new WasapiLoopbackCaptureProvider(M.Device))
                .Cast<IAudioProvider>();

            return rec.Concat(loop).ToArray();
        }
    }
}
