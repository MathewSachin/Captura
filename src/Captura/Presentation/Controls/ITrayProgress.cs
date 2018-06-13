using System;

namespace Captura
{
    public interface ITrayProgress
    {
        void RegisterClick(Action OnClick);

        void UpdateProgress(int Progress);

        void UpdatePrimaryText(string Text);

        void UpdateSecondaryText(string Text);

        void Finish(bool Success);
    }
}