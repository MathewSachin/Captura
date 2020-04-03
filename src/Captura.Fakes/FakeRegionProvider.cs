using System.Drawing;
using System;
using Captura.Video;

namespace Captura.Fakes
{
    public class FakeRegionProvider : IRegionProvider
    {
        FakeRegionProvider() { }

        public static FakeRegionProvider Instance { get; } = new FakeRegionProvider();

        public bool SelectorVisible
        {
            get => false;
            set { }
        }
        
        public Rectangle SelectedRegion { get; set; }

        public IVideoItem VideoSource => new RegionItem(this, ServiceProvider.Get<IPlatformServices>());

#pragma warning disable CS0067
        public event Action SelectorHidden;
#pragma warning restore CS0067

        public IntPtr Handle => IntPtr.Zero;
    }
}
