// Adapted from https://github.com/jasonpang/desktop-duplication-net

using SharpDX.DXGI;
using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using Captura.Video;
using Captura.Windows.DirectX;

namespace Captura.Windows.DesktopDuplication
{
    public class FullScreenDesktopDuplicator : IDisposable
    {
        Direct2DEditorSession _editorSession;
        readonly List<DeskDuplOutputEntry> _outputs = new List<DeskDuplOutputEntry>();

        public FullScreenDesktopDuplicator(bool IncludeCursor,
            IPreviewWindow PreviewWindow,
            IPlatformServices PlatformServices)
        {
            using var factory = new Factory1();
            var outputs = factory
                .Adapters1
                .SelectMany(M => M.Outputs)
                .ToArray();

            var bounds = PlatformServices.DesktopRectangle;

            Width = bounds.Width;
            Height = bounds.Height;

            PointTransform = P => new Point(P.X - bounds.Left, P.Y - bounds.Top);

            _editorSession = new Direct2DEditorSession(Width, Height, PreviewWindow);

            _outputs.AddRange(outputs.Select(M =>
            {
                var output1 = M.QueryInterface<Output1>();

                var rect = M.Description.DesktopBounds;

                return new DeskDuplOutputEntry
                {
                    DuplCapture = new DuplCapture(output1),
                    Location = new SharpDX.Point(rect.Left - bounds.Left, rect.Top - bounds.Top),
                    MousePointer = IncludeCursor ? new DxMousePointer(_editorSession) : null
                };
            }));
        }

        public int Width { get; }
        public int Height { get; }

        public Func<Point, Point> PointTransform { get; }

        public IEditableFrame Capture()
        {
            foreach (var entry in _outputs)
            {
                try
                {
                    if (!entry.DuplCapture.Get(_editorSession.DesktopTexture, entry.MousePointer, entry.Location))
                        return RepeatFrame.Instance;
                }
                catch
                {
                    try { entry.DuplCapture.Init(); }
                    catch
                    {
                        // ignored
                    }

                    return RepeatFrame.Instance;
                }                
            }

            var editor = new Direct2DEditor(_editorSession);

            foreach (var entry in _outputs)
            {
                entry.MousePointer?.Draw(editor, entry.Location);
            }

            return editor;
        }

        public void Dispose()
        {
            foreach (var entry in _outputs)
            {
                try { entry.DuplCapture.Dispose(); }
                catch { }
                finally { entry.DuplCapture = null; }

                // Mouse Pointer disposed later to prevent errors.
                try { entry.MousePointer?.Dispose(); }
                catch { }
                finally { entry.MousePointer = null; }
            }

            try { _editorSession.Dispose(); }
            catch { }
            finally { _editorSession = null; }
        }
    }
}
