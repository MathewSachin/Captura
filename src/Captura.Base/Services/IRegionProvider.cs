using System;
using System.Drawing;

namespace Captura.Video
{
    public interface IRegionProvider
    {
        bool SelectorVisible { get; set; }

        Rectangle SelectedRegion { get; set; }

        IVideoItem VideoSource { get; }

        IntPtr Handle { get; }
    }
}
