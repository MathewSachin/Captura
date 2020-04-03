using System;
using System.Drawing;
using System.Windows.Forms;
using Captura.Video;

namespace Captura.MouseKeyHook
{
    public class KeyOverlay : IOverlay
    {
        readonly KeystrokesSettings _settings;
        readonly KeyRecords _records;
        readonly KeymapViewModel _keymap;
        bool _modifierSingleDown;

        public KeyOverlay(IMouseKeyHook Hook,
            KeystrokesSettings Settings,
            KeymapViewModel Keymap)
        {
            _settings = Settings;
            _keymap = Keymap;

            _records = new KeyRecords(Settings.HistoryCount);

            Hook.KeyDown += OnKeyDown;
            Hook.KeyUp += OnKeyUp;
        }

        public void Dispose() { }

        public void Draw(IEditableFrame Editor, Func<Point, Point> PointTransform = null)
        {
            if (!_settings.Display)
                return;

            if (_records?.Last == null)
                return;

            var offsetY = 0f;
            var fontSize = _settings.FontSize;
            byte opacity = 255;

            var index = 0;

            foreach (var keyRecord in _records)
            {
                if ((DateTime.Now - keyRecord.TimeStamp).TotalSeconds > _records.Size * _settings.Timeout)
                    continue;

                using (var font = Editor.GetFont(_settings.FontFamily, Math.Max(1, fontSize)))
                {
                    DrawKeys(_settings, Editor, keyRecord.Display, font, opacity, offsetY);

                    var height = Editor.MeasureString("A", font).Height;

                    offsetY += height + _settings.HistorySpacing;

                    offsetY += _settings.VerticalPadding * 2 + _settings.BorderThickness * 2;
                }

                if (++index == 1)
                {
                    fontSize = Math.Max(1, fontSize - 5);
                    opacity = 200;
                }
            }
        }

        static Color ApplyOpacity(Color Color, int Opacity)
        {
            var alpha = (int)Math.Round((Opacity / 255.0) * (Color.A / 255.0) * 255);

            return Color.FromArgb(alpha, Color);
        }

        static void DrawKeys(KeystrokesSettings KeystrokesSettings, IEditableFrame Editor, string Text, IFont Font, byte Opacity, float OffsetY)
        {
            var size = Editor.MeasureString(Text, Font);

            int paddingX = KeystrokesSettings.HorizontalPadding, paddingY = KeystrokesSettings.VerticalPadding;

            var rect = new RectangleF(GetLeft(KeystrokesSettings, Editor.Width, size.Width),
                GetTop(KeystrokesSettings, Editor.Height, size.Height, OffsetY),
                size.Width + 2 * paddingX,
                size.Height + 2 * paddingY);

            Editor.FillRectangle(ApplyOpacity(KeystrokesSettings.BackgroundColor, Opacity),
                rect,
                KeystrokesSettings.CornerRadius);

            Editor.DrawString(Text,
                Font,
                ApplyOpacity(KeystrokesSettings.FontColor, Opacity),
                new RectangleF(rect.Left + paddingX, rect.Top + paddingY, size.Width, size.Height));

            var border = KeystrokesSettings.BorderThickness;

            if (border > 0)
            {
                rect = new RectangleF(rect.Left - border / 2f, rect.Top - border / 2f, rect.Width + border, rect.Height + border);

                Editor.DrawRectangle(ApplyOpacity(KeystrokesSettings.BorderColor, Opacity), border,
                    rect,
                    KeystrokesSettings.CornerRadius);
            }
        }

        public static float GetLeft(TextOverlaySettings KeystrokesSettings, float FullWidth, float TextWidth)
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

        public static float GetTop(TextOverlaySettings KeystrokesSettings, float FullHeight, float TextHeight, float Offset = 0)
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

        void OnKeyUp(object Sender, KeyEventArgs Args)
        {
            if (!_settings.Display)
            {
                _records.Clear();

                return;
            }

            var record = new KeyRecord(Args, _keymap);

            var display = record.Display;

            if (display == _keymap.Control
                || display == _keymap.Alt
                || display == _keymap.Shift)
            {
                if (_records.Last?.Display == display)
                {
                    _records.Last = new RepeatKeyRecord(record, _settings);
                }
                else if (_records.Last is RepeatKeyRecord repeat && repeat.Repeated.Display == display)
                {
                    repeat.Increment();
                }
                else if (_modifierSingleDown)
                {
                    _records.Add(record);
                }
            }
        }

        void OnKeyDown(object Sender, KeyEventArgs Args)
        {
            if (!_settings.Display)
            {
                _records.Clear();

                return;
            }

            _modifierSingleDown = false;

            var record = new KeyRecord(Args, _keymap);

            if (_records.Last == null)
            {
                _records.Add(record);

                return;
            }

            var elapsed = (record.TimeStamp - _records.Last.TimeStamp).TotalSeconds;

            var display = record.Display;
            var lastDisplay = _records.Last.Display;

            if (display.Length == 1
                && (_records.Last is DummyKeyRecord || lastDisplay.Length == 1)
                && display.Length + lastDisplay.Length <= _settings.MaxTextLength
                && elapsed <= _settings.Timeout)
            {
                _records.Last = new DummyKeyRecord(lastDisplay + display);
            }
            else if (display == _keymap.Control
                     || display == _keymap.Alt
                     || display == _keymap.Shift)
            {
                // Handled on Key Up
                _modifierSingleDown = true;
            }
            else if (_records.Last is KeyRecord keyRecord && keyRecord.Display == display)
            {
                _records.Last = new RepeatKeyRecord(record, _settings);
            }
            else if (_records.Last is RepeatKeyRecord repeatRecord && repeatRecord.Repeated.Display == display)
            {
                repeatRecord.Increment();
            }
            else
            {
                _records.Add(record);
            }
        }
    }
}