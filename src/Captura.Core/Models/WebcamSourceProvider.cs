using System;
using Captura.ViewModels;
using Screna;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WebcamSourceProvider : NotifyPropertyChanged, IVideoSourceProvider
    {
        readonly ILocalizationProvider _loc;
        readonly WebcamModel _webcamModel;

        public WebcamSourceProvider(ILocalizationProvider Loc,
            IIconSet Icons,
            WebcamModel WebcamModel)
        {
            _loc = Loc;
            _webcamModel = WebcamModel;
            Icon = Icons.Webcam;
            Source = new WebcamVideoItem(WebcamModel);

            Loc.LanguageChanged += L => RaisePropertyChanged(nameof(Name));
        }

        public string Name => _loc.WebCam;

        public string Description { get; } = "Record Webcam only";
        public string Icon { get; }

        public IVideoItem Source { get; }

        public IBitmapImage Capture(bool IncludeCursor)
        {
            return _webcamModel.WebcamCapture?.Capture(GraphicsBitmapLoader.Instance);
        }

        public bool OnSelect() => true;

        public void OnUnselect()
        {
        }

        public event Action UnselectRequested;

        public string Serialize() => "";

        public bool Deserialize(string Serialized) => true;

        public bool ParseCli(string Arg) => Arg == "webcam";
    }
}