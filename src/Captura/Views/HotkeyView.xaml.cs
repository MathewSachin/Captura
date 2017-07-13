﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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

            HotkeyGrid.RowDefinitions.Clear();
            HotkeyGrid.Children.Clear();

            Render();
        }

        void Render()
        {
            for (int i = 0; i < HotKeyManager.Hotkeys.Count; ++i)
            {
                var hotkey = HotKeyManager.Hotkeys[i];

                HotkeyGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

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
                    Content = textBlock
                };

                BindingOperations.SetBinding(textBlock, TextBlock.TextProperty, binding);

                HotkeyGrid.Children.Add(desc);
                Grid.SetRow(desc, i);

                var x = new HotkeySelector(hotkey)
                {
                    IsEnabled = hotkey.IsActive
                };

                HotkeyGrid.Children.Add(x);
                Grid.SetColumn(x, 2);
                Grid.SetRow(x, i);

                desc.Checked += (s, e) =>
                {
                    hotkey.IsActive = true;
                    x.IsEnabled = true;
                };
                
                desc.Unchecked += (s, e) =>
                {
                    hotkey.IsActive = false;
                    x.IsEnabled = false;
                };
            }
        }
    }
}
