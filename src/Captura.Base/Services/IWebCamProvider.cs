using System.Collections.Generic;

namespace Captura.Webcam
{
    public interface IWebCamProvider
    {
        IEnumerable<IWebcamItem> GetSources();
    }
}
