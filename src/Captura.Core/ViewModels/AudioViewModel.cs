using Captura.Models;
using System;

namespace Captura.ViewModels
{
    public class AudioViewModel : ViewModelBase, IDisposable
    {
        public AudioSource AudioSource { get; }

        public AudioViewModel()
        {
            if (BassAudioSource.Available)
                AudioSource = new BassAudioSource();
            /*
            else if (NAudioSource.Available)
                AudioSource = new NAudioSource();
            */
            else AudioSource = NoAudioSource.Instance;

            AudioSource.Init();
            
            AudioSource.Refresh();
        }
        
        public void Dispose() => AudioSource.Dispose();
    }
}