using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Captura.Views
{
    public partial class TrimmerWindow
    {
        public TrimmerWindow()
        {
            InitializeComponent();
            
            if (DataContext is TrimmerViewModel vm)
            {
                vm.AssignPlayer(MediaElement);

                Closing += (S, E) => vm.Dispose();
            }
        }

        public void Open(string FileName)
        {
            if (DataContext is TrimmerViewModel vm)
            {
                vm.Open(FileName);
            }
        }

        void Slider_PreviewMouseLeftButtonUp(object Sender, MouseButtonEventArgs E)
        {
            if (DataContext is TrimmerViewModel vm && Sender is Slider slider)
            {
                if (!vm.IsDragging)
                    return;

                vm.PlaybackPosition = TimeSpan.FromSeconds(slider.Value);

                vm.IsDragging = false;
            }
        }

        void Slider_PreviewMouseLeftButtonDown(object Sender, MouseButtonEventArgs E)
        {
            if (DataContext is TrimmerViewModel vm)
            {
                vm.IsDragging = true;
            }
        }

        void Slider_MouseLeftButtonUp(object Sender, MouseButtonEventArgs E)
        {
            if (DataContext is TrimmerViewModel vm && Sender is Slider slider)
            {
                vm.PlaybackPosition = TimeSpan.FromSeconds(slider.Value);
            }
        }
    }
}
