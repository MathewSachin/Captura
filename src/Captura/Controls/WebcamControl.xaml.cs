namespace Captura
{
    public partial class WebcamControl
    {
        public WebcamControl()
        {
            InitializeComponent();
        }

        void OpenPreview()
        {
            WebCamWindow.Instance.ShowAndFocus();
        }
    }
}
