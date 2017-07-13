using Captura.Models;

namespace Captura.Console
{
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
