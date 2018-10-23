﻿using Captura.Models;

namespace Captura
{
    public class ScreenShotSettings : PropertyStore
    {
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
            get => Get(400);
            set => Set(value);
        }

        public bool FlipHorizontal
        {
            get => Get<bool>();
            set => Set(value);
        }

        public bool FlipVertical
        {
            get => Get<bool>();
            set => Set(value);
        }

        public RotateBy RotateBy
        {
            get => Get<RotateBy>();
            set => Set(value);
        }

        public string ImageFormat
        {
            get => Get("Png");
            set => Set(value);
        }

        public string[] SaveTargets
        {
            get => Get(new []{ "Disk" });
            set => Set(value);
        }
    }
}