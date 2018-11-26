using System;
using System.Collections.ObjectModel;

namespace Captura.Models
{
    public interface IRecentList : IDisposable
    {
        void Add(IRecentItem RecentItem);

        ReadOnlyObservableCollection<IRecentItem> Items { get; }

        void Clear();
    }
}