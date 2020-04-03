using System;
using System.Windows.Input;
using Captura.Models;
using Captura.Video;
using Reactive.Bindings;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CustomImageOverlaysViewModel : OverlayListViewModel<CustomImageOverlaySettings>
    {
        readonly IDialogService _dialogService;

        public CustomImageOverlaysViewModel(Settings Settings, IDialogService DialogService) : base(Settings.ImageOverlays)
        {
            _dialogService = DialogService;

            ChangeCommand = new ReactiveCommand<CustomImageOverlaySettings>()
                .WithSubscribe(Change);
        }

        public ICommand ChangeCommand { get; }

        void Change(CustomImageOverlaySettings M)
        {
            var fileName = _dialogService.PickFile(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                "Select Image");

            if (fileName != null)
            {
                M.Source = fileName;
            }
        }
    }
}