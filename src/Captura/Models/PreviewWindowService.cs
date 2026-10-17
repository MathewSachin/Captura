using System;
using System.Windows;
using System.Windows.Interop;
using Captura.Windows.DirectX;
using Captura.Windows.Gdi;
using Reactive.Bindings.Extensions;
using SharpDX.Direct3D9;

namespace Captura.Video
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PreviewWindowService : IPreviewWindow
    {
        D3D9PreviewAssister _d3D9PreviewAssister;
        IntPtr _backBufferPtr;
        Texture _texture;
        bool _d3D9Failed;
        readonly VisualSettings _visualSettings;

        public void Show()
        {
            _visualSettings.Expanded = true;
        }

        public bool IsVisible { get; private set; }

        public PreviewWindowService(VisualSettings VisualSettings)
        {
            _visualSettings = VisualSettings;

            VisualSettings.ObserveProperty(M => M.Expanded)
                .Subscribe(M => IsVisible = M);
        }

        IBitmapFrame _lastFrame;

        public void Display(IBitmapFrame Frame)
        {
            if (Frame is RepeatFrame)
                return;

            if (!IsVisible)
            {
                Frame.Dispose();
                return;
            }

            var win = MainWindow.Instance;

            win.Dispatcher.Invoke(() =>
            {
                win.DisplayImage.Image = null;

                _lastFrame?.Dispose();
                _lastFrame = Frame;

                Frame = Frame.Unwrap();

                switch (Frame)
                {
                    case DrawingFrame drawingFrame:
                        try
                        {
                            // TODO: Preview is not shown during Webcam only recordings
                            // This check swallows errors
                            var h = drawingFrame.Bitmap.Height;

                            if (h == 0)
                                return;
                        }
                        catch { return; }

                        win.WinFormsHost.Visibility = Visibility.Visible;
                        win.DisplayImage.Image = drawingFrame.Bitmap;
                        break;

                    case Texture2DFrame texture2DFrame:
                        win.WinFormsHost.Visibility = Visibility.Collapsed;
                        if (_d3D9Failed)
                            break;

                        if (_d3D9PreviewAssister == null)
                        {
                            // All-or-nothing: a half-built setup is skipped by every later frame
                            // and breaks Dispose. On failure, no preview until this recording ends.
                            D3D9PreviewAssister assister = null;
                            Texture texture = null;
                            IntPtr backBufferPtr;

                            try
                            {
                                assister = new D3D9PreviewAssister(ServiceProvider.Get<IPlatformServices>());
                                texture = assister.GetSharedTexture(texture2DFrame.PreviewTexture);

                                using var surface = texture.GetSurfaceLevel(0);
                                backBufferPtr = surface.NativePointer;
                            }
                            catch
                            {
                                texture?.Dispose();
                                assister?.Dispose();
                                _d3D9Failed = true;
                                break;
                            }

                            _d3D9PreviewAssister = assister;
                            _texture = texture;
                            _backBufferPtr = backBufferPtr;
                        }

                        Invalidate(_backBufferPtr, texture2DFrame.Width, texture2DFrame.Height);
                        break;
                }
            });
        }

        void Invalidate(IntPtr BackBufferPtr, int Width, int Height)
        {
            var win = MainWindow.Instance;

            win.D3DImage.Lock();
            win.D3DImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, BackBufferPtr);

            if (BackBufferPtr != IntPtr.Zero)
                win.D3DImage.AddDirtyRect(new Int32Rect(0, 0, Width, Height));

            win.D3DImage.Unlock();
        }

        public void Dispose()
        {
            var win = MainWindow.Instance;

            win.Dispatcher.Invoke(() =>
            {
                win.DisplayImage.Image = null;
                win.WinFormsHost.Visibility = Visibility.Collapsed;

                _lastFrame?.Dispose();
                _lastFrame = null;

                if (_d3D9PreviewAssister != null)
                    Invalidate(IntPtr.Zero, 0, 0);

                _texture?.Dispose();
                _texture = null;

                _d3D9PreviewAssister?.Dispose();
                _d3D9PreviewAssister = null;

                _backBufferPtr = IntPtr.Zero;
                _d3D9Failed = false;
            });
        }
    }
}