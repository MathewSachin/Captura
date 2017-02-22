﻿using System.Windows;
using System.Windows.Input;

namespace Captura
{
    public partial class MainWindow
    {
        ConfigWindow _configWindow;
        Point _moveOrigin;
        
        public MainWindow()
        {
            InitializeComponent();

            _configWindow = new ConfigWindow();
            _configWindow.Closing += (s, e) =>
            {
                _configWindow.Hide();
                e.Cancel = true;
            };

            HotKey.RegisterAll();
            
            Closed += (s, e) =>
            {
                HotKey.UnRegisterAll();
                Application.Current.Shutdown();
            };
        }

        void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            _configWindow.Show();
            _configWindow.Focus();
        }

        void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();

            e.Handled = true;
        }

        void MinButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
                ReleaseMouseCapture();
        }

        void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _moveOrigin = e.GetPosition(this);
            CaptureMouse();
        }

        void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                var offset = e.GetPosition(this) - _moveOrigin;
                Left += offset.X;
                Top += offset.Y;
            }
        }        
    }
}
