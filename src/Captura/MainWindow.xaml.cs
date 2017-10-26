﻿using Captura.Models;
using Captura.ViewModels;
using Captura.Views;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Captura
{
    public partial class MainWindow
    {
        public static MainWindow Instance { get; private set; }

        FFMpegDownloader _downloader;

        public MainWindow()
        {
            Instance = this;

            ServiceProvider.RegionProvider = new RegionSelector();

            ServiceProvider.MessageProvider = new MessageProvider();

            ServiceProvider.WebCamProvider = new WebCamProvider();

            FFMpegService.FFMpegDownloader += () =>
            {
                if (_downloader == null)
                {
                    _downloader = new FFMpegDownloader();
                    _downloader.Closed += (s, args) => _downloader = null;
                }

                _downloader.ShowAndFocus();
            };
            
            InitializeComponent();

            ServiceProvider.MainWindow = new MainWindowProvider(this);

            if (App.CmdOptions.Tray)
                Hide();
            
            // Register for Windows Messages
            ComponentDispatcher.ThreadPreprocessMessage += (ref MSG Message, ref bool Handled) =>
            {
                const int WmHotkey = 786;

                if (Message.message == WmHotkey)
                {
                    var id = Message.wParam.ToInt32();

                    ServiceProvider.RaiseHotKeyPressed(id);
                }
            };

            ServiceProvider.SystemTray = new SystemTray(SystemTray);
            
            Closing += (s, e) =>
            {
                if (!TryExit())
                    e.Cancel = true;
            };

            Loaded += (s, e) =>
            {
                if (DataContext is MainViewModel vm)
                {
                    ServiceProvider.MainViewModel = vm;

                    vm.Init(!App.CmdOptions.NoPersist, true, !App.CmdOptions.Reset, !App.CmdOptions.NoHotkeys);
                }
            };
        }
        
        void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();

            e.Handled = true;
        }

        void MinButton_Click(object sender, RoutedEventArgs e) => SystemCommands.MinimizeWindow(this);

        void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        void SystemTray_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Hide();
            }
            else
            {
                Show();

                WindowState = WindowState.Normal;

                Activate();
            }
        }

        bool TryExit()
        {
            if (DataContext is MainViewModel vm)
            {
                if (vm.RecorderState == RecorderState.Recording)
                {
                    if (!ServiceProvider.MessageProvider.ShowYesNo(
                        "A Recording is in progress. Are you sure you want to exit?", "Confirm Exit"))
                        return false;
                }

                vm.Dispose();
            }

            SystemTray.Dispose();

            return true;
        }

        void MenuExit_Click(object sender, RoutedEventArgs e) => Close();

        void HideButton_Click(object Sender, RoutedEventArgs E) => Hide();
    }
}
