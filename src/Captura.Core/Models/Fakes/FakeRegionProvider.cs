using Captura.Models;
using System.Drawing;
using System;

namespace Captura.Core
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

        public IVideoItem VideoSource => new FakeRegionItem(SelectedRegion);

        public event Action SelectorHidden;

        public void Lock() { }

        public void Release() { }
    }
}
