using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Diagnostics;
using System;
using System.Windows.Media.Imaging;
using Captura.Models;

namespace Captura
{
    public partial class ScreenShotBalloon : IRemoveRequester
    {
        readonly string _filePath;

        public ScreenShotBalloon(string FilePath)
        {
            _filePath = FilePath;
            DataContext = Path.GetFileName(FilePath);

            InitializeComponent();

            // Do not assign image directly, cache it, else the file can't be deleted.
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(FilePath);
            image.EndInit();
            Img.Source = image;
        }

        void CloseButton_Click(object Sender, RoutedEventArgs E) => OnClose();

        void OnClose()
        {
            RemoveRequested?.Invoke();
        }

        public event Action RemoveRequested;
        
        void Image_MouseUp(object Sender, MouseButtonEventArgs E)
        {
            ServiceProvider.LaunchFile(new ProcessStartInfo(_filePath));

            OnClose();
        }

        void EditButton_OnClick(object Sender, RoutedEventArgs E)
        {
            var winserv = ServiceProvider.Get<IMainWindow>();
            winserv.EditImage(_filePath);
        }
    }
}