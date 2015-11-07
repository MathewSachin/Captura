using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Controls;
using ManagedWin32.Api;
using ManagedWin32;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Captura
{
    partial class Home : UserControl, INotifyPropertyChanged
    {
        public Home()
        {
            InitializeComponent();
            DataContext = this;

            AvailableWindows = new ObservableCollection<KeyValuePair<IntPtr, string>>();

            Refresh();
        }

        void ScreenShot<T>(object sender = null, T e = default(T))
        {
            string FileName = null;
            ImageFormat ImgFmt = ScreenShotSettings.SelectedImageFormat;
            string Extension = ImgFmt == ImageFormat.Icon ? "ico"
                : ImgFmt == ImageFormat.Jpeg ? "jpg"
                : ImgFmt.ToString();
            bool SaveToClipboard = ScreenShotSettings.SaveToClipboard;

            if (!SaveToClipboard)
                FileName = Path.Combine(Properties.Settings.Default.OutputPath,
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "." + Extension);

            if (SelectedWindow == Recorder.DesktopHandle
                || SelectedWindow == RegionSelector.Instance.Handle
                || !ScreenShotSettings.UseDWM)
            {
                RECT Rect = Recorder.DesktopRectangle;

                if (SelectedWindow != Recorder.DesktopHandle)
                    User32.GetWindowRect(SelectedWindow, ref Rect);

                var BMP = new Bitmap(Rect.Right - Rect.Left, Rect.Bottom - Rect.Top);

                using (var g = Graphics.FromImage(BMP))
                {
                    g.CopyFromScreen(Rect.Left, Rect.Top, 0, 0,
                        new System.Drawing.Size(Rect.Right - Rect.Left, Rect.Bottom - Rect.Top),
                        CopyPixelOperation.SourceCopy);

                    g.Flush();
                }

                if (SaveToClipboard)
                {
                    BMP.WriteToClipboard(ImgFmt == ImageFormat.Png);
                    Status.Content = "Saved to Clipboard";
                }
                else
                {
                    try { BMP.Save(FileName, ImgFmt); }
                    catch (Exception E)
                    {
                        Status.Content = "Not Saved. " + E.Message;
                        return;
                    }

                    Status.Content = "Saved to " + FileName;
                }
            }
            else new Screenshot().CaptureWindow(SelectedWindow, SaveToClipboard, IncludeCursor, FileName, ImgFmt);

            if (FileName != null && !SaveToClipboard) Recent.Add(FileName);
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
