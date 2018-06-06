﻿using System.Windows;
using Captura.ViewModels;

namespace Captura
{
    public partial class RecentItem
    {
        public RecentItem()
        {
            InitializeComponent();
        }

        void CropClick(object Sender, RoutedEventArgs E)
        {
            if (DataContext is RecentItemViewModel vm)
            {
                new CropWindow(vm.FilePath).ShowAndFocus();
            }
        }
    }
}
