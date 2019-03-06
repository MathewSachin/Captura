using System;

namespace Captura.Models
{
    public interface IMessageProvider
    {
        void ShowError(string Message, string Header = null);

        bool ShowYesNo(string Message, string Title);

        void ShowException(Exception Exception, string Message, bool Blocking = false);
    }
}
