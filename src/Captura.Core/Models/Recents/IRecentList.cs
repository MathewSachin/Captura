using System;
using System.Collections.Generic;

namespace Captura.Models
{
    public interface IRecentList : IDisposable
    {
        void Add(IRecentItem RecentItem);

        IEnumerable<IRecentItem> Items { get; }

        void Clear();
    }
}