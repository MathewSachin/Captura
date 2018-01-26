using System;
using Captura.Models;

namespace Captura.Core
{
    class FakeMessageProvider : IMessageProvider
    {
        public void ShowError(string Message)
        {
            Console.Error.WriteLine(Message);
        }

        public void ShowFFMpegUnavailable()
        {
            ShowError("FFMpeg is not available.\nYou can install ffmpeg by calling: captura ffmpeg --install [path]");
        }

        public bool ShowYesNo(string Message, string Title)
        {
            Console.Write($"{Message} (Y/N):");

            var reply = Console.ReadLine();

            return reply != null && reply.ToLower() == "y";
        }
    }
}
