using Moq;
using Ninject.Modules;
using Screna;
using System.Drawing;
using Ninject;
using Screna.Audio;

namespace Captura.Tests
{
    public class MoqModule : NinjectModule
    {
        public override void Load()
        {
            Bind<Mock<IImageProvider>>().ToMethod(Context =>
            {
                var mock = new Mock<IImageProvider>();

                const int width = 100;
                const int height = 50;

                mock.Setup(M => M.Width).Returns(width);

                mock.Setup(M => M.Height).Returns(height);

                mock.Setup(M => M.Capture()).Returns(new OneTimeFrame(new Bitmap(width, height)));

                return mock;
            });

            Bind<IImageProvider>().ToMethod(Context => Context.Kernel.Get<Mock<IImageProvider>>().Object);

            Bind<Mock<IAudioProvider>>().ToMethod(Context =>
            {
                var mock = new Mock<IAudioProvider>();

                mock.Setup(M => M.WaveFormat).Returns(new WaveFormat());

                return mock;
            });

            Bind<IAudioProvider>().ToMethod(Context => Context.Kernel.Get<Mock<IAudioProvider>>().Object);

            Bind<Mock<IAudioFileWriter>>().ToMethod(Context => new Mock<IAudioFileWriter>());

            Bind<IAudioFileWriter>().ToMethod(Context => Context.Kernel.Get<Mock<IAudioFileWriter>>().Object);

            Bind<Mock<IVideoFileWriter>>().ToMethod(Context =>
            {
                var mock = new Mock<IVideoFileWriter>();

                mock.Setup(M => M.SupportsAudio).Returns(true);

                return mock;
            });

            Bind<IVideoFileWriter>().ToMethod(Context => Context.Kernel.Get<Mock<IVideoFileWriter>>().Object);

            Bind<Mock<IOverlay>>().ToMethod(Context => new Mock<IOverlay>());

            Bind<IOverlay>().ToMethod(Context => Context.Kernel.Get<Mock<IOverlay>>().Object);
        }
    }
}