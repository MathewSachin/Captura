using System;
using System.Windows;

namespace Captura.Models
{
    public class WebCamProvider : IWebCamProvider
    {
        WebCamWindow _window;

        public WebCamProvider()
        {
            _window = WebCamWindow.Instance;

            _window.IsVisibleChanged += (s, e) => IsVisibleChanged?.Invoke();
        }

        public bool IsVisible
        {
            get => _window.Visibility == Visibility.Visible;
            set
            {
                _window.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public event Action IsVisibleChanged;
    }
}
