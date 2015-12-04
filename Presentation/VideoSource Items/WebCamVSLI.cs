using System.Collections.Generic;
using System.Drawing;
using ScreenWorks;

namespace Captura
{
    class WebCamVSLI : IVideoSourceListItem
    {
        WebCam Device;

        WebCamVSLI(WebCam Device) { this.Device = Device; }

        public static IEnumerable<WebCamVSLI> Enumerate()
        {
            foreach (var Cam in WebCam.Enumerate())
                yield return new WebCamVSLI(Cam);
        }

        public string Name { get { return Device.Name; } }

        public Bitmap Capture() { return Device.TakePicture(); }

        public WebCamProvider ToWebCamProvider() { return new WebCamProvider(Device); }
    }
}