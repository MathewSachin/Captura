using System;
using Captura.Models;

namespace Captura.Fakes
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class FakeMessageProvider : IMessageProvider
    {
        public void ShowError(string Message, string Header = null)
        {
            if (Header != null)
                Console.WriteLine(Header);

            Console.Error.WriteLine(Message);
        }

        public void ShowException(Exception Exception, string Message, bool Blocking = false)
        {
            ShowError(Exception.ToString());
        }

        public bool ShowYesNo(string Message, string Title)
        {
            Console.Write($"{Message} (Y/N): ");

            var reply = Console.ReadLine();

            return reply != null && reply.ToLower() == "y";
        }
    }
}
