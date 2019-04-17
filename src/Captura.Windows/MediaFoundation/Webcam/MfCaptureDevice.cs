using System;
using System.Collections.Generic;
using System.Linq;
using Captura.Models;
using SharpDX.MediaFoundation;

namespace DesktopDuplication
{
    public class MfCaptureDevice : IWebcamItem
    {
        readonly Activate _activate;

        MfCaptureDevice(Activate Activate)
        {
            _activate = Activate;
            Name = Activate.Get(CaptureDeviceAttributeKeys.FriendlyName);
        }

        public string Name { get; }

        public MediaSource GetSource()
        {
            return _activate.ActivateObject<MediaSource>();
        }

        public static IEnumerable<MfCaptureDevice> Enumerate()
        {
            var attribs = new MediaAttributes(1);
            attribs.Set(CaptureDeviceAttributeKeys.SourceType, CaptureDeviceAttributeKeys.SourceTypeVideoCapture.Guid);

            return MediaFactory.EnumDeviceSources(attribs)
                .Select(M => new MfCaptureDevice(M));
        }

        public IWebcamCapture BeginCapture(Action OnClick)
        {
            return new MfCapture(this);
        }
    }
}