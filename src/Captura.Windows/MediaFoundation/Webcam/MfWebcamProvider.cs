using System.Collections.Generic;
using Captura.Models;

namespace DesktopDuplication
{
    public class MfWebcamProvider : IWebCamProvider
    {
        public IEnumerable<IWebcamItem> GetSources()
        {
            return MfCaptureDevice.Enumerate();
        }
    }
}
