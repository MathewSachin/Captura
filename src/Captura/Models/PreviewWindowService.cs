using Screna;

namespace Captura.Models
{
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

        public void Display(IBitmapFrame Frame)
        {
            _previewWindow.Dispatcher.Invoke(() =>
            {
                if (Frame is RepeatFrame)
                    return;

                _lastFrame?.Dispose();

                _lastFrame = Frame;

                if (!_previewWindow.IsVisible)
                    return;

                if (Frame is FrameBase frame)
                {
                    _previewWindow.DisplayImage.Image = frame.Bitmap;
                }
            });
        }
        
        IBitmapFrame _lastFrame;

        public void Dispose()
        {
            _previewWindow.Dispatcher.Invoke(() =>
            {
                _lastFrame?.Dispose();
                _lastFrame = null;

                _previewWindow.DisplayImage.Image = null;
            });
        }

        public void Show()
        {
            _previewWindow.Dispatcher.Invoke(() => _previewWindow.Show());
        }
    }
}