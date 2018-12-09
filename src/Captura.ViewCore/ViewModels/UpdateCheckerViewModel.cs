using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Captura.Models;

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

            CheckCommand = new DelegateCommand(Check);

            GoToDownload = new DelegateCommand(UpdateChecker.GoToDownloadsPage);
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
            private set
            {
                _checking = value;

                OnPropertyChanged();
            }
        }

        public bool UpdateAvailable
        {
            get => _available;
            private set
            {
                _available = value;

                OnPropertyChanged();
            }
        }

        public bool CheckFailed
        {
            get => _checkFailed;
            private set
            {
                _checkFailed = value;

                OnPropertyChanged();
            }
        }

        string _newVersion;

        public string NewVersion
        {
            get => _newVersion;
            private set
            {
                _newVersion = value;
                OnPropertyChanged();
            }
        }

        public ICommand CheckCommand { get; }

        public ICommand GoToDownload { get; }
    }
}