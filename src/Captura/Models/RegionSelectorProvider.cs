using System;
using System.Drawing;
using System.Windows;
using Captura.ViewModels;

namespace Captura.Video
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RegionSelectorProvider : IRegionProvider
    {
        readonly Lazy<RegionSelector> _regionSelector;
        readonly RegionItem _regionItem;
        readonly RegionSelectorViewModel _viewModel;

        public RegionSelectorProvider(RegionSelectorViewModel ViewModel,
            IPlatformServices PlatformServices)
        {
            _viewModel = ViewModel;

            _regionSelector = new Lazy<RegionSelector>(() => new RegionSelector(ViewModel));

            _regionItem = new RegionItem(this, PlatformServices);
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
            get => _viewModel.SelectedRegion;
            set => _viewModel.SelectedRegion = value;
        }

        public IVideoItem VideoSource => _regionItem;

        public IntPtr Handle => _regionSelector.Value.Handle;
    }
}