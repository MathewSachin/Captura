using System;
using System.Collections.Generic;
using Screna;

namespace Captura.Models
{
    public class ImgurRecentItem : IRecentItem
    {
        public string DeleteHash { get; }
        public string Link { get; }

        public ImgurRecentItem(string Link,
            string DeleteHash,
            IIconSet Icons,
            LanguageManager Loc)
        {
            this.DeleteHash = DeleteHash;

            Display = this.Link = Link;
            Icon = Icons.Link;

            Actions = new[]
            {
                new RecentAction(Loc.CopyPath, Icons.Clipboard, () => this.Link.WriteToClipboard()),
                new RecentAction(Loc.Delete, Icons.Delete, OnDelete),
            };
        }

        async void OnDelete()
        {
            if (!ServiceProvider.MessageProvider.ShowYesNo($"Are you sure you want to Delete: {Link}?", "Confirm Deletion"))
                return;

            try
            {
                await ServiceProvider.Get<ImgurWriter>().DeleteUploadedFile(DeleteHash);
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

        bool IRecentItem.IsSaving => false;

        public IEnumerable<RecentAction> Actions { get; }

        public event Action RemoveRequested;
    }
}