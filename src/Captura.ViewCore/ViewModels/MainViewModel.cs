using Captura.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows.Input;
using Captura.FFmpeg;
using Captura.Hotkeys;
using Captura.Loc;
using Captura.Video;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainViewModel : ViewModelBase, IDisposable
    {
        bool _persist;

        readonly RememberByName _rememberByName;
        readonly IDialogService _dialogService;

        public ICommand ShowPreviewCommand { get; }
        public ICommand OpenOutputFolderCommand { get; }
        public ICommand SelectOutputFolderCommand { get; }
        public ICommand SelectFFmpegFolderCommand { get; }
        public ICommand ResetFFmpegFolderCommand { get; }
        public ICommand TrayLeftClickCommand { get; }

        public IReadOnlyReactiveProperty<string> OutFolderDisplay { get; }

        public MainViewModel(Settings Settings,
            ILocalizationProvider Loc,
            HotKeyManager HotKeyManager,
            IPreviewWindow PreviewWindow,
            IDialogService DialogService,
            RecordingModel RecordingModel,
            IFFmpegViewsProvider FFmpegViewsProvider,
            RememberByName RememberByName) : base(Settings, Loc)
        {
            _dialogService = DialogService;
            _rememberByName = RememberByName;

            OutFolderDisplay = Settings
                .ObserveProperty(M => M.OutPath)
                .Select(M => Settings.GetOutputPath())
                .ToReadOnlyReactivePropertySlim();

            ShowPreviewCommand = new ReactiveCommand()
                .WithSubscribe(PreviewWindow.Show);

            SelectFFmpegFolderCommand = new ReactiveCommand()
                .WithSubscribe(FFmpegViewsProvider.PickFolder);

            #region Commands
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

        public void Init(bool Persist, bool Remembered)
        {
            _persist = Persist;

            if (Remembered)
            {
                _rememberByName.RestoreRemembered();
            }
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
            string currentFolder = null;

            try
            {
                currentFolder = Settings.GetOutputPath();
            }
            catch
            {
                // Error can happen if current folder is inaccessible
            }

            var folder = _dialogService.PickFolder(currentFolder, Loc.SelectOutFolder);

            if (folder != null)
                Settings.OutPath = folder;
        }

        public void Dispose()
        {
            // Remember things if not console.
            if (!_persist)
                return;

            _rememberByName.Remember();

            Settings.Save();
        }
    }
}