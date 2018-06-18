using System;

namespace Captura.Models
{
    class FakeMessageProvider : IMessageProvider
    {
        public void ShowError(string Message, string Header = null)
        {
            if (Header != null)
                Console.WriteLine(Header);

            Console.Error.WriteLine(Message);
        }

        public void ShowFFmpegUnavailable()
        {
            ShowError("FFmpeg is not available.\nYou can install ffmpeg by calling: captura ffmpeg --install [path]");
        }

        public void ShowException(Exception Exception, string Message)
        {
            ShowError(Exception.ToString());
        }

        public bool ShowYesNo(string Message, string Title)
        {
            Console.Write($"{Message} (Y/N):");

            var reply = Console.ReadLine();

            return reply != null && reply.ToLower() == "y";
        }
    }
}
