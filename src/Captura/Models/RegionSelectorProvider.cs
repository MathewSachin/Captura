using System;
using System.Drawing;
using System.Windows;
using Captura.Models;

namespace Captura
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RegionSelectorProvider : IRegionProvider
    {
        readonly Lazy<RegionSelector> _regionSelector;
        readonly RegionItem _regionItem;

        public RegionSelectorProvider(IVideoSourcePicker VideoSourcePicker)
        {
            _regionSelector = new Lazy<RegionSelector>(() =>
            {
                var reg = new RegionSelector(VideoSourcePicker);

                reg.SelectorHidden += () => SelectorHidden?.Invoke();
                reg.UpdateRegionName += M => _regionItem.Name = M;

                return reg;
            });

            _regionItem = new RegionItem(this);
        }

        public bool SelectorVisible
        {
            get => _regionSelector.Value.Visibility == Visibility.Visible;
            set
            {
                if (value)
                    _regionSelector.Value.Show();
                else _regionSelector.Value.Hide();
            }
        }

        public Rectangle SelectedRegion
        {
            get => _regionSelector.Value.SelectedRegion;
            set => _regionSelector.Value.SelectedRegion = value;
        }

        public IVideoItem VideoSource => _regionItem;

        public void Lock() => _regionSelector.Value.Lock();

        public void Release() => _regionSelector.Value.Release();

        public event Action SelectorHidden;

        public IntPtr Handle => _regionSelector.Value.Handle;
    }
}