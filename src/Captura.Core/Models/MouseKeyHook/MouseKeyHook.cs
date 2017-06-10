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
                _hook.MouseDown += OnMouseDown;
            
            if (CaptureKeystrokes)
                _hook.KeyDown += OnKeyPressed;
        }

        void OnMouseDown(object sender, EventArgs e) => _mouseClicked = true;

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
                                    (keyRecord.TimeStamp - _lastKeyRecord.TimeStamp).TotalSeconds > 2)))
                _output = "";

            _output += key;

            _lastKeyRecord = keyRecord;
        }
        
        /// <summary>
        /// Draws overlay.
        /// </summary>
        public void Draw(Graphics g, Point Offset = default(Point))
        {
            if (_mouseClicked)
            {
                var _clickRadius = Settings.Instance.MouseClick_Radius;

                var curPos = MouseCursor.CursorPosition;
                var d = _clickRadius * 2;

                g.FillEllipse(new SolidBrush(Settings.Instance.MouseClick_Color),
                    curPos.X - _clickRadius - Offset.X,
                    curPos.Y - _clickRadius - Offset.Y,
                    d, d);

                _mouseClicked = false;
            }
            
            if (_lastKeyRecord == null || (DateTime.Now - _lastKeyRecord.TimeStamp).TotalSeconds > 2)
                return;

            int left = 80, bottom = 200;

            float height = g.VisibleClipBounds.Height;

            var keystrokeFont = new Font(FontFamily.GenericMonospace, Settings.Instance.Keystrokes_FontSize);

            var size = g.MeasureString(_output, keystrokeFont);

            int paddingX = Settings.Instance.Keystrokes_PaddingX,
                paddingY = Settings.Instance.Keystrokes_PaddingY;

            var rectHeight = size.Height + 2 * paddingY;
            var rect = new RectangleF(left, height - bottom - rectHeight, size.Width + 2 * paddingX, rectHeight);

            g.FillRoundedRectangle(new SolidBrush(Settings.Instance.KeystrokesRect_Color),
                rect,
                Settings.Instance.Keystrokes_CornerRadius);
            
            g.DrawString(_output,
                keystrokeFont,
                new SolidBrush(Settings.Instance.Keystrokes_Color),
                new RectangleF(left + paddingX, rect.Top + paddingY, size.Width, size.Height));
        }

        /// <summary>
        /// Frees all resources used by this object.
        /// </summary>
        public void Dispose()
        {
            _hook.MouseDown -= OnMouseDown;
            _hook.KeyDown -= OnKeyPressed;

            _hook?.Dispose();
        }
    }
}
