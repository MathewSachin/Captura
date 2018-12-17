using Captura.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows.Input;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainViewModel : ViewModelBase
    {
        readonly IDialogService _dialogService;
        readonly RecordingViewModel _recordingViewModel;
        readonly VideoSourcesViewModel _videoSourcesViewModel;

        public ICommand ShowPreviewCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand OpenOutputFolderCommand { get; }
        public ICommand SelectOutputFolderCommand { get; }
        public ICommand SelectFFmpegFolderCommand { get; } = new DelegateCommand(FFmpegService.SelectFFmpegFolder);
        public ICommand ResetFFmpegFolderCommand { get; }
        public ICommand TrayLeftClickCommand { get; }

        public MainViewModel(VideoSourcesViewModel VideoSourcesViewModel,
            Settings Settings,
            LanguageManager LanguageManager,
            HotKeyManager HotKeyManager,
            IPreviewWindow PreviewWindow,
            IDialogService DialogService,
            RecordingViewModel RecordingViewModel,
            MainModel MainModel) : base(Settings, LanguageManager)
        {
            _videoSourcesViewModel = VideoSourcesViewModel;
            _dialogService = DialogService;
            _recordingViewModel = RecordingViewModel;

            ShowPreviewCommand = new DelegateCommand(PreviewWindow.Show);

            #region Commands
            RefreshCommand = RecordingViewModel
                .ObserveProperty(M => M.RecorderState)
                .Select(M => M == RecorderState.NotRecording)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    MainModel.Refresh();

                    Refreshed?.Invoke();
                });

            OpenOutputFolderCommand = new DelegateCommand(OpenOutputFolder);

            SelectOutputFolderCommand = new DelegateCommand(SelectOutputFolder);

            ResetFFmpegFolderCommand = new DelegateCommand(() => Settings.FFmpeg.FolderPath = "");

            TrayLeftClickCommand = new DelegateCommand(() => HotKeyManager.FakeHotkey(Settings.Tray.LeftClickAction));
            #endregion

            Settings.Audio.PropertyChanged += (Sender, Args) =>
            {
                switch (Args.PropertyName)
                {
                    case nameof(Settings.Audio.Enabled):
                    case null:
                    case "":
                        CheckFunctionalityAvailability();
                        break;
                }
            };

            VideoSourcesViewModel.PropertyChanged += (Sender, Args) =>
            {
                switch (Args.PropertyName)
                {
                    case nameof(VideoSourcesViewModel.SelectedVideoSourceKind):
                    case null:
                    case "":
                        CheckFunctionalityAvailability();
                        break;
                }
            };
        }
        
        void CheckFunctionalityAvailability()
        {
            var audioAvailable = Settings.Audio.Enabled;

            var videoAvailable = !(_videoSourcesViewModel.SelectedVideoSourceKind is NoVideoSourceProvider);
            
            _recordingViewModel.RecordCommand.RaiseCanExecuteChanged(audioAvailable || videoAvailable);
        }

        public static IEnumerable<ObjectLocalizer<Alignment>> XAlignments { get; } = new[]
        {
            new ObjectLocalizer<Alignment>(Alignment.Start, nameof(LanguageManager.Left)),
            new ObjectLocalizer<Alignment>(Alignment.Center, nameof(LanguageManager.Center)),
            new ObjectLocalizer<Alignment>(Alignment.End, nameof(LanguageManager.Right))
        };

        public static IEnumerable<ObjectLocalizer<Alignment>> YAlignments { get; } = new[]
        {
            new ObjectLocalizer<Alignment>(Alignment.Start, nameof(LanguageManager.Top)),
            new ObjectLocalizer<Alignment>(Alignment.Center, nameof(LanguageManager.Center)),
            new ObjectLocalizer<Alignment>(Alignment.End, nameof(LanguageManager.Bottom))
        };

        void OpenOutputFolder()
        {
            Settings.EnsureOutPath();

            Process.Start(Settings.OutPath);
        }

        void SelectOutputFolder()
        {
            var folder = _dialogService.PickFolder(Settings.OutPath, Loc.SelectOutFolder);

            if (folder != null)
                Settings.OutPath = folder;
        }

        public event Action Refreshed;
    }
}