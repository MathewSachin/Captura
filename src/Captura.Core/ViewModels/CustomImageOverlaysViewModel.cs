using System;
using System.Windows.Input;
using Captura.Models;

namespace Captura.ViewModels
{
    public class CustomImageOverlaysViewModel : OverlayListViewModel<CustomImageOverlaySettings>
    {
        readonly IDialogService _dialogService;

        public CustomImageOverlaysViewModel(Settings Settings, IDialogService DialogService) : base(Settings.ImageOverlays)
        {
            _dialogService = DialogService;

            ChangeCommand = new DelegateCommand(Change);
        }

        public ICommand ChangeCommand { get; }

        void Change(object M)
        {
            if (M is CustomImageOverlaySettings settings)
            {
                var fileName = _dialogService.PickFile(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                    "Select Image");

                if (fileName != null)
                {
                    settings.Source = fileName;
                }
            }
        }
    }
}