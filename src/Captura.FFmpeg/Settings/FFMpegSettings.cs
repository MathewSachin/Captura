﻿using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Captura
{
    public class FFmpegSettings : PropertyStore
    {
        public string FolderPath
        {
            get => Get<string>();
            set => Set(value);
        }

        public string GetFolderPath()
        {
            var path = FolderPath;

            if (!string.IsNullOrWhiteSpace(path))
            {
                path = path.Replace(ServiceProvider.CapturaPathConstant, ServiceProvider.AppDir);
            }
            else
            {
                var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                path = Path.Combine(localAppDataPath, nameof(Captura));
            }

            return path;
        }

        public string TwitchKey
        {
            get => Get("");
            set => Set(value);
        }

        public string YouTubeLiveKey
        {
            get => Get("");
            set => Set(value);
        }

        public ObservableCollection<CustomFFmpegCodec> CustomCodecs { get; } = new ObservableCollection<CustomFFmpegCodec>();

        public string CustomStreamingUrl
        {
            get => Get("rtmp://");
            set => Set(value);
        }

        public bool Resize
        {
            get => Get<bool>();
            set => Set(value);
        }

        public int ResizeWidth
        {
            get => Get(640);
            set => Set(value);
        }

        public int ResizeHeight
        {
            get => Get(480);
            set => Set(value);
        }

        public X264Settings X264 { get; } = new X264Settings();
    }
}