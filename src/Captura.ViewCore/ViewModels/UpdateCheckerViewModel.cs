using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Captura.Models;
using Reactive.Bindings;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UpdateCheckerViewModel : NotifyPropertyChanged
    {
        readonly IUpdateChecker _updateChecker;

        public UpdateCheckerViewModel(IUpdateChecker UpdateChecker)
        {
            _updateChecker = UpdateChecker;
            BuildName = UpdateChecker.BuildName;

            Check();

            CheckCommand = new ReactiveCommand()
                .WithSubscribe(Check);

            GoToDownload = new ReactiveCommand()
                .WithSubscribe(UpdateChecker.GoToDownloadsPage);
        }

        public string BuildName { get; }

        void Check()
        {
            if (Checking)
                return;

            Checking = true;
            UpdateAvailable = false;
            CheckFailed = false;

            Task.Run(async () =>
            {
                try
                {
                    var newVersion = await _updateChecker.Check();

                    if (newVersion != null)
                    {
                        UpdateAvailable = true;

                        NewVersion = "v" + newVersion;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());

                    CheckFailed = true;
                }
                finally
                {
                    Checking = false;
                }
            });
        }

        bool _checking, _available, _checkFailed;

        public bool Checking
        {
            get => _checking;
            private set => Set(ref _checking, value);
        }

        public bool UpdateAvailable
        {
            get => _available;
            private set => Set(ref _available, value);
        }

        public bool CheckFailed
        {
            get => _checkFailed;
            private set => Set(ref _checkFailed, value);
        }

        string _newVersion;

        public string NewVersion
        {
            get => _newVersion;
            private set => Set(ref _newVersion, value);
        }

        public ICommand CheckCommand { get; }

        public ICommand GoToDownload { get; }
    }
}