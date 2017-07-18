﻿using System.Drawing;
using Screna;
using DesktopDuplication;

namespace Captura.Models
{
    public class DeskDuplImageProvider : IImageProvider
    {
        DesktopDuplicator _dupl;
        int _monitor;
        bool _includeCursor;

        public DeskDuplImageProvider(int Monitor, bool IncludeCursor)
        {
            _monitor = Monitor;
            _includeCursor = IncludeCursor;

            Width = WindowProvider.DesktopRectangle.Width;
            Height = WindowProvider.DesktopRectangle.Height;
        }
        
        public int Height { get; }

        public int Width { get; }

        public Bitmap Capture()
        {
            try
            {
                return _dupl.GetLatestFrame();
            }
            catch
            {
                _dupl = new DesktopDuplicator(WindowProvider.DesktopRectangle, _includeCursor, _monitor);

                return Capture();
            }
        }

        public void Dispose()
        {
            _dupl?.Dispose();
        }
    }
}
