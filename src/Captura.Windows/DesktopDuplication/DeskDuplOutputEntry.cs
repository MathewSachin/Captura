﻿using SharpDX;

namespace DesktopDuplication
{
    class DeskDuplOutputEntry
    {
        public Point Location { get; set; }

        public DuplCapture DuplCapture { get; set; }

        public DxMousePointer MousePointer { get; set; }
    }
}
