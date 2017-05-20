using System.Drawing;
using Captura.Models;
using System.Windows;

namespace Captura
{
    class RegionProvider : IRegionProvider
    {
        public bool SelectorVisible
        {
            get => RegionSelector.Instance.Visibility == Visibility.Visible;
            set
            {
                if (value)
                    RegionSelector.Instance.Show();
                else RegionSelector.Instance.Hide();
            }
        }

        public Rectangle SelectedRegion
        {
            get => RegionSelector.Instance.Rectangle;
            set => RegionSelector.Instance.Rectangle = value;
        }

        public IVideoItem VideoSource => RegionItem.Instance;
    }
}
