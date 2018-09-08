using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Captura.Models;

namespace Captura
{
    public partial class VideoPage
    {
        public VideoPage()
        {
            InitializeComponent();
        }

        void Preview_Click(object Sender, RoutedEventArgs E)
        {
            WebCamWindow.Instance.ShowAndFocus();
        }

        void OnVideoSourceReSelect(object Sender, MouseButtonEventArgs E)
        {
            if (Sender is ListViewItem item
                && item.IsSelected
                && item.DataContext is VideoSourceModel model)
            {
                switch (model.Provider)
                {
                    case WindowSourceProvider windowSourceProvider:
                        windowSourceProvider.PickWindow();
                        break;

                    case ScreenSourceProvider screenSourceProvider:
                        screenSourceProvider.PickScreen();
                        break;

                    case DeskDuplSourceProvider deskDuplSourceProvider:
                        deskDuplSourceProvider.PickScreen();
                        break;
                }
            }
        }
    }
}
