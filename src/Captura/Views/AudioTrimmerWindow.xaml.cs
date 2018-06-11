using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Captura.Views
{
    public partial class AudioTrimmerWindow
    {
        public AudioTrimmerWindow()
        {
            InitializeComponent();
        }
        
        void Slider_PreviewMouseLeftButtonUp(object Sender, MouseButtonEventArgs E)
        {
            if (DataContext is AudioTrimmerViewModel vm && Sender is Slider slider)
            {
                if (!vm.IsDragging)
                    return;

                vm.PlaybackPosition = TimeSpan.FromSeconds(slider.Value);

                vm.IsDragging = false;
            }
        }

        void Slider_PreviewMouseLeftButtonDown(object Sender, MouseButtonEventArgs E)
        {
            if (DataContext is AudioTrimmerViewModel vm)
            {
                vm.IsDragging = true;
            }
        }

        void Slider_MouseLeftButtonUp(object Sender, MouseButtonEventArgs E)
        {
            if (DataContext is AudioTrimmerViewModel vm && Sender is Slider slider)
            {
                vm.PlaybackPosition = TimeSpan.FromSeconds(slider.Value);
            }
        }
    }
}
