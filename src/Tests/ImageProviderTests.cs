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
        [ClassInitialize]
        public static void Init(TestContext Context)
        {
            ServiceProvider.LoadModule(new FakesModule());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OverlayImageProviderNull()
        {
            var overlay = MoqFactory.GetOverlayMock().Object;

            using (new OverlayedImageProvider(null, P => P, overlay))
            {
                
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OverlaysNull()
        {
            var imageProvider = MoqFactory.GetImageProviderMock().Object;

            using (new OverlayedImageProvider(imageProvider, P => P, null))
            {

            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OverlaysTransformNull()
        {
            var imageProvider = MoqFactory.GetImageProviderMock().Object;
            var overlay = MoqFactory.GetOverlayMock().Object;

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
                        Assert.AreEqual(img.Width, screen.Screen.Bounds.Width);
                        Assert.AreEqual(img.Height, screen.Screen.Bounds.Height);
                    }
                }
            }
        }

        [TestMethod]
        public void ScreenCount()
        {
            var screenSourceProvider = ServiceProvider.Get<ScreenSourceProvider>();

            // There should be atleast 3 screen sources including Full screen and Screen Picker
            Assert.IsTrue(screenSourceProvider.Count() >= 3);
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
                    Assert.AreEqual(img.Width, rect.Width);
                    Assert.AreEqual(img.Height, rect.Height);
                }
            }
        }

        [TestMethod]
        public void RegionImageSizeOdd()
        {
            var rect = new Rectangle(0, 0, 101, 53);

            using (var imgProvider = new RegionProvider(rect, false))
            {
                Assert.IsTrue(imgProvider.Width % 2 == 0);
                Assert.IsTrue(imgProvider.Height % 2 == 0);

                using (var img = imgProvider.Capture())
                {
                    Assert.AreEqual(img.Width, imgProvider.Width);
                    Assert.AreEqual(img.Height, imgProvider.Height);

                    Assert.IsTrue(img.Width % 2 == 0);
                    Assert.IsTrue(img.Height % 2 == 0);
                }
            }
        }

        [TestMethod]
        public void OverlayedSize()
        {
            var imgProvider = MoqFactory.GetImageProviderMock().Object;
            var overlay = MoqFactory.GetOverlayMock().Object;

            using (var provider = new OverlayedImageProvider(imgProvider, P => P, overlay))
            {
                Assert.AreEqual(provider.Width, imgProvider.Width);
                Assert.AreEqual(provider.Height, imgProvider.Height);

                using (var img = provider.Capture())
                {
                    Assert.AreEqual(provider.Width, img.Width);
                    Assert.AreEqual(provider.Height, img.Height);
                }
            }
        }

        [TestMethod]
        public void CaptureOverlayedImage()
        {
            var imgProviderMock = MoqFactory.GetImageProviderMock();
            var overlayMock = MoqFactory.GetOverlayMock();

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