using System.Drawing;
using System;

namespace Captura.Models
{
    public class FakeRegionProvider : IRegionProvider
    {
        public static FakeRegionProvider Instance { get; } = new FakeRegionProvider();

        public bool SelectorVisible
        {
            get => false;
            set { }
        }

        
        public Rectangle SelectedRegion { get; set; }

        public IVideoItem VideoSource => new FakeRegionItem(SelectedRegion);

#pragma warning disable CS0067
        public event Action SelectorHidden;
#pragma warning restore CS0067

        public void Lock() { }

        public void Release() { }

        public IntPtr WindowsHandle { get; set; } = IntPtr.Zero;

        public IntPtr Handle => WindowsHandle;
    }
}
