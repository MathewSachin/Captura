using System;
using Captura.Models;

namespace Captura.Console
{
    class FakeMessageProvider : IMessageProvider
    {
        public void ShowError(string Message)
        {
            System.Console.Error.WriteLine(Message);
        }

        public bool ShowYesNo(string Message, string Title)
        {
            throw new NotImplementedException();
        }
    }
}
