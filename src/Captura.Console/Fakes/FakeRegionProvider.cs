using Captura.Models;
using System.Drawing;

namespace Captura.Console
{
    class FakeRegionProvider : IRegionProvider
    {
        FakeRegionProvider() { }

        public static FakeRegionProvider Instance { get; } = new FakeRegionProvider();

        public bool SelectorVisible
        {
            get => false;
            set { }
        }
        
        public Rectangle SelectedRegion { get; set; }

        public IVideoItem VideoSource => new FakeRegionItem(SelectedRegion);

        public bool SnapEnabled
        {
            get => false;
            set { }
        }
    }
}
