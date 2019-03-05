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

        public ICommand ShowPreviewCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand OpenOutputFolderCommand { get; }
        public ICommand SelectOutputFolderCommand { get; }
        public ICommand SelectFFmpegFolderCommand { get; } = new DelegateCommand(FFmpegService.SelectFFmpegFolder);
        public ICommand ResetFFmpegFolderCommand { get; }
        public ICommand TrayLeftClickCommand { get; }

        public MainViewModel(Settings Settings,
            ILocalizationProvider Loc,
            HotKeyManager HotKeyManager,
            IPreviewWindow PreviewWindow,
            IDialogService DialogService,
            RecordingModel RecordingModel,
            MainModel MainModel) : base(Settings, Loc)
        {
            _dialogService = DialogService;

            ShowPreviewCommand = new DelegateCommand(PreviewWindow.Show);

            #region Commands
            RefreshCommand = RecordingModel
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
        }

        public static IEnumerable<ObjectLocalizer<Alignment>> XAlignments { get; } = new[]
        {
            new ObjectLocalizer<Alignment>(Alignment.Start, nameof(ILocalizationProvider.Left)),
            new ObjectLocalizer<Alignment>(Alignment.Center, nameof(ILocalizationProvider.Center)),
            new ObjectLocalizer<Alignment>(Alignment.End, nameof(ILocalizationProvider.Right))
        };

        public static IEnumerable<ObjectLocalizer<Alignment>> YAlignments { get; } = new[]
        {
            new ObjectLocalizer<Alignment>(Alignment.Start, nameof(ILocalizationProvider.Top)),
            new ObjectLocalizer<Alignment>(Alignment.Center, nameof(ILocalizationProvider.Center)),
            new ObjectLocalizer<Alignment>(Alignment.End, nameof(ILocalizationProvider.Bottom))
        };

        void OpenOutputFolder()
        {
            Settings.EnsureOutPath();

            Process.Start(Settings.GetOutputPath());
        }

        void SelectOutputFolder()
        {
            var folder = _dialogService.PickFolder(Settings.GetOutputPath(), Loc.SelectOutFolder);

            if (folder != null)
                Settings.OutPath = folder;
        }

        public event Action Refreshed;
    }
}