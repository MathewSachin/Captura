using System.Drawing;
using Moq;
using Screna;
using Screna.Audio;

namespace Captura.Tests
{
    public static class MoqFactory
    {
        public static Mock<IImageProvider> GetImageProviderMock(int Width = 100, int Height = 50)
        {
            var mock = new Mock<IImageProvider>();

            mock.Setup(M => M.Width).Returns(Width);

            mock.Setup(M => M.Height).Returns(Height);

            mock.Setup(M => M.Capture()).Returns(new OneTimeFrame(new Bitmap(Width, Height)));

            return mock;
        }

        public static Mock<IAudioProvider> GetAudioProviderMock()
        {
            var mock = new Mock<IAudioProvider>();

            mock.Setup(M => M.WaveFormat).Returns(new WaveFormat());

            return mock;
        }

        public static Mock<IAudioFileWriter> GetAudioFileWriterMock()
        {
            return new Mock<IAudioFileWriter>();
        }

        public static Mock<IVideoFileWriter> GetVideoFileWriterMock()
        {
            var mock = new Mock<IVideoFileWriter>();

            mock.Setup(M => M.WriteFrame(It.IsAny<IBitmapFrame>()))
                .Callback<IBitmapFrame>(M => M.Dispose());

            mock.Setup(M => M.SupportsAudio).Returns(true);

            return mock;
        }

        public static Mock<IOverlay> GetOverlayMock()
        {
            return new Mock<IOverlay>();
        }
    }
}