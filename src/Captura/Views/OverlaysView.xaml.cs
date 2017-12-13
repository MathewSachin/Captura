using System.Windows.Controls;

namespace Captura
{
    public partial class MouseKeyHookView
    {
        public MouseKeyHookView()
        {
            InitializeComponent();

            UpdateSelection();
        }

        void ItemsControl_OnSelectionChanged(object Sender, SelectionChangedEventArgs E)
        {
            UpdateSelection();
        }

        void UpdateSelection()
        {
            if (ItemsControl.SelectedIndex == -1 && ItemsControl.HasItems)
            {
                ItemsControl.SelectedIndex = 0;
            }
        }
    }
}
