using Captura.Models;
using System;

namespace Captura.ViewModels
{
    public class AudioViewModel : ViewModelBase, IDisposable
    {
        public AudioSource AudioSource { get; }

        public AudioViewModel(AudioSource AudioSource, Settings Settings, LanguageManager LanguageManager) : base(Settings, LanguageManager)
        {
            this.AudioSource = AudioSource;

            AudioSource.Init();
            
            AudioSource.Refresh();
        }
        
        public void Dispose() => AudioSource.Dispose();
    }
}