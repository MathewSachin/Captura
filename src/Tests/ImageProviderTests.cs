using System;
using System.Drawing;
using System.Linq;
using Captura.Models;
using Moq;
using Screna;
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
                using (new OverlayedImageProvider(null, P => P, overlay)) { }
            });
        }

        [Fact]
        public void OverlaysNull()
        {
            var imageProvider = _moq.GetImageProviderMock().Object;

            Assert.Throws<ArgumentNullException>(() =>
            {
                using (new OverlayedImageProvider(imageProvider, P => P, null)) { }
            });
        }

        [Fact]
        public void OverlaysTransformNull()
        {
            var imageProvider = _moq.GetImageProviderMock().Object;
            var overlay = _moq.GetOverlayMock().Object;

            Assert.Throws<ArgumentNullException>(() =>
            {
                using (new OverlayedImageProvider(imageProvider, null, overlay)) { }
            });
        }

        [Fact]
        public void WindowProviderNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (new WindowProvider(null, false, out var _)) { }
            });
        }

        [Fact]
        public void ScreenImageSize()
        {
            var screenSourceProvider = ServiceProvider.Get<ScreenSourceProvider>();

            if (screenSourceProvider.FirstOrDefault() is ScreenItem screen)
            {
                using (var imgProvider = screen.GetImageProvider(false, out var _))
                {
                    Assert.Equal(imgProvider.Width, screen.Screen.Rectangle.Width);
                    Assert.Equal(imgProvider.Height, screen.Screen.Rectangle.Height);

                    using (var img = imgProvider.Capture())
                    {
                        Assert.Equal(img.Width, screen.Screen.Rectangle.Width);
                        Assert.Equal(img.Height, screen.Screen.Rectangle.Height);
                    }
                }
            }
        }

        [Fact]
        public void ScreenCount()
        {
            var screenSourceProvider = ServiceProvider.Get<ScreenSourceProvider>();

            // There should be atleast 3 screen sources including Full screen and Screen Picker
            Assert.True(screenSourceProvider.Count() >= 3);
        }

        [Fact]
        public void DeskDuplCount()
        {
            var deskDuplProvider = ServiceProvider.Get<DeskDuplSourceProvider>();
            
            // Atleast one screen
            Assert.True(deskDuplProvider.Any());
        }

        [Fact]
        public void RegionImageSize()
        {
            var rect = new Rectangle(0, 0, 100, 100);

            using (var imgProvider = new RegionProvider(rect, false))
            {
                Assert.Equal(imgProvider.Width, rect.Width);
                Assert.Equal(imgProvider.Height, rect.Height);

                using (var img = imgProvider.Capture())
                {
                    Assert.Equal(img.Width, rect.Width);
                    Assert.Equal(img.Height, rect.Height);
                }
            }
        }

        [Fact]
        public void RegionImageSizeOdd()
        {
            var rect = new Rectangle(0, 0, 101, 53);

            using (var imgProvider = new RegionProvider(rect, false))
            {
                Assert.True(imgProvider.Width % 2 == 0);
                Assert.True(imgProvider.Height % 2 == 0);

                using (var img = imgProvider.Capture())
                {
                    Assert.Equal(img.Width, imgProvider.Width);
                    Assert.Equal(img.Height, imgProvider.Height);

                    Assert.True(img.Width % 2 == 0);
                    Assert.True(img.Height % 2 == 0);
                }
            }
        }

        [Fact]
        public void OverlayedSize()
        {
            var imgProvider = _moq.GetImageProviderMock().Object;
            var overlay = _moq.GetOverlayMock().Object;

            using (var provider = new OverlayedImageProvider(imgProvider, P => P, overlay))
            {
                Assert.Equal(provider.Width, imgProvider.Width);
                Assert.Equal(provider.Height, imgProvider.Height);

                using (var img = provider.Capture())
                {
                    Assert.Equal(provider.Width, img.Width);
                    Assert.Equal(provider.Height, img.Height);
                }
            }
        }

        [Fact]
        public void CaptureOverlayedImage()
        {
            var imgProviderMock = _moq.GetImageProviderMock();
            var overlayMock = _moq.GetOverlayMock();

            using (var provider = new OverlayedImageProvider(imgProviderMock.Object, P => P, overlayMock.Object))
            {
                using (provider.Capture())
                {
                    imgProviderMock.Verify(M => M.Capture(), Times.Once);
                    overlayMock.Verify(M => M.Draw(It.IsAny<IBitmapEditor>(), It.IsAny<Func<Point, Point>>()), Times.Once);
                }
            }

            imgProviderMock.Verify(M => M.Dispose(), Times.Once);
            overlayMock.Verify(M => M.Dispose(), Times.Once);
        }
    }
}