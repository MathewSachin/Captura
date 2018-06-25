namespace Captura
{
    public partial class FFmpegCodecWindow
    {
        FFmpegCodecWindow()
        {
            InitializeComponent();
        }

        static FFmpegCodecWindow _instance;

        public static void ShowInstance()
        {
            if (_instance == null)
            {
                _instance = new FFmpegCodecWindow();

                _instance.Closed += (S, E) => _instance = null;
            }

            _instance.ShowAndFocus();
        }
    }
}
