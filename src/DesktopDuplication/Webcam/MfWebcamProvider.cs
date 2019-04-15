using System.Collections.Generic;
using Captura.Models;

namespace DesktopDuplication
{
    class MfWebcamProvider : IWebCamProvider
    {
        public IEnumerable<IWebcamItem> GetSources()
        {
            return MfCaptureDevice.Enumerate();
        }
    }
}
