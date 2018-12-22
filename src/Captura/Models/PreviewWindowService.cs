using Screna;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PreviewWindowService : IPreviewWindow
    {
        readonly PreviewWindow _previewWindow = new PreviewWindow();

        public PreviewWindowService()
        {
            // Prevent Closing by User
            _previewWindow.Closing += (S, E) =>
            {
                E.Cancel = true;

                _previewWindow.Hide();
            };
        }

        public void Init(int Width, int Height) { }

        IBitmapFrame _lastFrame;

        public void Display(IBitmapFrame Frame)
        {
            if (Frame is RepeatFrame)
                return;

            _previewWindow.Dispatcher.Invoke(() =>
            {
                _previewWindow.DisplayImage.Image = null;

                _lastFrame?.Dispose();

                _lastFrame = Frame;

                if (!_previewWindow.IsVisible)
                    return;

                if (Frame is FrameBase frame)
                {
                    _previewWindow.DisplayImage.Image = frame.Bitmap;
                }
                else if (Frame is MultiDisposeFrame multiDisposeFrame
                         && multiDisposeFrame.Frame is FrameBase frameBase)
                {
                    _previewWindow.DisplayImage.Image = frameBase.Bitmap;
                }
            });
        }

        public void Dispose()
        {
            _previewWindow.Dispatcher.Invoke(() =>
            {
                _previewWindow.DisplayImage.Image = null;

                _lastFrame?.Dispose();
                _lastFrame = null;
            });
        }

        public void Show()
        {
            _previewWindow.Dispatcher.Invoke(() => _previewWindow.Show());
        }
    }
}