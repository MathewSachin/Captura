namespace Captura
{
    public partial class FFMpegLogView
    {
        FFMpegLogView()
        {
            InitializeComponent();

            Closing += (s, e) =>
            {
                Hide();

                e.Cancel = true;
            };
        }

        public static FFMpegLogView Instance { get; } = new FFMpegLogView();
    }
}
