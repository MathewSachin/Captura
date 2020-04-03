using Captura.Hotkeys;
using Captura.Models;
using Captura.Video;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class HotkeyActor : IHotkeyActor
    {
        readonly ScreenShotViewModel _screenShotViewModel;
        readonly RecordingViewModel _recordingViewModel;
        readonly Settings _settings;
        readonly VideoSourcesViewModel _videoSourcesViewModel;
        readonly RegionSourceProvider _regionSourceProvider;

        public HotkeyActor(ScreenShotViewModel ScreenShotViewModel,
            RecordingViewModel RecordingViewModel,
            Settings Settings,
            VideoSourcesViewModel VideoSourcesViewModel,
            RegionSourceProvider RegionSourceProvider)
        {
            _screenShotViewModel = ScreenShotViewModel;
            _recordingViewModel = RecordingViewModel;
            _settings = Settings;
            _videoSourcesViewModel = VideoSourcesViewModel;
            _regionSourceProvider = RegionSourceProvider;
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

                case ServiceName.ToggleRegionPicker:
                    // Stop any recording in progress
                    if (_recordingViewModel.RecorderState.Value != RecorderState.NotRecording)
                    {
                        _recordingViewModel.RecordCommand.Execute(null);
                    }

                    if (_videoSourcesViewModel.SelectedVideoSourceKind != _regionSourceProvider)
                    {
                        _videoSourcesViewModel.SelectedVideoSourceKind = _regionSourceProvider;

                        if (_settings.RegionPickerHotkeyAutoStartRecording)
                        {
                            _recordingViewModel.RecordCommand.Execute(null);
                        }
                    }
                    else _videoSourcesViewModel.SetDefaultSource();
                    break;
            }
        }
    }
}