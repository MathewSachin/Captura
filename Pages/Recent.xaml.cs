using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Controls;
using Captura.Properties;

namespace Captura
{
    partial class Recent : UserControl
    {
        static ObservableCollection<RecentButton> _RecentList = new ObservableCollection<RecentButton>();

        ObservableCollection<RecentButton> RecentList;

        public Recent()
        {
            RecentList = _RecentList;
            InitializeComponent();

            DataContext = this;
        }

        public static void Add(string FileName) { _RecentList.Add(new RecentButton(FileName)); }

        void OpenOutputFolder<T>(object sender, T e) { Process.Start("explorer.exe", Settings.Default.OutputPath); }
    }
}
