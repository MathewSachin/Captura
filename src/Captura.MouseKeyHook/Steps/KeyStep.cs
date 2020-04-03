using System;
using System.Drawing;

namespace Captura.MouseKeyHook.Steps
{
    class KeyStep : IRecordStep
    {
        public string Text { get; private set; }

        readonly KeystrokesSettings _settings;
        readonly bool _mergeable;
        int _repeat;

        public KeyStep(KeystrokesSettings Settings,
            KeyRecord KeyRecord)
        {
            _settings = Settings;

            // TODO: Handle Modifiers keys on KeyUp like in KeyOverlay
            Text = KeyRecord.Display;

            _mergeable = Text.Length == 1;
        }

        public static void DrawString(IEditableFrame Editor, string Text, KeystrokesSettings Settings)
        {
            if (string.IsNullOrWhiteSpace(Text))
                return;

            using var font = Editor.GetFont(Settings.FontFamily, Settings.FontSize);
            var size = Editor.MeasureString(Text, font);

            int paddingX = Settings.HorizontalPadding,
                paddingY = Settings.VerticalPadding;

            var rect = new RectangleF(KeyOverlay.GetLeft(Settings, Editor.Width, size.Width),
                KeyOverlay.GetTop(Settings, Editor.Height, size.Height),
                size.Width + 2 * paddingX,
                size.Height + 2 * paddingY);

            Editor.FillRectangle(Settings.BackgroundColor,
                rect,
                Settings.CornerRadius);

            Editor.DrawString(Text,
                font,
                Settings.FontColor,
                new RectangleF(rect.Left + paddingX, rect.Top + paddingY, size.Width, size.Height));

            var border = Settings.BorderThickness;

            if (border > 0)
            {
                rect = new RectangleF(rect.Left - border / 2f, rect.Top - border / 2f, rect.Width + border, rect.Height + border);

                Editor.DrawRectangle(Settings.BorderColor,
                    border,
                    rect,
                    Settings.CornerRadius);
            }
        }

        public void Draw(IEditableFrame Editor, Func<Point, Point> PointTransform)
        {
            var repeat = (_repeat == 0 ? "" : $" x {_repeat + 1}");

            var text = $"{Text}{repeat}";

            DrawString(Editor, text, _settings);
        }

        public bool Merge(IRecordStep NextStep)
        {
            if (NextStep is KeyStep nextStep)
            {
                if (_repeat == 0 && _mergeable && nextStep.Text.Length == 1)
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
