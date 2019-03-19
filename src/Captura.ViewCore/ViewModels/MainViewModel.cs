using Captura.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows.Input;
using Captura.FFmpeg;
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
        public ICommand SelectFFmpegFolderCommand { get; }
        public ICommand ResetFFmpegFolderCommand { get; }
        public ICommand TrayLeftClickCommand { get; }

        public MainViewModel(Settings Settings,
            ILocalizationProvider Loc,
            HotKeyManager HotKeyManager,
            IPreviewWindow PreviewWindow,
            IDialogService DialogService,
            RecordingModel RecordingModel,
            IEnumerable<IRefreshable> Refreshables,
            IFFmpegViewsProvider FFmpegViewsProvider) : base(Settings, Loc)
        {
            _dialogService = DialogService;

            ShowPreviewCommand = new ReactiveCommand()
                .WithSubscribe(PreviewWindow.Show);

            SelectFFmpegFolderCommand = new ReactiveCommand()
                .WithSubscribe(FFmpegViewsProvider.PickFolder);

            #region Commands
            RefreshCommand = RecordingModel
                .ObserveProperty(M => M.RecorderState)
                .Select(M => M == RecorderState.NotRecording)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    foreach (var refreshable in Refreshables)
                    {
                        refreshable.Refresh();
                    }

                    Refreshed?.Invoke();
                });

            OpenOutputFolderCommand = new ReactiveCommand()
                .WithSubscribe(OpenOutputFolder);

            SelectOutputFolderCommand = new ReactiveCommand()
                .WithSubscribe(SelectOutputFolder);

            ResetFFmpegFolderCommand = new ReactiveCommand()
                .WithSubscribe(() => Settings.FFmpeg.FolderPath = "");

            TrayLeftClickCommand = new ReactiveCommand()
                .WithSubscribe(() => HotKeyManager.FakeHotkey(Settings.Tray.LeftClickAction));
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