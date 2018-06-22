using System.Windows;
using System.Windows.Interop;
using Captura.Webcam;
using Point = System.Drawing.Point;

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
                    Scale = Dpi.X
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
                    Scale = Dpi.X
                };
                
                Capture.StartPreview();

                Capture.OnPreviewWindowResize(50, 40, new Point(257, 1));
            }
        }

        void OnSizeChange()
        {
            Capture?.OnPreviewWindowResize(ActualWidth, ActualHeight, new Point(5, 40));
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
