using System;
using System.Windows.Forms;
using System.Windows.Input;
using Captura.Models;

namespace Captura.ViewModels
{
    public class CustomImageOverlaysViewModel : CustomOverlaysBaseViewModel<CustomImageOverlaySettings>
    {
        public CustomImageOverlaysViewModel(Settings Settings) : base(Settings.ImageOverlays)
        {
            ChangeCommand = new DelegateCommand(Change);
        }

        public ICommand ChangeCommand { get; }

        void Change(object M)
        {
            if (M is CustomImageOverlaySettings settings)
            {
                var ofd = new OpenFileDialog
                {
                     CheckFileExists = true,
                     CheckPathExists = true,
                     InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                     Title = "Select Image"
                };

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    settings.Source = ofd.FileName;
                }
            }
        }
    }
}