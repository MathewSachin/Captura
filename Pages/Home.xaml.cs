using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Controls;
using ManagedWin32.Api;

namespace Captura
{
    partial class Home : UserControl
    {
        public Home() { InitializeComponent(); }

        void ScreenShot<T>(object sender = null, T e = default(T))
        {
            IntPtr SelectedWindow = SettingsVideo.SelectedWindow;

            string FileName = null;
            ImageFormat ImgFmt = SettingsScreenShot.SelectedImageFormat;
            string Extension = ImgFmt == ImageFormat.Icon ? "ico"
                : ImgFmt == ImageFormat.Jpeg ? "jpg"
                : ImgFmt.ToString();
            bool SaveToClipboard = SettingsScreenShot.SaveToClipboard;

            if (!SaveToClipboard)
                FileName = Path.Combine(Properties.Settings.Default.OutputPath,
                    DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "." + Extension);

            if (SelectedWindow == Recorder.DesktopHandle
                || SelectedWindow == RegionSelector.Instance.Handle
                || !SettingsScreenShot.UseDWM)
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
            //else new Screenshot().CaptureWindow(this, FileName, ImgFmt);

            if (FileName != null && !SaveToClipboard) Recent.Add(FileName);
        }
    }
}
