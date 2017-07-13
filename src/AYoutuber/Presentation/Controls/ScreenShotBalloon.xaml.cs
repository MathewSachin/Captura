﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;
using System.IO;
using System.Diagnostics;
using System;
using System.Windows.Media.Imaging;

namespace Captura
{
    public partial class ScreenShotBalloon : UserControl
    {
        public string FileName { get; }

        public string FilePath { get; }

        public ScreenShotBalloon(string FilePath)
        {
            this.FilePath = FilePath;
            FileName = Path.GetFileName(FilePath);

            InitializeComponent();

            // Do not assign image directly, cache it, else the file can't be deleted.
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(FilePath);
            image.EndInit();
            img.Source = image;
        }

        public void CloseButton_Click(object sender = null, RoutedEventArgs e = null)
        {
            //the tray icon assigned this attached property to simplify access
            var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon?.CloseBalloon();
        }

        /// <summary>
        /// If the users hovers over the balloon, we don't close it.
        /// </summary>
        void grid_MouseEnter(object sender, MouseEventArgs e)
        {
            // the tray icon assigned this attached property to simplify access
            var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon?.ResetBalloonCloseTimer();
        }

        void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ServiceProvider.LaunchFile(new ProcessStartInfo(FilePath));

            CloseButton_Click();
        }
    }
}