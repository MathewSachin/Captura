﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Input;
using Captura.Models;
using Captura.ViewModels;
using Screna;

namespace Captura
{
    public static class Extensions
    {
        public static void ExecuteIfCan(this ICommand Command)
        {
            if (Command.CanExecute(null))
                Command.Execute(null);
        }

        static Bitmap Resize(this Bitmap Image, Size Resize, bool KeepAspectRatio, bool DisposeOriginal = true)
        {
            var resizeWidth = Resize.Width;
            var resizeHeight = Resize.Height;

            if (KeepAspectRatio)
            {
                var ratio = Math.Min((double) Resize.Width / Image.Width, (double) Resize.Height / Image.Height);

                resizeWidth = (int)(Image.Width * ratio);
                resizeHeight = (int)(Image.Height * ratio);
            }

            var resized = new Bitmap(Resize.Width, Resize.Height);

            using (var g = Graphics.FromImage(resized))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;

                try
                {
                    g.DrawImage(Image, 0, 0, resizeWidth, resizeHeight);
                }
                finally
                {
                    if (DisposeOriginal)
                        Image.Dispose();
                }
            }

            return resized;
        }

        public static Bitmap Transform(this Bitmap Image, ScreenShotSettings TransformSettings, bool SkipResize = false)
        {
            if (TransformSettings.Resize && !SkipResize)
            {
                Image = Image.Resize(new Size(TransformSettings.ResizeWidth, TransformSettings.ResizeHeight), true);
            }

            #region Rotate Flip
            var flip = "Flip";

            if (!TransformSettings.FlipHorizontal && !TransformSettings.FlipVertical)
                flip += "None";

            if (TransformSettings.FlipHorizontal)
                flip += "X";

            if (TransformSettings.FlipVertical)
                flip += "Y";

            var rotateFlip = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), TransformSettings.RotateBy + flip);

            Image.RotateFlip(rotateFlip);
            #endregion

            return Image;
        }

        public static async Task UploadToImgur(this Bitmap Bitmap)
        {
            var imgur = ServiceProvider.Get<ImgurWriter>();

            var shotVm = ServiceProvider.Get<ScreenShotViewModel>();

            var response = await imgur.Save(Bitmap, shotVm.SelectedScreenShotImageFormat);

            switch (response)
            {
                case Exception ex:
                    ServiceProvider.MessageProvider.ShowException(ex, "Upload to Imgur failed");
                    break;

                case ImgurUploadResponse uploadResponse:
                    var recents = ServiceProvider.Get<IRecentList>();

                    var link = uploadResponse.Data.Link;
                    var deleteHash = uploadResponse.Data.DeleteHash;

                    recents.Add(new ImgurRecentItem(link, deleteHash));

                    link.WriteToClipboard();
                    break;
            }
        }
    }
}