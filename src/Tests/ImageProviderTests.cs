using System;
using System.Drawing;
using System.Linq;
using Captura.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Screna;

namespace Captura.Tests
{
    [TestClass]
    public class ImageProviderTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OverlayImageProviderNull()
        {
            var overlay = ServiceProvider.Get<IOverlay>();

            using (new OverlayedImageProvider(null, P => P, overlay))
            {
                
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OverlaysNull()
        {
            var imageProvider = ServiceProvider.Get<IImageProvider>();

            using (new OverlayedImageProvider(imageProvider, P => P, null))
            {

            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OverlaysTransformNull()
        {
            var imageProvider = ServiceProvider.Get<IImageProvider>();
            var overlay = ServiceProvider.Get<IOverlay>();

            using (new OverlayedImageProvider(imageProvider, null, overlay))
            {

            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WindowProviderNull()
        {
            using (new WindowProvider(null, false, out var _))
            {

            }
        }

        [TestMethod]
        public void ScreenImageSize()
        {
            var screenSourceProvider = ServiceProvider.Get<ScreenSourceProvider>();

            if (screenSourceProvider.FirstOrDefault() is ScreenItem screen)
            {
                using (var imgProvider = screen.GetImageProvider(false, out var _))
                {
                    Assert.AreEqual(imgProvider.Width, screen.Screen.Bounds.Width);
                    Assert.AreEqual(imgProvider.Height, screen.Screen.Bounds.Height);

                    using (var img = imgProvider.Capture())
                    {
                        if (img.Bitmap != null)
                        {
                            Assert.AreEqual(img.Bitmap.Width, screen.Screen.Bounds.Width);
                            Assert.AreEqual(img.Bitmap.Height, screen.Screen.Bounds.Height);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ScreenCount()
        {
            var screenSourceProvider = ServiceProvider.Get<ScreenSourceProvider>();

            // There should be atleast two screen sources including Full screen
            Assert.IsTrue(screenSourceProvider.Count() >= 2);
        }

        [TestMethod]
        public void DeskDuplCount()
        {
            var deskDuplProvider = ServiceProvider.Get<DeskDuplSourceProvider>();
            
            // Atleast one screen
            Assert.IsTrue(deskDuplProvider.Any());
        }

        [TestMethod]
        public void RegionImageSize()
        {
            var rect = new Rectangle(0, 0, 100, 100);

            using (var imgProvider = new RegionProvider(rect, false))
            {
                Assert.AreEqual(imgProvider.Width, rect.Width);
                Assert.AreEqual(imgProvider.Height, rect.Height);

                using (var img = imgProvider.Capture())
                {
                    if (img.Bitmap != null)
                    {
                        Assert.AreEqual(img.Bitmap.Width, rect.Width);
                        Assert.AreEqual(img.Bitmap.Height, rect.Height);
                    }
                }
            }
        }

        [TestMethod]
        public void OverlayedSize()
        {
            var imgProvider = ServiceProvider.Get<IImageProvider>();
            var overlay = ServiceProvider.Get<IOverlay>();

            using (var provider = new OverlayedImageProvider(imgProvider, P => P, overlay))
            {
                Assert.AreEqual(provider.Width, imgProvider.Width);
                Assert.AreEqual(provider.Height, imgProvider.Height);

                using (var img = provider.Capture())
                {
                    if (img.Bitmap != null)
                    {
                        Assert.AreEqual(provider.Width, img.Bitmap.Width);
                        Assert.AreEqual(provider.Height, img.Bitmap.Height);
                    }
                }
            }
        }

        [TestMethod]
        public void CaptureOverlayedImage()
        {
            var imgProviderMock = ServiceProvider.Get<Mock<IImageProvider>>();
            var overlayMock = ServiceProvider.Get<Mock<IOverlay>>();

            using (var provider = new OverlayedImageProvider(imgProviderMock.Object, P => P, overlayMock.Object))
            {
                using (provider.Capture())
                {
                    imgProviderMock.Verify(M => M.Capture(), Times.Once);
                    overlayMock.Verify(M => M.Draw(It.IsAny<Graphics>(), It.IsAny<Func<Point, Point>>()), Times.Once);
                }
            }

            imgProviderMock.Verify(M => M.Dispose(), Times.Once);
            overlayMock.Verify(M => M.Dispose(), Times.Once);
        }
    }
}