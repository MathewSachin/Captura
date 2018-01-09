using Captura.Models;
using Con = System.Console;

namespace Captura.Console
{
    class FakeMessageProvider : IMessageProvider
    {
        public void ShowError(string Message)
        {
            Con.Error.WriteLine(Message);
        }

        public void ShowFFMpegUnavailable()
        {
            ShowError("FFMpeg is not available.\nYou can install ffmpeg by calling: captura ffmpeg --install [path]");
        }

        public bool ShowYesNo(string Message, string Title)
        {
            Con.Write($"{Message} (Y/N):");

            var reply = Con.ReadLine();

            return reply != null && reply.ToLower() == "y";
        }
    }
}
