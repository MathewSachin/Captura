﻿using System;
using Captura.ViewModels;

namespace Captura.Models
{
    public class WebcamSourceProvider : NotifyPropertyChanged, IVideoSourceProvider
    {
        readonly LanguageManager _loc;

        public WebcamSourceProvider(LanguageManager Loc,
            IIconSet Icons,
            WebcamModel WebcamModel)
        {
            _loc = Loc;
            Icon = Icons.Webcam;
            Source = new WebcamVideoItem(WebcamModel);

            Loc.LanguageChanged += L => RaisePropertyChanged(nameof(Name));
        }

        public string Name => _loc.WebCam;

        public string Description { get; } = "Record Webcam only";
        public string Icon { get; }

        public IVideoItem Source { get; }

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