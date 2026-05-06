using System;

namespace Captura.Gifski
{
    public class GifskiException : Exception
    {
        public GifskiException(int ExitCode, Exception InnerException = null)
            : base($"An Error Occurred with Gifski, Exit Code: {ExitCode}", InnerException) { }
    }
}
