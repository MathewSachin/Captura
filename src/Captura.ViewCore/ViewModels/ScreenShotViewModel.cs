using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Reactive.Linq;
using System.Windows.Input;
using Captura.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Screna;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ScreenShotViewModel : ViewModelBase
    {
        public DiskWriter DiskWriter { get; }
        public ClipboardWriter ClipboardWriter { get; }
        public ImageUploadWriter ImgurWriter { get; }

        public ScreenShotViewModel(LanguageManager Loc,
            Settings Settings,
            DiskWriter DiskWriter,
            ClipboardWriter ClipboardWriter,
            ImageUploadWriter ImgurWriter,
            ScreenShotModel ScreenShotModel,
            VideoSourcesViewModel VideoSourcesViewModel) : base(Settings, Loc)
        {
            this.DiskWriter = DiskWriter;
            this.ClipboardWriter = ClipboardWriter;
            this.ImgurWriter = ImgurWriter;

            ScreenShotCommand = VideoSourcesViewModel
                .ObserveProperty(M => M.SelectedVideoSourceKind)
                .Select(M => !(M is NoVideoSourceProvider))
                .ToReactiveCommand()
                .WithSubscribe(() => ScreenShotModel.CaptureScreenShot());

            ScreenShotActiveCommand = new DelegateCommand(async () => await ScreenShotModel.SaveScreenShot(ScreenShotModel.ScreenShotWindow(Window.ForegroundWindow)));
            ScreenShotDesktopCommand = new DelegateCommand(async () => await ScreenShotModel.SaveScreenShot(ScreenShotModel.ScreenShotWindow(Window.DesktopWindow)));
            ScreenshotRegionCommand = new DelegateCommand(async () => await ScreenShotModel.ScreenshotRegion());
            ScreenshotWindowCommand = new DelegateCommand(async () => await ScreenShotModel.ScreenshotWindow());
            ScreenshotScreenCommand = new DelegateCommand(async () => await ScreenShotModel.ScreenshotScreen());

            ScreenShotImageFormats = ScreenShotModel.ScreenShotImageFormats;

            SelectedScreenShotImageFormat = ScreenShotModel
                .ToReactivePropertyAsSynchronized(M => M.SelectedScreenShotImageFormat);
        }

        public ICommand ScreenShotCommand { get; }
        public ICommand ScreenShotActiveCommand { get; }
        public ICommand ScreenShotDesktopCommand { get; }
        public ICommand ScreenshotRegionCommand { get; }
        public ICommand ScreenshotWindowCommand { get; }
        public ICommand ScreenshotScreenCommand { get; }

        public IEnumerable<ImageFormat> ScreenShotImageFormats { get; }

        public IReactiveProperty<ImageFormat> SelectedScreenShotImageFormat { get; }
    }
}