using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FirstFloor.ModernUI.Windows.Controls;

namespace Captura
{
    public partial class RecentItem : UserControl
    {
        public RecentItem(string FileName)
        {
            InitializeComponent();

            UrlButton.Content = FileName;

            UrlButton.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open,
                (s, e) => Process.Start(UrlButton.Content as string),
                (s, e) => e.CanExecute = File.Exists(UrlButton.Content as string)));
        }

        public event Action Remove;

        void RemoveButton_Click(object sender, RoutedEventArgs e) { if (Remove != null) Remove(); }

        void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(UrlButton.Content as string) { Verb = "Print" });
        }

        void Delete_Click(object sender, RoutedEventArgs e)
        {
            var MD = new ModernDialog() { Content = "Are you Sure?" };
            MD.Buttons = new Button[] { MD.OkButton, MD.CancelButton };

            if (MD.ShowDialog().Value)
            {
                File.Delete(UrlButton.Content as string);
                if (Remove != null) Remove();
            }
        }
    }
}
