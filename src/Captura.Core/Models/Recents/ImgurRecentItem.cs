using System;
using System.Collections.Generic;
using Screna;

namespace Captura.Models
{
    public class ImgurRecentItem : IRecentItem
    {
        readonly string _deleteHash;
        readonly string _link;

        public ImgurRecentItem(string Link,
            string DeleteHash,
            IIconSet Icons,
            LanguageManager Loc)
        {
            _deleteHash = DeleteHash;

            Display = _link = Link;
            Icon = Icons.Link;

            Actions = new[]
            {
                new RecentAction(Loc.CopyPath, Icons.Clipboard, () => _link.WriteToClipboard()),
                new RecentAction(Loc.Delete, Icons.Delete, OnDelete),
            };
        }

        async void OnDelete()
        {
            if (!ServiceProvider.MessageProvider.ShowYesNo($"Are you sure you want to Delete: {_link}?", "Confirm Deletion"))
                return;

            try
            {
                await ServiceProvider.Get<ImgurWriter>().DeleteUploadedFile(_deleteHash);
            }
            catch (Exception e)
            {
                ServiceProvider.MessageProvider.ShowException(e, $"Could not Delete: {_link}");

                return;
            }

            RemoveRequested?.Invoke();
        }

        public string Display { get; }

        public string Icon { get; }

        bool IRecentItem.IsSaving { get; }

        public IEnumerable<RecentAction> Actions { get; }

        public event Action RemoveRequested;
    }
}