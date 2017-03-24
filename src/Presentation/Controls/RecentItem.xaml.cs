using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Captura
{
    public partial class RecentItem
    {
        readonly string _filePath;

        public RecentItem(string FileName)
        {
            InitializeComponent();

            _filePath = FileName;
            UrlButton.Content = Path.GetFileName(FileName);

            UrlButton.CommandBindings.Add(new CommandBinding(NavigationCommands.GoToPage, (s, e) => 
            {
                try { Process.Start(_filePath); }
                catch
                {
                    // Suppress Errors
                }
            }, (s, e) => e.CanExecute = File.Exists(_filePath)));
        }

        public event Action Remove;

        void RemoveButton_Click(object sender, RoutedEventArgs e) => Remove?.Invoke();

        void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(_filePath) { Verb = "Print" });
        }

        void Delete_Click(object sender, RoutedEventArgs e)
        {
            try { File.Delete(_filePath); }
            catch { MessageBox.Show($"Can't Delete {_filePath}. It will still be removed from list.", "Delete Failed", MessageBoxButton.OK, MessageBoxImage.Warning); }

            Remove?.Invoke();
        }
    }
}
