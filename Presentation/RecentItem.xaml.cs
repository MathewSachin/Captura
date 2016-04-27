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

            UrlButton.CommandBindings.Add(new CommandBinding(NavigationCommands.GoToPage,
                (s, e) => Process.Start(_filePath),
                (s, e) => e.CanExecute = File.Exists(_filePath)));
        }

        public event Action Remove;

        void RemoveButton_Click(object sender, RoutedEventArgs e) => Remove?.Invoke();

        void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(_filePath) { Verb = "Print" });
        }

        void Delete_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(_filePath);
            Remove?.Invoke();
        }
    }
}
