using System;
using System.Drawing;
using Captura.Video;
using Moq;
using Xunit;

namespace Captura.Tests
{
    [Collection(nameof(Tests))]
    public class ImageProviderTests
    {
        readonly MoqFixture _moq;

        public ImageProviderTests(MoqFixture Moq)
        {
            _moq = Moq;
        }

        [Fact]
        public void OverlayImageProviderNull()
        {
            var overlay = _moq.GetOverlayMock().Object;

            Assert.Throws<ArgumentNullException>(() =>
            {
                using (new OverlayedImageProvider(null, overlay)) { }
            });
        }

        [Fact]
        public void OverlaysNull()
        {
            var imageProvider = _moq.GetImageProviderMock().Object;

            Assert.Throws<ArgumentNullException>(() =>
            {
                using (new OverlayedImageProvider(imageProvider, null)) { }
            });
        }

        [Fact]
        public void WindowProviderNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var platformServices = _moq.GetService<IPlatformServices>();

                using (platformServices.GetWindowProvider(null, false)) { }
            });
        }

        [Fact]
        public void RegionImageSize()
        {
            var rect = new Rectangle(0, 0, 100, 100);

            var platformServices = _moq.GetService<IPlatformServices>();

            using var imgProvider = platformServices.GetRegionProvider(rect, false);
            Assert.Equal(imgProvider.Width, rect.Width);
            Assert.Equal(imgProvider.Height, rect.Height);

            using var img = imgProvider.Capture();
            Assert.Equal(img.Width, rect.Width);
            Assert.Equal(img.Height, rect.Height);
        }

        [Fact]
        public void RegionImageSizeOdd()
        {
            var rect = new Rectangle(0, 0, 101, 53);

            var platformServices = _moq.GetService<IPlatformServices>();

            using var imgProvider = platformServices.GetRegionProvider(rect, false);
            Assert.True(imgProvider.Width % 2 == 0);
            Assert.True(imgProvider.Height % 2 == 0);

            using var img = imgProvider.Capture();
            Assert.Equal(img.Width, imgProvider.Width);
            Assert.Equal(img.Height, imgProvider.Height);

            Assert.True(img.Width % 2 == 0);
            Assert.True(img.Height % 2 == 0);
        }

        [Fact]
        public void OverlayedSize()
        {
            var imgProvider = _moq.GetImageProviderMock().Object;
            var overlay = _moq.GetOverlayMock().Object;

            using var provider = new OverlayedImageProvider(imgProvider, overlay);
            Assert.Equal(provider.Width, imgProvider.Width);
            Assert.Equal(provider.Height, imgProvider.Height);

            using var img = provider.Capture();
            Assert.Equal(provider.Width, img.Width);
            Assert.Equal(provider.Height, img.Height);
        }

        [Fact]
        public void CaptureOverlayedImage()
        {
            var imgProviderMock = _moq.GetImageProviderMock();
            var overlayMock = _moq.GetOverlayMock();

            using (var provider = new OverlayedImageProvider(imgProviderMock.Object, overlayMock.Object))
            {
                using (provider.Capture())
                {
                    imgProviderMock.Verify(M => M.Capture(), Times.Once);
                    overlayMock.Verify(M => M.Draw(It.IsAny<IEditableFrame>(), It.IsAny<Func<Point, Point>>()), Times.Once);
                }
            }

            imgProviderMock.Verify(M => M.Dispose(), Times.Once);
            overlayMock.Verify(M => M.Dispose(), Times.Once);
        }
    }
}