using System.Windows.Controls;

namespace Captura
{
    public partial class MouseKeyHookView
    {
        public MouseKeyHookView()
        {
            InitializeComponent();

            UpdateSelection(ItemsControl);

            UpdateSelection(ImagesItemsControl);
        }

        void ItemsControl_OnSelectionChanged(object Sender, SelectionChangedEventArgs E)
        {
            if (Sender is ListView listView)
                UpdateSelection(listView);
        }

        void UpdateSelection(ListView Sender)
        {
            if (Sender.SelectedIndex == -1 && Sender.HasItems)
            {
                Sender.SelectedIndex = 0;
            }
        }
    }
}
