using Moq;
using Ninject.Modules;
using Screna;
using System.Drawing;
using Screna.Audio;

namespace Captura.Tests
{
    public class MoqModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IImageProvider>().ToMethod(Context =>
            {
                var mock = new Mock<IImageProvider>();

                const int size = 100;

                mock.Setup(M => M.Width).Returns(size);

                mock.Setup(M => M.Height).Returns(size);

                mock.Setup(M => M.Capture()).Returns(new OneTimeFrame(new Bitmap(size, size)));

                return mock.Object;
            });

            Bind<IAudioProvider>().ToMethod(Context =>
            {
                var mock = new Mock<IAudioProvider>();

                mock.Setup(M => M.WaveFormat).Returns(new WaveFormat());

                return mock.Object;
            });

            Bind<IAudioFileWriter>().ToMethod(Context =>
            {
                var mock = new Mock<IAudioFileWriter>();

                return mock.Object;
            });

            Bind<IVideoFileWriter>().ToMethod(Context =>
            {
                var mock = new Mock<IVideoFileWriter>();

                mock.Setup(M => M.SupportsAudio).Returns(true);

                return mock.Object;
            });
        }
    }
}