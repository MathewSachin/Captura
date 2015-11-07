using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using ManagedWin32;
using ManagedWin32.Api;

namespace Captura
{
    public partial class SettingsVideo : UserControl, INotifyPropertyChanged
    {
        public SettingsVideo()
        {
            InitializeComponent();

            DataContext = this;

            AvailableWindows = new ObservableCollection<KeyValuePair<IntPtr, string>>();

            Refresh();
        }

        public static IntPtr SelectedWindow = Recorder.DesktopHandle;
        public static bool IncludeCursor { get { return Properties.Settings.Default.IncludeCursor; } }

        public ObservableCollection<KeyValuePair<IntPtr, string>> AvailableWindows { get; private set; }

        public IntPtr _SelectedWindow
        {
            get { return SelectedWindow; }
            set
            {
                if (SelectedWindow != value)
                { 
                    SelectedWindow = value;
                    OnPropertyChanged("_SelectedWindow");
                }
            }
        }

        public void Refresh()
        {
            AvailableWindows.Clear();
            AvailableWindows.Add(new KeyValuePair<IntPtr, string>((IntPtr)(-1), "[No Video]"));
            AvailableWindows.Add(new KeyValuePair<IntPtr, string>(Recorder.DesktopHandle, "[Desktop]"));
            AvailableWindows.Add(new KeyValuePair<IntPtr, string>(User32.FindWindow("Shell_TrayWnd", null), "[Taskbar]"));

            foreach (var win in WindowHandler.Enumerate())
            {
                var hWnd = win.Handle;
                if (!win.IsVisible) continue;
                if (!(User32.GetWindowLong(hWnd, GetWindowLongValue.GWL_EXSTYLE).HasFlag(WindowStyles.WS_EX_APPWINDOW)))
                {
                    if (User32.GetWindow(hWnd, GetWindowEnum.Owner) != IntPtr.Zero)
                        continue;
                    if (User32.GetWindowLong(hWnd, GetWindowLongValue.GWL_EXSTYLE).HasFlag(WindowStyles.WS_EX_TOOLWINDOW))
                        continue;
                    if (User32.GetWindowLong(hWnd, GetWindowLongValue.GWL_STYLE).HasFlag(WindowStyles.WS_CHILD))
                        continue;
                }

                AvailableWindows.Add(new KeyValuePair<IntPtr, string>(hWnd, win.Title));
            }

            _SelectedWindow = Recorder.DesktopHandle;
        }

        void OnPropertyChanged(string e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(e));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
