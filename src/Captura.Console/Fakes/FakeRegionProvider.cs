using Captura.Models;
using System.Drawing;

namespace Captura.Console
{
    class FakeRegionProvider : IRegionProvider
    {
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
