using System;
using System.Collections.Generic;
using Captura.ViewModels;

namespace Captura.Models
{
    public interface IRecentList : IDisposable
    {
        RecentItemViewModel Add(string FilePath, RecentItemType ItemType, bool IsSaving);

        IEnumerable<RecentItemViewModel> Items { get; }

        void Clear();
    }
}