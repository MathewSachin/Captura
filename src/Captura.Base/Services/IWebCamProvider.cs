using System.Collections.Generic;

namespace Captura.Models
{
    public interface IWebCamProvider
    {
        IEnumerable<IWebcamItem> GetSources();
    }
}
