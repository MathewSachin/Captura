using System.Windows;
using System.Windows.Controls;

namespace Captura
{
    public partial class HotkeyView
    {
        public HotkeyView()
        {
            InitializeComponent();
            
            for (int i = 0; i < HotKeyManager.Hotkeys.Count; ++i)
            {
                HotkeyGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var desc = new TextBlock { Text = HotKeyManager.Hotkeys[i].Description };

                HotkeyGrid.Children.Add(desc);
                Grid.SetRow(desc, i);

                var x = new HotkeySelector(HotKeyManager.Hotkeys[i]);

                HotkeyGrid.Children.Add(x);
                Grid.SetColumn(x, 2);
                Grid.SetRow(x, i);
            }
        }
    }
}
