using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using Captura.Loc;

namespace Captura.Models
{
    public class UploadRecentItem : IRecentItem
    {
        public string DeleteHash { get; }
        public string Link { get; }
        public IImageUploader UploaderService { get; }

        public UploadRecentItem(string Link, string DeleteHash, IImageUploader UploaderService)
        {
            this.DeleteHash = DeleteHash;
            this.UploaderService = UploaderService;

            ClickCommand = new DelegateCommand(() => ServiceProvider.LaunchFile(new ProcessStartInfo(Link)));

            RemoveCommand = new DelegateCommand(() => RemoveRequested?.Invoke());

            var icons = ServiceProvider.Get<IIconSet>();
            var loc = ServiceProvider.Get<ILocalizationProvider>();

            Display = this.Link = Link;
            Icon = icons.Link;

            Actions = new[]
            {
                new RecentAction(loc.CopyPath, icons.Clipboard, () => this.Link.WriteToClipboard()),
                new RecentAction(loc.Delete, icons.Delete, OnDelete)
            };
        }

        async void OnDelete()
        {
            if (!ServiceProvider.MessageProvider.ShowYesNo($"Are you sure you want to Delete: {Link}?", "Confirm Deletion"))
                return;

            try
            {
                await UploaderService.DeleteUploadedFile(DeleteHash);
            }
            catch (Exception e)
            {
                ServiceProvider.MessageProvider.ShowException(e, $"Could not Delete: {Link}");

                return;
            }

            RemoveRequested?.Invoke();
        }

        public string Display { get; }

        public string Icon { get; }

        public string IconColor => "MediumPurple";

        bool IRecentItem.IsSaving => false;

        public ICommand ClickCommand { get; }

        public ICommand RemoveCommand { get; }

        public IEnumerable<RecentAction> Actions { get; }

        public event Action RemoveRequested;
    }
}