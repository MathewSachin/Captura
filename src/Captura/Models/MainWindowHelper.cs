using Captura.Hotkeys;
using Captura.ViewModels;

namespace Captura
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class MainWindowHelper
    {
        public MainWindowHelper(MainViewModel MainViewModel,
            HotkeySetup HotkeySetup,
            TimerModel TimerModel,
            Settings Settings,
            RecordingViewModel RecordingViewModel)
        {
            this.MainViewModel = MainViewModel;
            this.HotkeySetup = HotkeySetup;
            this.TimerModel = TimerModel;
            this.Settings = Settings;
            this.RecordingViewModel = RecordingViewModel;
        }

        public MainViewModel MainViewModel { get; }

        public HotkeySetup HotkeySetup { get; }

        public TimerModel TimerModel { get; }

        public Settings Settings { get; }

        public RecordingViewModel RecordingViewModel { get; }
    }
}