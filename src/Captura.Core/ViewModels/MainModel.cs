using System;
using Captura.Models;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainModel : NotifyPropertyChanged, IDisposable
    {
        readonly Settings _settings;
        bool _persist;

        readonly RememberByName _rememberByName;

        public MainModel(Settings Settings,
            RememberByName RememberByName)
        {
            _settings = Settings;
            _rememberByName = RememberByName;
        }

        public void Init(bool Persist, bool Remembered)
        {
            _persist = Persist;

            if (Remembered)
            {
                _rememberByName.RestoreRemembered();
            }
        }

        public void Dispose()
        {
            // Remember things if not console.
            if (!_persist)
                return;

            _rememberByName.Remember();

            _settings.Save();
        }
    }
}