using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Captura.Models;

namespace Captura
{
    public partial class HotkeyView
    {
        public HotkeyView()
        {
            InitializeComponent();

            Render();
        }

        void Reset_Click(object sender, RoutedEventArgs e)
        {
            HotKeyManager.Dispose();
            Settings.Instance.Hotkeys.Clear();

            HotKeyManager.Hotkeys.Clear();
            HotKeyManager.Populate();

            HotkeysPanel.Children.Clear();

            Render();
        }

        void Render()
        {
            foreach (var hotkey in HotKeyManager.Hotkeys)
            {
                var splitter = new GridSplitter
                {
                    Height = 1,
                    Margin = new Thickness(0, 10, 0, 10)
                };

                HotkeysPanel.Children.Add(splitter);

                var binding = new Binding
                {
                    Path = new PropertyPath(nameof(hotkey.Description.Display)),
                    Source = hotkey.Description
                };

                var textBlock = new TextBlock
                {
                    TextWrapping = TextWrapping.WrapWithOverflow
                };

                var desc = new CheckBox
                {
                    IsChecked = hotkey.IsActive,
                    Content = textBlock,
                    Margin = new Thickness(0, 0, 0, 2)
                };

                BindingOperations.SetBinding(textBlock, TextBlock.TextProperty, binding);

                HotkeysPanel.Children.Add(desc);

                var x = new HotkeySelector(hotkey)
                {
                    IsEnabled = hotkey.IsActive
                };

                HotkeysPanel.Children.Add(x);

                desc.Checked += (s, e) =>
                {
                    hotkey.IsActive = true;
                    x.IsEnabled = true;

                    x.TextColor();
                };
                
                desc.Unchecked += (s, e) =>
                {
                    hotkey.IsActive = false;
                    x.IsEnabled = false;

                    x.TextColor();
                };
            }
        }
    }
}
