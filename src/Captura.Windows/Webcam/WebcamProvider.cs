using System.Collections.Generic;
using System.Linq;

namespace Captura.Webcam
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class WebcamProvider : NotifyPropertyChanged, IWebCamProvider
    {
        public IEnumerable<IWebcamItem> GetSources()
        {
            return Filter.VideoInputDevices.Select(M => new WebcamItem(M));
        }
    }
}
