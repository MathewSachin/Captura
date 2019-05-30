using System;
using System.Windows;
using System.Windows.Interop;
using DesktopDuplication;
using Screna;
using SharpDX.Direct3D9;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PreviewWindowService : IPreviewWindow
    {
        readonly PreviewWindow _previewWindow = new PreviewWindow();

        D3D9PreviewAssister _d3D9PreviewAssister;
        IntPtr _backBufferPtr;
        Texture _texture;

        public bool IsVisible { get; private set; }

        public PreviewWindowService()
        {
            _previewWindow.IsVisibleChanged += (S, E) => IsVisible = _previewWindow.IsVisible;

            IsVisible = _previewWindow.IsVisible;

            // Prevent Closing by User
            _previewWindow.Closing += (S, E) =>
            {
                E.Cancel = true;

                _previewWindow.Hide();
            };
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

            _previewWindow.Dispatcher.Invoke(() =>
            {
                _previewWindow.DisplayImage.Image = null;

                _lastFrame?.Dispose();
                _lastFrame = Frame;

                Frame = Frame.Unwrap();

                switch (Frame)
                {
                    case DrawingFrameBase drawingFrame:
                        _previewWindow.WinFormsHost.Visibility = Visibility.Visible;
                        _previewWindow.DisplayImage.Image = drawingFrame.Bitmap;
                        break;

                    case Texture2DFrame texture2DFrame:
                        _previewWindow.WinFormsHost.Visibility = Visibility.Collapsed;
                        if (_d3D9PreviewAssister == null)
                        {
                            _d3D9PreviewAssister = new D3D9PreviewAssister(ServiceProvider.Get<IPlatformServices>());
                            _texture = _d3D9PreviewAssister.GetSharedTexture(texture2DFrame.PreviewTexture);

                            using (var surface = _texture.GetSurfaceLevel(0))
                            {
                                _backBufferPtr = surface.NativePointer;
                            }
                        }

                        Invalidate(_backBufferPtr, texture2DFrame.Width, texture2DFrame.Height);
                        break;
                }
            });
        }

        void Invalidate(IntPtr BackBufferPtr, int Width, int Height)
        {
            _previewWindow.D3DImage.Lock();
            _previewWindow.D3DImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, BackBufferPtr);

            if (BackBufferPtr != IntPtr.Zero)
                _previewWindow.D3DImage.AddDirtyRect(new Int32Rect(0, 0, Width, Height));

            _previewWindow.D3DImage.Unlock();
        }

        public void Dispose()
        {
            _previewWindow.Dispatcher.Invoke(() =>
            {
                _previewWindow.DisplayImage.Image = null;

                _lastFrame?.Dispose();
                _lastFrame = null;

                if (_d3D9PreviewAssister != null)
                {
                    Invalidate(IntPtr.Zero, 0, 0);

                    _texture.Dispose();

                    _d3D9PreviewAssister.Dispose();

                    _d3D9PreviewAssister = null;
                }
            });
        }

        public void Show()
        {
            _previewWindow.Dispatcher.Invoke(() => _previewWindow.ShowAndFocus());
        }
    }
}