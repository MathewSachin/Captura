using Captura.Properties;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Captura
{
    public class RecentViewModel : ViewModelBase
    {
        public ObservableCollection<FrameworkElement> RecentList { get; } = new ObservableCollection<FrameworkElement>();
        
        public FrameworkElement AddTemp(string FileName)
        {
            var i = new Button
            {
                Content = "Saving " + Path.GetFileName(FileName),
                IsEnabled = false
            };

            RecentList.Insert(0, i);

            return i;
        }

        public void Remove(FrameworkElement Item) => RecentList.Remove(Item);

        public void Add(string FileName, RecentItemType Type)
        {
            var I = new RecentItem(FileName);

            if (Type == RecentItemType.Image)
                I.PrintButton.Visibility = Visibility.Visible;

            I.Remove += () => RecentList.Remove(I);
            
            RecentList.Insert(0, I);

            CommandManager.InvalidateRequerySuggested();
        }

        public ICommand OpenOutputFolderCommand => new DelegateCommand(() => Process.Start("explorer.exe", Settings.Default.OutputPath));
    }
}