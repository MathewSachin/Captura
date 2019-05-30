﻿using DesktopDuplication;
using SharpDX.DXGI;
using System;
using System.Drawing;

namespace Captura.Models
{
    class DeskDuplImageProvider : IImageProvider
    {
        readonly DesktopDuplicator _dupl;

        public DeskDuplImageProvider(Output1 Output, bool IncludeCursor, IPreviewWindow PreviewWindow)
        {
            var bounds = Output.Description.DesktopBounds;

            Width = bounds.Right - bounds.Left;
            Height = bounds.Bottom - bounds.Top;

            PointTransform = P => new Point(P.X - bounds.Left, P.Y - bounds.Top);

            _dupl = new DesktopDuplicator(IncludeCursor, Output, PreviewWindow);
        }

        public int Height { get; }

        public int Width { get; }

        public Func<Point, Point> PointTransform { get; }
        
        public IEditableFrame Capture()
        {
            return _dupl.Capture();
        }

        public void Dispose()
        {
            _dupl?.Dispose();
        }

        public Type EditorType { get; } = typeof(Direct2DEditor);
    }
}
