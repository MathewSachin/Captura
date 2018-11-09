﻿using System;
using System.Windows;
using Captura.Views;

namespace Captura.Models
{
    class MainWindowProvider : IMainWindow
    {
        readonly Func<Window> _window;

        public MainWindowProvider(Func<Window> Window)
        {
            _window = Window;
        }

        public bool IsVisible
        {
            get => _window.Invoke().IsVisible;
            set
            {
                if (value)
                    _window.Invoke().Show();
                else _window.Invoke().Hide();
            }
        }

        public bool IsMinimized
        {
            get => _window.Invoke().WindowState == WindowState.Minimized;
            set => _window.Invoke().WindowState = value ? WindowState.Minimized : WindowState.Normal;
        }

        public void EditImage(string FileName)
        {
            var win = new ImageEditorWindow();

            win.Open(FileName);

            win.ShowAndFocus();
        }

        public void CropImage(string FileName)
        {
            new CropWindow(FileName).ShowAndFocus();
        }

        public void TrimMedia(string FileName)
        {
            var win = new TrimmerWindow();

            win.Open(FileName);

            win.ShowAndFocus();
        }
    }
}
