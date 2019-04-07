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
        int _repeat;

        public KeyStep(KeystrokesSettings Settings,
            KeyEventArgs Args,
            KeymapViewModel Keymap)
        {
            _settings = Settings;
            _keymap = Keymap;

            Text = $"{GetModifierString(Args)}{Args.KeyCode}";
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

                var rect = new RectangleF(10, 10, size.Width, size.Height);

                Editor.DrawString(text, font, _settings.FontColor, rect);
            }
        }

        public bool Merge(IRecordStep NextStep)
        {
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
