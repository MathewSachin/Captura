using System;
using Captura.Models;

namespace Captura.Console
{
    class FakeWebCamProvider : IWebCamProvider
    {
        public bool IsVisible
        {
            get => false;
            set { }
        }

        public event Action IsVisibleChanged;
    }
}
