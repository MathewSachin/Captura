using Gma.System.MouseKeyHook;
using Screna;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Captura.Models
{
    /// <summary>
    /// Draws Mouse Clicks and/or Keystrokes on an Image.
    /// </summary>
    public class MouseKeyHook : IOverlay
    {
        #region Fields
        readonly IKeyboardMouseEvents _hook;

        bool _mouseClicked;
        
        readonly KeyRecords _records;
        #endregion
        
        /// <summary>
        /// Creates a new instance of <see cref="MouseKeyHook"/>.
        /// </summary>
        /// <param name="CaptureMouseClicks">Whether to capture Mouse CLicks.</param>
        /// <param name="CaptureKeystrokes">Whether to capture Keystrokes.</param>
        public MouseKeyHook(bool CaptureMouseClicks, bool CaptureKeystrokes)
        {
            _records = new KeyRecords(Settings.Instance.Keystrokes_History_Count);

            _hook = Hook.GlobalEvents();

            if (CaptureMouseClicks)
            {
                _hook.MouseDown += (s, e) => _mouseClicked = true;

                _hook.MouseUp += (s, e) => _mouseClicked = false;
            }

            if (CaptureKeystrokes)
            {
                _hook.KeyDown += OnKeyDown;
                _hook.KeyUp += OnKeyUp;
            }
        }

        void OnKeyUp(object Sender, KeyEventArgs Args)
        {
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
            var record = new KeyRecord(Args);
            
            if (_records.Last == null)
            {
                _records.Add(record);

                return;
            }

            var elapsed = (record.TimeStamp - _records.Last.TimeStamp).TotalSeconds;

            if (record.Display.Length == 1
                && (_records.Last is DummyKeyRecord || _records.Last.Display.Length == 1)
                && record.Display.Length + _records.Last.Display.Length <= Settings.Instance.Keystrokes_MaxLength
                && elapsed <= Settings.Instance.Keystrokes_MaxSeconds)
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

        static float GetLeft(float FullWidth, float TextWidth)
        {
            var x = Settings.Instance.Keystrokes_X;

            switch (Settings.Instance.Keystrokes_XAlign)
            {
                case Alignment.Start:
                    return x;

                case Alignment.End:
                    return FullWidth - x - TextWidth - 2 * Settings.Instance.Keystrokes_PaddingX;

                case Alignment.Center:
                    return FullWidth / 2 + x - TextWidth / 2 - Settings.Instance.Keystrokes_PaddingX;

                default:
                    return 0;
            }
        }

        static float GetTop(float FullHeight, float TextHeight, float Offset = 0)
        {
            var y = Settings.Instance.Keystrokes_Y;

            switch (Settings.Instance.Keystrokes_YAlign)
            {
                case Alignment.Start:
                    return y + Offset;

                case Alignment.End:
                    return FullHeight - y - TextHeight - 2 * Settings.Instance.Keystrokes_PaddingY - Offset;

                case Alignment.Center:
                    return FullHeight / 2 + y - TextHeight / 2 - Settings.Instance.Keystrokes_PaddingY + Offset;

                default:
                    return 0;
            }
        }
        
        /// <summary>
        /// Draws overlay.
        /// </summary>
        public void Draw(Graphics g, Func<Point, Point> Transform = null)
        {
            DrawClicks(g, Transform);
            DrawKeys(g);
        }

        void DrawKeys(Graphics g)
        {
            if (_records.Last == null)
                return;

            var offsetY = 0f;
            var fontSize = Settings.Instance.Keystrokes_FontSize;
            byte opacity = 255;

            var index = 0;

            foreach (var keyRecord in _records)
            {
                ++index;

                if ((DateTime.Now - keyRecord.TimeStamp).TotalSeconds > _records.Size * Settings.Instance.Keystrokes_MaxSeconds)
                    continue;
                
                DrawKeys(g, keyRecord.Display, Math.Max(1, fontSize), opacity, offsetY);

                var keystrokeFont = new Font(FontFamily.GenericMonospace, fontSize);

                var height = g.MeasureString("A", keystrokeFont).Height;

                offsetY += height + Settings.Instance.Keystrokes_History_Spacing;

                offsetY += Settings.Instance.Keystrokes_PaddingY * 2 + Settings.Instance.Keystrokes_Border * 2;

                if (index == 1)
                {
                    fontSize -= 5;
                    opacity = 200;
                }
            }
        }

        static void DrawKeys(Graphics g, string Text, int FontSize, byte Opacity, float OffsetY)
        {
            var keystrokeFont = new Font(FontFamily.GenericMonospace, FontSize);

            var size = g.MeasureString(Text, keystrokeFont);

            int paddingX = Settings.Instance.Keystrokes_PaddingX, paddingY = Settings.Instance.Keystrokes_PaddingY;

            var rect = new RectangleF(GetLeft(g.VisibleClipBounds.Width, size.Width),
                GetTop(g.VisibleClipBounds.Height, size.Height, OffsetY),
                size.Width + 2 * paddingX,
                size.Height + 2 * paddingY);
            
            g.FillRoundedRectangle(new SolidBrush(Color.FromArgb(Opacity, Settings.Instance.KeystrokesRect_Color)),
                rect,
                Settings.Instance.Keystrokes_CornerRadius);
            
            g.DrawString(Text,
                keystrokeFont,
                new SolidBrush(Color.FromArgb(Opacity, Settings.Instance.Keystrokes_Color)),
                new RectangleF(rect.Left + paddingX, rect.Top + paddingY, size.Width, size.Height));

            var border = Settings.Instance.Keystrokes_Border;

            if (border > 0)
            {
                rect = new RectangleF(rect.Left - border / 2, rect.Top - border / 2, rect.Width + border, rect.Height + border);

                g.DrawRoundedRectangle(new Pen(Color.FromArgb(Opacity, Settings.Instance.Keystrokes_BorderColor), border),
                    rect,
                    Settings.Instance.Keystrokes_CornerRadius);
            }
        }

        void DrawClicks(Graphics g, Func<Point, Point> Transform)
        {
            if (_mouseClicked)
            {
                var clickRadius = Settings.Instance.MouseClick_Radius;

                var curPos = MouseCursor.CursorPosition;

                if (Transform != null)
                    curPos = Transform(curPos);

                var d = clickRadius * 2;
                
                var x = curPos.X - clickRadius;
                var y = curPos.Y - clickRadius;

                g.FillEllipse(new SolidBrush(Settings.Instance.MouseClick_Color), x, y, d, d);

                var border = Settings.Instance.MouseClick_Border;

                if (border > 0)
                {
                    x -= border / 2;
                    y -= border / 2;
                    d += border;

                    g.DrawEllipse(new Pen(Settings.Instance.MouseClick_BorderColor, border), x, y, d, d);
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
