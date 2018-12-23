﻿using System;
using System.Drawing;

namespace Captura
{
    public interface IEditableFrame : IDisposable
    {
        float Width { get; }
        float Height { get; }

        void DrawImage(object Image, Rectangle? Region, int Opacity = 100);

        void FillRectangle(Color Color, RectangleF Rectangle);

        void FillRectangle(Color Color, RectangleF Rectangle, int CornerRadius);

        void DrawRectangle(Color Color, float StrokeWidth, RectangleF Rectangle);

        void DrawRectangle(Color Color, float StrokeWidth, RectangleF Rectangle, int CornerRadius);

        void FillEllipse(Color Color, RectangleF Rectangle);

        void DrawEllipse(Color Color, float StrokeWidth, RectangleF Rectangle);

        SizeF MeasureString(string Text, Font Font);

        void DrawString(string Text, Font Font, Color Color, RectangleF LayoutRectangle);

        IBitmapFrame GenerateFrame();
    }
}