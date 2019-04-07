using System;
using System.Drawing;
using System.Windows.Forms;

namespace Captura.Models
{
    class KeyStep : IRecordStep
    {
        public string Text { get; private set; }

        readonly KeymapViewModel _keymap;
        readonly KeystrokesSettings _settings;
        readonly bool _mergeable;
        int _repeat;

        public KeyStep(KeystrokesSettings Settings,
            KeyEventArgs Args,
            KeymapViewModel Keymap)
        {
            _settings = Settings;
            _keymap = Keymap;

            Text = $"{GetModifierString(Args)}{Args.KeyCode}";

            _mergeable = Text.Length == 1;
        }

        string GetModifierString(KeyEventArgs Args)
        {
            var result = "";

            if (Args.Control)
            {
                result += $"{_keymap.Control} + ";
            }

            if (Args.Shift)
            {
                result += $"{_keymap.Shift} + ";
            }

            if (Args.Alt)
            {
                result += $"{_keymap.Alt} + ";
            }

            return result;
        }

        public void Draw(IEditableFrame Editor, Func<Point, Point> PointTransform)
        {
            var repeat = (_repeat == 0 ? "" : $"x {_repeat + 1}");

            using (var font = Editor.GetFont(_settings.FontFamily, _settings.FontSize))
            {
                var text = $"{Text}{repeat}";

                var size = Editor.MeasureString(text, font);

                int paddingX = _settings.HorizontalPadding,
                    paddingY = _settings.VerticalPadding;

                var rect = new RectangleF(KeyOverlay.GetLeft(_settings, Editor.Width, size.Width),
                    KeyOverlay.GetTop(_settings, Editor.Height, size.Height),
                    size.Width + 2 * paddingX,
                    size.Height + 2 * paddingY);

                Editor.FillRectangle(_settings.BackgroundColor,
                    rect,
                    _settings.CornerRadius);

                Editor.DrawString(text,
                    font,
                    _settings.FontColor,
                    new RectangleF(rect.Left + paddingX, rect.Top + paddingY, size.Width, size.Height));

                var border = _settings.BorderThickness;

                if (border > 0)
                {
                    rect = new RectangleF(rect.Left - border / 2f, rect.Top - border / 2f, rect.Width + border, rect.Height + border);

                    Editor.DrawRectangle(_settings.BorderColor,
                        border,
                        rect,
                        _settings.CornerRadius);
                }
            }
        }

        public bool Merge(IRecordStep NextStep)
        {
            if (!_mergeable)
                return false;

            if (NextStep is KeyStep nextStep)
            {
                if (_repeat == 0 && nextStep.Text.Length == 1)
                {
                    Text += nextStep.Text;
                    return true;
                }

                if (Text == nextStep.Text)
                {
                    ++_repeat;
                    return true;
                }
            }

            return false;
        }
    }
}
