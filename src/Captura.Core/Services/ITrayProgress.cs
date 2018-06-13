using System;

namespace Captura
{
    public interface ITrayProgress
    {
        void RegisterClick(Action OnClick);

        int Progress { get; set; }

        string PrimaryText { get; set; }

        string SecondaryText { get; set; }

        bool Finished { get; set; }

        bool Success { get; set; }
    }
}