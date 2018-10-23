namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class FakeWindowProvider : IMainWindow
    {
        public bool IsVisible
        {
            get => true;
            set { }
        }

        public bool IsMinimized
        {
            get => false;
            set { }
        }
    }
}
