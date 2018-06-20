using Gma.System.MouseKeyHook;
using Screna;
using System;
using System.Drawing;
using System.Windows.Forms;
using ManagedBass;

namespace Captura.Models
{
    /// <summary>
    /// Draws Mouse Clicks and/or Keystrokes on an Image.
    /// </summary>
    public class MouseKeyHook : IOverlay
    {
        #region Fields
        readonly IKeyboardMouseEvents _hook;
        readonly MouseClickSettings _mouseClickSettings;
        readonly KeystrokesSettings _keystrokesSettings;

        bool _mouseClicked;
        MouseButtons _mouseButtons;
        
        readonly KeyRecords _records;
        #endregion
        
        /// <summary>
        /// Creates a new instance of <see cref="MouseKeyHook"/>.
        /// </summary>
        public MouseKeyHook(MouseClickSettings MouseClickSettings, KeystrokesSettings KeystrokesSettings)
        {
            _mouseClickSettings = MouseClickSettings;
            _keystrokesSettings = KeystrokesSettings;
            
            _hook = Hook.GlobalEvents();
            
            _hook.MouseDown += (S, E) =>
            {
                _mouseClicked = true;

                _mouseButtons = E.Button;
            };

            _hook.MouseUp += (S, E) => _mouseClicked = false;
            
            _records = new KeyRecords(KeystrokesSettings.HistoryCount);

            _hook.KeyDown += OnKeyDown;
            _hook.KeyUp += OnKeyUp;
        }

        void OnKeyUp(object Sender, KeyEventArgs Args)
        {
            if (!_keystrokesSettings.Display)
            {
                _records.Clear();

                return;
            }

            var record = new KeyRecord(Args);

            if (record.Display == "Ctrl" || record.Display == "Alt" || record.Display == "Shift")
            {
                if (_records.Last?.Display == record.Display)
                {
                    _records.Last = new RepeatKeyRecord(record);
                }
                else if (_records.Last is RepeatKeyRecord repeat && repeat.Repeated.Display == record.Display)
                {
                    repeat.Increment();
                }
                else _records.Add(record);
            }
        }

        void OnKeyDown(object Sender, KeyEventArgs Args)
        {
            if (!_keystrokesSettings.Display)
            {
                _records.Clear();

                return;
            }

            var record = new KeyRecord(Args);
            
            if (_records.Last == null)
            {
                _records.Add(record);

                return;
            }

            var elapsed = (record.TimeStamp - _records.Last.TimeStamp).TotalSeconds;

            if (record.Display.Length == 1
                && (_records.Last is DummyKeyRecord || _records.Last.Display.Length == 1)
                && record.Display.Length + _records.Last.Display.Length <= _keystrokesSettings.MaxTextLength
                && elapsed <= _keystrokesSettings.Timeout)
            {
                _records.Last = new DummyKeyRecord(_records.Last.Display + record.Display);
            }
            else if (record.Display == "Ctrl" || record.Display == "Alt" || record.Display == "Shift")
            {
                // Handle in OnKeyUp
            }
            else if (_records.Last is KeyRecord keyRecord && keyRecord.Key == record.Key)
            {
                _records.Last = new RepeatKeyRecord(record);
            }
            else if (_records.Last is RepeatKeyRecord repeatRecord && repeatRecord.Repeated.Key == record.Key)
            {
                repeatRecord.Increment();
            }
            else
            {
                _records.Add(record);
            }
        }

        static float GetLeft(TextOverlaySettings KeystrokesSettings, float FullWidth, float TextWidth)
        {
            var x = KeystrokesSettings.X;
            var padding = KeystrokesSettings.HorizontalPadding;

            switch (KeystrokesSettings.HorizontalAlignment)
            {
                case Alignment.Start:
                    return x;

                case Alignment.End:
                    return FullWidth - x - TextWidth - 2 * padding;

                case Alignment.Center:
                    return FullWidth / 2 + x - TextWidth / 2 - padding;

                default:
                    return 0;
            }
        }

        static float GetTop(TextOverlaySettings KeystrokesSettings, float FullHeight, float TextHeight, float Offset = 0)
        {
            var y = KeystrokesSettings.Y;
            var padding = KeystrokesSettings.VerticalPadding;

            switch (KeystrokesSettings.VerticalAlignment)
            {
                case Alignment.Start:
                    return y + Offset;

                case Alignment.End:
                    return FullHeight - y - TextHeight - 2 * padding - Offset;

                case Alignment.Center:
                    return FullHeight / 2 + y - TextHeight / 2 - padding + Offset;

                default:
                    return 0;
            }
        }
        
        /// <summary>
        /// Draws overlay.
        /// </summary>
        public void Draw(Graphics G, Func<Point, Point> Transform = null)
        {
            if (_mouseClickSettings.Display)
                DrawClicks(G, Transform);

            if (_keystrokesSettings.Display)
                DrawKeys(G);
        }

        void DrawKeys(Graphics G)
        {
            if (_records?.Last == null)
                return;

            var offsetY = 0f;
            var fontSize = _keystrokesSettings.FontSize;
            byte opacity = 255;

            var index = 0;

            foreach (var keyRecord in _records)
            {
                ++index;

                if ((DateTime.Now - keyRecord.TimeStamp).TotalSeconds > _records.Size * _keystrokesSettings.Timeout)
                    continue;
                
                DrawKeys(_keystrokesSettings, G, keyRecord.Display, Math.Max(1, fontSize), opacity, offsetY);

                var keystrokeFont = new Font(FontFamily.GenericMonospace, fontSize);

                var height = G.MeasureString("A", keystrokeFont).Height;

                offsetY += height + _keystrokesSettings.HistorySpacing;

                offsetY += _keystrokesSettings.VerticalPadding * 2 + _keystrokesSettings.BorderThickness * 2;

                if (index == 1)
                {
                    fontSize -= 5;
                    opacity = 200;
                }
            }
        }

        static void DrawKeys(KeystrokesSettings KeystrokesSettings, Graphics G, string Text, int FontSize, byte Opacity, float OffsetY)
        {
            var keystrokeFont = new Font(FontFamily.GenericMonospace, FontSize);

            var size = G.MeasureString(Text, keystrokeFont);

            int paddingX = KeystrokesSettings.HorizontalPadding, paddingY = KeystrokesSettings.VerticalPadding;

            var rect = new RectangleF(GetLeft(KeystrokesSettings, G.VisibleClipBounds.Width, size.Width),
                GetTop(KeystrokesSettings, G.VisibleClipBounds.Height, size.Height, OffsetY),
                size.Width + 2 * paddingX,
                size.Height + 2 * paddingY);
            
            G.FillRoundedRectangle(new SolidBrush(Color.FromArgb(Opacity, KeystrokesSettings.BackgroundColor)),
                rect,
                KeystrokesSettings.CornerRadius);
            
            G.DrawString(Text,
                keystrokeFont,
                new SolidBrush(Color.FromArgb(Opacity, KeystrokesSettings.FontColor)),
                new RectangleF(rect.Left + paddingX, rect.Top + paddingY, size.Width, size.Height));

            var border = KeystrokesSettings.BorderThickness;

            if (border > 0)
            {
                rect = new RectangleF(rect.Left - border / 2f, rect.Top - border / 2f, rect.Width + border, rect.Height + border);

                G.DrawRoundedRectangle(new Pen(Color.FromArgb(Opacity, KeystrokesSettings.BorderColor), border),
                    rect,
                    KeystrokesSettings.CornerRadius);
            }
        }

        Color GetClickCirleColor()
        {
            if (_mouseButtons.HasFlag(MouseButtons.Right))
            {
                return _mouseClickSettings.RightClickColor;
            }

            if (_mouseButtons.HasFlag(MouseButtons.Middle))
            {
                return _mouseClickSettings.MiddleClickColor;
            }

            return _mouseClickSettings.Color;
        }

        float _currentMouseRatio;
        const float MouseRatioDeltaUp = 0.9f;
        const float MouseRatioDeltaDown = 0.25f;
        const float MouseRatioMin = 0.6f;
        const float MouseRatioMax = 1.2f;

        void DrawClicks(Graphics G, Func<Point, Point> Transform)
        {
            if (_mouseClicked && _currentMouseRatio < MouseRatioMax)
            {
                _currentMouseRatio += MouseRatioDeltaUp;

                if (_currentMouseRatio > MouseRatioMax)
                {
                    _currentMouseRatio = MouseRatioMax;
                }
            }
            else if (!_mouseClicked && _currentMouseRatio > MouseRatioMin)
            {
                _currentMouseRatio -= MouseRatioDeltaDown;

                if (_currentMouseRatio < MouseRatioMin)
                {
                    _currentMouseRatio = MouseRatioMin;
                }
            }

            if (_currentMouseRatio > MouseRatioMin)
            {
                var clickRadius = _mouseClickSettings.Radius * _currentMouseRatio;

                var curPos = MouseCursor.CursorPosition;

                if (Transform != null)
                    curPos = Transform(curPos);

                var d = clickRadius * 2;
                
                var x = curPos.X - clickRadius;
                var y = curPos.Y - clickRadius;

                var color = GetClickCirleColor();

                color = Color.FromArgb((byte) (color.A * _currentMouseRatio).Clip(0, 255), color);

                G.FillEllipse(new SolidBrush(color), x, y, d, d);

                var border = _mouseClickSettings.BorderThickness * _currentMouseRatio;

                if (border > 0)
                {
                    x -= border / 2f;
                    y -= border / 2f;
                    d += border;

                    var borderColor = _mouseClickSettings.BorderColor;

                    borderColor = Color.FromArgb((byte) (borderColor.A * _currentMouseRatio).Clip(0, 255), borderColor);

                    G.DrawEllipse(new Pen(borderColor, border), x, y, d, d);
                }
            }
        }

        /// <summary>
        /// Frees all resources used by this object.
        /// </summary>
        public void Dispose()
        {
            _hook?.Dispose();
        }
    }
}
