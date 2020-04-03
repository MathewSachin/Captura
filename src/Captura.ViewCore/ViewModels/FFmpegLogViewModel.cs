using System;
using System.Reactive.Linq;
using System.Windows.Input;
using Captura.FFmpeg;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FFmpegLogViewModel : NotifyPropertyChanged
    {
        public FFmpegLogViewModel(IClipboardService ClipboardService,
            FFmpegLog FFmpegLog)
        {
            LogItems = FFmpegLog
                .LogItems
                .ToReadOnlyReactiveCollection();

            LogItems
                .ObserveAddChanged()
                .Subscribe(M => SelectedLogItem = M);

            LogItems
                .ObserveRemoveChanged()
                .Subscribe(M =>
                {
                    if (LogItems.Count > 0)
                        SelectedLogItem = LogItems[0];
                });

            if (LogItems.Count > 0)
                SelectedLogItem = LogItems[0];

            CopyToClipboardCommand = this
                .ObserveProperty(M => M.SelectedLogItem)
                .Select(M => M != null)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    ClipboardService.SetText(SelectedLogItem.GetCompleteLog());
                });

            RemoveCommand = this
                .ObserveProperty(M => M.SelectedLogItem)
                .Select(M => M != null)
                .ToReactiveCommand()
                .WithSubscribe(() => FFmpegLog.Remove(SelectedLogItem));
        }

        public ReadOnlyReactiveCollection<FFmpegLogItem> LogItems { get; }

        FFmpegLogItem _selectedLogItem;

        public FFmpegLogItem SelectedLogItem
        {
            get => _selectedLogItem;
            set => Set(ref _selectedLogItem, value);
        }

        public ICommand CopyToClipboardCommand { get; }

        public ICommand RemoveCommand { get; }
    }
}
