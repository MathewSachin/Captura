using Captura.Models;
using Captura.ViewModels;

namespace Captura
{
    public partial class SettingsWindow
    {
        SettingsWindow()
        {
            InitializeComponent();

            Closing += (S, E) => RefreshFFmpegCodecs();
        }

        static void RefreshFFmpegCodecs()
        {
            // Refresh only if not Recording and FFmpeg encoder is selected
            var mainVm = ServiceProvider.Get<MainViewModel>();

            var canRefresh = mainVm.RefreshCommand.CanExecute(null);

            if (!canRefresh)
                return;

            var writersVm = ServiceProvider.Get<VideoWritersViewModel>();

            if (!(writersVm.SelectedVideoWriterKind is FFmpegWriterProvider))
                return;

            if (writersVm is IRefreshable refreshable)
            {
                refreshable.Refresh();
            }
        }

        static SettingsWindow _instance;

        public static void ShowInstance()
        {
            if (_instance == null)
            {
                _instance = new SettingsWindow();

                _instance.Closed += (S, E) => _instance = null;
            }

            _instance.ShowAndFocus();
        }
    }
}
