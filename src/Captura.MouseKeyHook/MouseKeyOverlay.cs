﻿using System;
using System.Drawing;
using System.IO;

namespace Captura.Models
{
    /// <summary>
    /// Draws Mouse Clicks and/or Keystrokes on an Image.
    /// </summary>
    public class MouseKeyOverlay : IOverlay
    {
        #region Fields
        readonly IMouseKeyHook _hook;
        readonly KeystrokesSettings _keystrokesSettings;
        readonly IOverlay _mouseClickOverlay;
        readonly IOverlay _keyOverlay;

        readonly KeymapViewModel _keymap;
        readonly TextWriter _textWriter;
        #endregion
        
        /// <summary>
        /// Creates a new instance of <see cref="MouseKeyHook"/>.
        /// </summary>
        public MouseKeyOverlay(MouseClickSettings MouseClickSettings,
            KeystrokesSettings KeystrokesSettings,
            KeymapViewModel Keymap,
            string FileName,
            Func<TimeSpan> Elapsed)
        {
            _keystrokesSettings = KeystrokesSettings;
            _keymap = Keymap;

            _hook = new MouseKeyHook();
            _mouseClickOverlay = new MouseClickOverlay(_hook, MouseClickSettings);

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

            _keyOverlay?.Draw(Editor, Transform);
        }

        /// <summary>
        /// Frees all resources used by this object.
        /// </summary>
        public void Dispose()
        {
            _hook?.Dispose();

            _mouseClickOverlay?.Dispose();
            _keyOverlay?.Dispose();

            _textWriter?.Dispose();
        }
    }
}