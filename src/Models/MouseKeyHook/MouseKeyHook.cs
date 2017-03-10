using Gma.System.MouseKeyHook;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Screna
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

        readonly Brush _clickBrush = new SolidBrush(Color.FromArgb(100, Color.DarkGray));
        readonly double _clickRadius = 25;
        readonly Font _keyStrokeFont = new Font(FontFamily.GenericMonospace, 20);
        readonly Brush _keyStrokeBrush = Brushes.Black;
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
            if (_output.Length > 15)
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
                var curPos = MouseCursor.CursorPosition;
                var d = (float)(_clickRadius * 2);
                
                g.FillEllipse(_clickBrush,
                    curPos.X - (float)_clickRadius - Offset.X,
                    curPos.Y - (float)_clickRadius - Offset.Y,
                    d, d);

                _mouseClicked = false;
            }
            
            if (_lastKeyRecord == null || (DateTime.Now - _lastKeyRecord.TimeStamp).TotalSeconds > 2)
                return;
            
            var keyStrokeRect = new Rectangle(80, (int)g.VisibleClipBounds.Height - 200, (int)(_output.Length * _keyStrokeFont.Size + 5), 35);
            
            g.FillRectangle(_clickBrush, keyStrokeRect);

            g.DrawString(_output,
                _keyStrokeFont,
                _keyStrokeBrush,
                keyStrokeRect);
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
