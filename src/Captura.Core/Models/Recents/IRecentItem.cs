using System;
using System.Collections.Generic;

namespace Captura.Models
{
    public interface IRecentItem
    {
        string Display { get; }

        string Icon { get; }

        bool IsSaving { get; }

        event Action RemoveRequested;

        IEnumerable<RecentAction> Actions { get; }
    }
}