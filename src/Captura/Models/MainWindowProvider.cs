using System.Windows;

namespace Captura.Models
{
    class MainWindowProvider : IMainWindow
    {
        readonly Window _window;

        public MainWindowProvider(Window Window)
        {
            _window = Window;
        }

        public bool IsVisible
        {
            get => _window.IsVisible;
            set
            {
                if (value)
                    _window.Show();
                else _window.Hide();
            }
        }

        public bool IsMinimized
        {
            get => _window.WindowState == WindowState.Minimized;
            set => _window.WindowState = value ? WindowState.Minimized : WindowState.Normal;
        }
    }
}
