using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Captura.Properties;

namespace Captura
{
    public class RecentViewModel : ViewModelBase
    {
        public ObservableCollection<RecentItem> RecentList { get; } = new ObservableCollection<RecentItem>();
        
        public void Add(string FileName, RecentItemType Type)
        {
            var I = new RecentItem(FileName);

            if (Type == RecentItemType.Image)
                I.PrintButton.Visibility = Visibility.Visible;

            I.Remove += () => RecentList.Remove(I);

            RecentList.Add(I);
        }

        public ICommand OpenOutputFolderCommand => new DelegateCommand(() => Process.Start("explorer.exe", Settings.Default.OutputPath));
    }
}