﻿using Captura.Models;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class HotkeyActor : IHotkeyActor
    {
        readonly ScreenShotViewModel _screenShotViewModel;
        readonly RecordingViewModel _recordingViewModel;
        readonly Settings _settings;

        public HotkeyActor(ScreenShotViewModel ScreenShotViewModel,
            RecordingViewModel RecordingViewModel,
            Settings Settings)
        {
            _screenShotViewModel = ScreenShotViewModel;
            _recordingViewModel = RecordingViewModel;
            _settings = Settings;
        }

        public void Act(ServiceName Service)
        {
            switch (Service)
            {
                case ServiceName.Recording:
                    _recordingViewModel.RecordCommand.ExecuteIfCan();
                    break;

                case ServiceName.Pause:
                    _recordingViewModel.PauseCommand.ExecuteIfCan();
                    break;

                case ServiceName.ScreenShot:
                    _screenShotViewModel.ScreenShotCommand.ExecuteIfCan();
                    break;

                case ServiceName.ActiveScreenShot:
                    _screenShotViewModel.ScreenShotActiveCommand.ExecuteIfCan();
                    break;

                case ServiceName.DesktopScreenShot:
                    _screenShotViewModel.ScreenShotDesktopCommand.ExecuteIfCan();
                    break;

                case ServiceName.ToggleMouseClicks:
                    _settings.Clicks.Display = !_settings.Clicks.Display;
                    break;

                case ServiceName.ToggleKeystrokes:
                    _settings.Keystrokes.Display = !_settings.Keystrokes.Display;
                    break;

                case ServiceName.ScreenShotRegion:
                    _screenShotViewModel.ScreenshotRegionCommand.ExecuteIfCan();
                    break;

                case ServiceName.ScreenShotScreen:
                    _screenShotViewModel.ScreenshotScreenCommand.ExecuteIfCan();
                    break;

                case ServiceName.ScreenShotWindow:
                    _screenShotViewModel.ScreenshotWindowCommand.ExecuteIfCan();
                    break;
            }
        }
    }
}