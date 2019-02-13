using System.Collections.Generic;
using System.Linq;
using Captura.Webcam;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WebcamProvider : NotifyPropertyChanged, IWebCamProvider
    {
        public IEnumerable<IWebcamItem> GetSources()
        {
            return Filter.VideoInputDevices.Select(M => new WebcamItem(M));
        }
    }
}
