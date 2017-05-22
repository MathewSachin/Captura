using Captura.ViewModels;

namespace Captura
{
    public partial class WebCamWindow
    {
        WebCamViewModel viewModel;

        WebCamWindow()
        {
            InitializeComponent();

            viewModel = DataContext as WebCamViewModel;

            viewModel.Init(webCameraControl);

            Closing += (s, e) =>
            {
                Hide();

                e.Cancel = true;
            };

            IsVisibleChanged += (s, e) =>
            {
                if (!IsVisible)
                    viewModel.Dispose();
                else viewModel.Refresh();
            };
        }

        public static WebCamWindow Instance { get; } = new WebCamWindow();
    }
}