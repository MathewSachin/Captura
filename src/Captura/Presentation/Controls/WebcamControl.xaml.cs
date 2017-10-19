using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using Captura.Webcam;

namespace Captura
{
    public partial class WebcamControl
    {
        public CaptureWebcam Capture { get; set; }

        public Filter VideoDevice { get; set; }

        public WebcamControl()
        {
            InitializeComponent();
        }

        bool IsInDesignMode()
        {
            return DesignerProperties.GetIsInDesignMode(this);
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
            if (VideoDevice != null)
            {
                var source = PresentationSource.FromVisual(this) as HwndSource;

                Capture = new CaptureWebcam(VideoDevice)
                {
                    PreviewWindow = source.Handle,
                    Scale = Scale()
                };

                void OnSizeChange()
                {
                    Capture.OnPreviewWindowResize(ActualWidth, ActualHeight, new System.Drawing.Point(5, 40));
                }

                SizeChanged += (s, e) => OnSizeChange();

                Capture.StartPreview();

                OnSizeChange();
            }
        }

        public void Unload()
        {
            if (Capture != null)
            {
                Capture.StopPreview();
                Capture.Dispose();
            }

            VideoDevice = null;

            GC.Collect();
        }

        void WebcamControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            //Don't show the feed if in design mode.
            if (IsInDesignMode())
                return;

            Refresh();
        }

        void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Unload();
        }
    }
}
