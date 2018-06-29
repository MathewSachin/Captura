using System;
using System.Drawing;

namespace Captura.Models
{
    public interface IRegionProvider
    {
        bool SelectorVisible { get; set; }

        Rectangle SelectedRegion { get; set; }

        IVideoItem VideoSource { get; }
        
        void Lock();

        void Release();

        event Action SelectorHidden;
    }
}
