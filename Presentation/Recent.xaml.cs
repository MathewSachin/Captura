using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Captura.Properties;
using System.Windows;

namespace Captura
{
    public enum RecentItemType
    {
        Image,
        Video,
        Audio
    }

    partial class Recent
    {
        static readonly ObservableCollection<RecentItem> _RecentList = new ObservableCollection<RecentItem>();

        public ObservableCollection<RecentItem> RecentList { get; private set; } = _RecentList;

        public static RoutedUICommand RecentButtonClick = new RoutedUICommand();

        public Recent()
        {
            InitializeComponent();

            DataContext = this;
        }

        public static void Add(string FileName, RecentItemType Type) 
        {
            var I = new RecentItem(FileName);

            if (Type == RecentItemType.Image)
                I.PrintButton.Visibility = Visibility.Visible;

            I.Remove += () => _RecentList.Remove(I);

            _RecentList.Add(I);
        }

        void OpenOutputFolder(object sender, RoutedEventArgs e) => Process.Start("explorer.exe", Settings.Default.OutputPath);
    }
}
