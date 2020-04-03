using System;
using System.Drawing;
using System.IO;
using Captura.Video;

namespace Captura.MouseKeyHook
{
    /// <summary>
    /// Draws Mouse Clicks and/or Keystrokes on an Image.
    /// </summary>
    public class MouseKeyOverlay : IOverlay
    {
        #region Fields
        readonly IMouseKeyHook _hook;
        readonly KeystrokesSettings _keystrokesSettings;
        readonly IOverlay _mouseClickOverlay,
            _keyOverlay,
            _scrollOverlay;

        readonly KeymapViewModel _keymap;
        readonly TextWriter _textWriter;
        #endregion
        
        /// <summary>
        /// Creates a new instance of <see cref="MouseKeyHook"/>.
        /// </summary>
        public MouseKeyOverlay(IMouseKeyHook Hook,
            MouseClickSettings MouseClickSettings,
            KeystrokesSettings KeystrokesSettings,
            KeymapViewModel Keymap,
            string FileName,
            Func<TimeSpan> Elapsed)
        {
            _keystrokesSettings = KeystrokesSettings;
            _keymap = Keymap;

            _hook = Hook;
            _mouseClickOverlay = new MouseClickOverlay(_hook, MouseClickSettings);
            _scrollOverlay = new ScrollOverlay(_hook, MouseClickSettings);

            if (KeystrokesSettings.SeparateTextFile)
            {
                _textWriter = InitKeysToTextFile(FileName, Elapsed);
            }
            else _keyOverlay = new KeyOverlay(_hook, KeystrokesSettings, Keymap);
        }

        TextWriter InitKeysToTextFile(string FileName, Func<TimeSpan> Elapsed)
        {
            var dir = Path.GetDirectoryName(FileName);
            var fileNameWoExt = Path.GetFileNameWithoutExtension(FileName);

            var targetName = $"{fileNameWoExt}.keys.txt";

            var path = dir == null ? targetName : Path.Combine(dir, targetName);

            var keystrokeFileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            var textWriter = new StreamWriter(keystrokeFileStream);

            _hook.KeyDown += (S, E) =>
            {
                if (!_keystrokesSettings.Display)
                {
                    return;
                }

                var record = new KeyRecord(E, _keymap);

                _textWriter.WriteLine($"{Elapsed.Invoke()}: {record.Display}");
            };

            return textWriter;
        }
        
        /// <summary>
        /// Draws overlay.
        /// </summary>
        public void Draw(IEditableFrame Editor, Func<Point, Point> Transform = null)
        {
            _mouseClickOverlay?.Draw(Editor, Transform);
            _scrollOverlay?.Draw(Editor, Transform);

            _keyOverlay?.Draw(Editor, Transform);
        }

        /// <summary>
        /// Frees all resources used by this object.
        /// </summary>
        public void Dispose()
        {
            _hook?.Dispose();

            _mouseClickOverlay?.Dispose();
            _scrollOverlay?.Dispose();
            _keyOverlay?.Dispose();

            _textWriter?.Dispose();
        }
    }
}