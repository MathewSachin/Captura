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
        
        string _output = string.Empty;
        KeyRecord _lastKeyRecord;
        #endregion
        
        /// <summary>
        /// Creates a new instance of <see cref="MouseKeyHook"/>.
        /// </summary>
        /// <param name="CaptureMouseClicks">Whether to capture Mouse CLicks.</param>
        /// <param name="CaptureKeystrokes">Whether to capture Keystrokes.</param>
        public MouseKeyHook(bool CaptureMouseClicks, bool CaptureKeystrokes)
        {
            _hook = Hook.GlobalEvents();

            if (CaptureMouseClicks)
            {
                _hook.MouseDown += (s, e) => _mouseClicked = true;

                _hook.MouseUp += (s, e) => _mouseClicked = false;
            }
            
            if (CaptureKeystrokes)
                _hook.KeyDown += OnKeyPressed;
        }
        
        void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (_output.Length > Settings.Instance.Keystrokes_MaxLength)
                _output = "";

            var keyRecord = new KeyRecord(e);
            string key;

            switch (e.KeyCode)
            {
                case Keys.Shift:
                case Keys.ShiftKey:
                case Keys.LShiftKey:
                case Keys.RShiftKey:

                    key = "Shift";
                    break;

                case Keys.Control:
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:

                    key = "Ctrl";
                    break;

                case Keys.Alt:
                case Keys.Menu:
                case Keys.LMenu:
                case Keys.RMenu:

                    key = "Alt";
                    break;

                case Keys.LWin:
                case Keys.RWin:
                    key = "Win";
                    break;

                default:
                    key = keyRecord.ToString();

                    if (keyRecord.Control || keyRecord.Alt)
                        _output = "";
                    break;
            }
            
            if (key.Length > 1 || (_lastKeyRecord != null &&
                                    (_lastKeyRecord.ToString().Length > 1 ||
                                    (keyRecord.TimeStamp - _lastKeyRecord.TimeStamp).TotalSeconds > Settings.Instance.Keystrokes_MaxSeconds)))
                _output = "";

            _output += key;

            _lastKeyRecord = keyRecord;
        }

        float GetLeft(float FullWidth, float TextWidth)
        {
            int x = Settings.Instance.Keystrokes_X;

            switch (Settings.Instance.Keystrokes_XAlign)
            {
                case Alignment.Start:
                    return x;

                case Alignment.End:
                    return FullWidth - x - TextWidth - 2 * Settings.Instance.Keystrokes_PaddingX;

                case Alignment.Center:
                    return FullWidth / 2 - x - TextWidth / 2 - Settings.Instance.Keystrokes_PaddingX;

                default:
                    return 0;
            }
        }

        float GetTop(float FullHeight, float TextHeight)
        {
            int y = Settings.Instance.Keystrokes_Y;

            switch (Settings.Instance.Keystrokes_YAlign)
            {
                case Alignment.Start:
                    return y;

                case Alignment.End:
                    return FullHeight - y - TextHeight - 2 * Settings.Instance.Keystrokes_PaddingY;

                case Alignment.Center:
                    return FullHeight / 2 - y - TextHeight / 2 - Settings.Instance.Keystrokes_PaddingY;

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
            if (_lastKeyRecord == null || (DateTime.Now - _lastKeyRecord.TimeStamp).TotalSeconds > Settings.Instance.Keystrokes_MaxSeconds)
                return;

            var keystrokeFont = new Font(FontFamily.GenericMonospace, Settings.Instance.Keystrokes_FontSize);

            var size = g.MeasureString(_output, keystrokeFont);

            int paddingX = Settings.Instance.Keystrokes_PaddingX, paddingY = Settings.Instance.Keystrokes_PaddingY;

            var rect = new RectangleF(GetLeft(g.VisibleClipBounds.Width, size.Width),
                GetTop(g.VisibleClipBounds.Height, size.Height),
                size.Width + 2 * paddingX,
                size.Height + 2 * paddingY);

            g.FillRoundedRectangle(new SolidBrush(Settings.Instance.KeystrokesRect_Color),
                rect,
                Settings.Instance.Keystrokes_CornerRadius);
            
            g.DrawString(_output,
                keystrokeFont,
                new SolidBrush(Settings.Instance.Keystrokes_Color),
                new RectangleF(rect.Left + paddingX, rect.Top + paddingY, size.Width, size.Height));

            var border = Settings.Instance.Keystrokes_Border;

            if (border > 0)
            {
                rect = new RectangleF(rect.Left - border / 2, rect.Top - border / 2, rect.Width + border, rect.Height + border);

                g.DrawRoundedRectangle(new Pen(Settings.Instance.Keystrokes_BorderColor, border),
                    rect,
                    Settings.Instance.Keystrokes_CornerRadius);
            }
        }

        void DrawClicks(Graphics g, Func<Point, Point> Transform)
        {
            if (_mouseClicked)
            {
                var _clickRadius = Settings.Instance.MouseClick_Radius;

                var curPos = MouseCursor.CursorPosition;

                if (Transform != null)
                    curPos = Transform(curPos);

                var d = _clickRadius * 2;
                
                var x = curPos.X - _clickRadius;
                var y = curPos.Y - _clickRadius;

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
