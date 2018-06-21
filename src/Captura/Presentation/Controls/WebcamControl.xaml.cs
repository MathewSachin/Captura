using System.Windows;
using System.Windows.Interop;
using Captura.Webcam;

namespace Captura
{
    public partial class WebcamControl
    {
        public CaptureWebcam Capture { get; private set; }

        public Filter VideoDevice { get; set; }

        public WebcamControl()
        {
            InitializeComponent();
        }

        double Scale()
        {
            var source = PresentationSource.FromVisual(this);

            return source?.CompositionTarget?.TransformToDevice.M11 ?? 1d;
        }

        public void Refresh()
        {
            //To change the video device, a dispose is needed.
            if (Capture != null)
            {
                Capture.Dispose();
                Capture = null;
            }

            //Create capture object.
            if (VideoDevice != null && PresentationSource.FromVisual(this) is HwndSource source)
            {
                Capture = new CaptureWebcam(VideoDevice)
                {
                    PreviewWindow = source.Handle,
                    Scale = Scale()
                };
                
                SizeChanged += (S, E) => OnSizeChange();

                if (IsVisible)
                    Capture.StartPreview();

                OnSizeChange();
            }
        }

        public void ShowOnMainWindow(Window MainWindow)
        {
            //To change the video device, a dispose is needed.
            if (Capture != null)
            {
                Capture.Dispose();
                Capture = null;
            }

            //Create capture object.
            if (VideoDevice != null && PresentationSource.FromVisual(MainWindow) is HwndSource source)
            {
                Capture = new CaptureWebcam(VideoDevice)
                {
                    PreviewWindow = source.Handle,
                    Scale = Scale()
                };
                
                Capture.StartPreview();

                Capture.OnPreviewWindowResize(50 * Dpi.X, 40 * Dpi.Y, new System.Drawing.Point((int)(257 * Dpi.X), (int) Dpi.Y));
            }
        }

        void OnSizeChange()
        {
            Capture?.OnPreviewWindowResize(ActualWidth, ActualHeight, new System.Drawing.Point(5, 40));
        }

        void WebcamControl_OnIsVisibleChanged(object Sender, DependencyPropertyChangedEventArgs E)
        {
            if (IsVisible)
            {
                Refresh();
            }
            else
            {
                ShowOnMainWindow(MainWindow.Instance);
            }
        }
    }
}
