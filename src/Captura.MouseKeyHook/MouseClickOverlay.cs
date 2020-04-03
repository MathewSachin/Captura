using System;
using System.Drawing;
using System.Windows.Forms;
using Captura.Video;

namespace Captura.MouseKeyHook
{
    public class MouseClickOverlay : IOverlay
    {
        readonly MouseClickSettings _settings;
        bool _clicked;
        MouseButtons _buttons;

        public MouseClickOverlay(IMouseKeyHook Hook,
            MouseClickSettings Settings)
        {
            _settings = Settings;

            Hook.MouseDown += (S, E) =>
            {
                _clicked = true;

                _buttons = E.Button;
            };

            Hook.MouseUp += (S, E) => _clicked = false;
        }

        public void Dispose() { }

        public void Draw(IEditableFrame Editor, Func<Point, Point> PointTransform = null)
        {
            if (!_settings.Display)
                return;

            if (_clicked && _currentMouseRatio < MouseRatioMax)
            {
                _currentMouseRatio += MouseRatioDeltaUp;

                if (_currentMouseRatio > MouseRatioMax)
                {
                    _currentMouseRatio = MouseRatioMax;
                }
            }
            else if (!_clicked && _currentMouseRatio > MouseRatioMin)
            {
                _currentMouseRatio -= MouseRatioDeltaDown;

                if (_currentMouseRatio < MouseRatioMin)
                {
                    _currentMouseRatio = MouseRatioMin;
                }
            }

            if (_currentMouseRatio > MouseRatioMin)
            {
                var clickRadius = _settings.Radius * _currentMouseRatio;

                var platformServices = ServiceProvider.Get<IPlatformServices>();

                var curPos = platformServices.CursorPosition;

                if (PointTransform != null)
                    curPos = PointTransform(curPos);

                var d = clickRadius * 2;

                var x = curPos.X - clickRadius;
                var y = curPos.Y - clickRadius;

                var color = GetClickCircleColor();

                color = Color.FromArgb(ToByte(color.A * _currentMouseRatio), color);

                Editor.FillEllipse(color, new RectangleF(x, y, d, d));

                var border = _settings.BorderThickness * _currentMouseRatio;

                if (border > 0)
                {
                    x -= border / 2f;
                    y -= border / 2f;
                    d += border;

                    var borderColor = _settings.BorderColor;

                    borderColor = Color.FromArgb(ToByte(borderColor.A * _currentMouseRatio), borderColor);

                    Editor.DrawEllipse(borderColor, border, new RectangleF(x, y, d, d));
                }
            }
        }

        Color GetClickCircleColor()
        {
            if (_buttons.HasFlag(MouseButtons.Right))
            {
                return _settings.RightClickColor;
            }

            if (_buttons.HasFlag(MouseButtons.Middle))
            {
                return _settings.MiddleClickColor;
            }

            return _settings.Color;
        }

        static byte ToByte(double Value)
        {
            if (Value > 255)
                return 255;

            if (Value < 0)
                return 0;

            return (byte)Value;
        }

        float _currentMouseRatio;
        const float MouseRatioDeltaUp = 0.9f;
        const float MouseRatioDeltaDown = 0.25f;
        const float MouseRatioMin = 0.6f;
        const float MouseRatioMax = 1.2f;
    }
}