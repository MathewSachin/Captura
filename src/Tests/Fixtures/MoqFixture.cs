using System;
using Captura.Audio;
using Captura.Video;
using Moq;

namespace Captura.Tests
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MoqFixture : IDisposable
    {
        public void Dispose() { }

        public Mock<IImageProvider> GetImageProviderMock(int Width = 100, int Height = 50)
        {
            var mock = new Mock<IImageProvider>();

            mock.Setup(M => M.Width).Returns(Width);

            mock.Setup(M => M.Height).Returns(Height);

            var editorMock = new Mock<IEditableFrame>();
            editorMock.Setup(M => M.Width).Returns(Width);
            editorMock.Setup(M => M.Height).Returns(Height);

            mock.Setup(M => M.Capture()).Returns(editorMock.Object);

            return mock;
        }

        public Mock<IAudioProvider> GetAudioProviderMock()
        {
            var mock = new Mock<IAudioProvider>();

            mock.Setup(M => M.WaveFormat).Returns(new WaveFormat());

            return mock;
        }

        public Mock<IAudioFileWriter> GetAudioFileWriterMock()
        {
            return new Mock<IAudioFileWriter>();
        }

        public Mock<IVideoFileWriter> GetVideoFileWriterMock()
        {
            var mock = new Mock<IVideoFileWriter>();

            mock.Setup(M => M.WriteFrame(It.IsAny<IBitmapFrame>()))
                .Callback<IBitmapFrame>(M => M.Dispose());

            mock.Setup(M => M.SupportsAudio).Returns(true);

            return mock;
        }

        public Mock<IOverlay> GetOverlayMock()
        {
            return new Mock<IOverlay>();
        }

        public T GetService<T>() => ServiceProvider.Get<T>();
    }
}