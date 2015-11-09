using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Captura.Properties;

namespace Captura
{
    partial class Recent : UserControl
    {
        static int Count = 0;

        static ObservableCollection<Button> _RecentList = new ObservableCollection<Button>();

        public ObservableCollection<Button> RecentList { get; private set; }

        public static RoutedUICommand RecentButtonClick = new RoutedUICommand();

        public Recent()
        {
            RecentList = _RecentList;
            InitializeComponent();

            DataContext = this;
        }

        public static void Add(string FileName)
        {
            var B = new Button() { Content = ++Count + ".\t" + Path.GetFileName(FileName) };

            B.Command = RecentButtonClick;

            B.Background = new SolidColorBrush(Colors.Transparent);

            B.HorizontalContentAlignment = HorizontalAlignment.Left;

            B.Height = 28;

            B.CommandBindings.Add(new CommandBinding(RecentButtonClick, (s, e) => Process.Start(FileName),
                (s, e) => e.CanExecute = File.Exists(FileName)));

            _RecentList.Add(B);
        }

        void OpenOutputFolder<T>(object sender, T e) { Process.Start("explorer.exe", Settings.Default.OutputPath); }
    }
}
