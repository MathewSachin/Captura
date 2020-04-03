using Captura.Video;

namespace Captura.Webcam
{
    public class WebcamVideoItem : NotifyPropertyChanged, IVideoItem
    {
        readonly WebcamModel _webcamModel;

        public WebcamVideoItem(WebcamModel WebcamModel)
        {
            _webcamModel = WebcamModel;

            _webcamModel.PropertyChanged += (S, E) => RaisePropertyChanged(nameof(Name));
        }

        public string Name => _webcamModel.SelectedCam?.Name;

        public IImageProvider GetImageProvider(bool IncludeCursor)
        {
            return new WebcamImageProvider(_webcamModel);
        }
    }
}