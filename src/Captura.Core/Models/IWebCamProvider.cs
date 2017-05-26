using System;

namespace Captura.Models
{
    public interface IWebCamProvider
    {
        bool IsVisible { get; set; }

        event Action IsVisibleChanged;
    }
}
