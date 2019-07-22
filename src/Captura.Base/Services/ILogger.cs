using System;

namespace Captura
{
    public interface ILogger : IDisposable
    {
        void Fatal(Exception E, string Message, params object[] Properties);

        void Error(Exception E, string Message, params object[] Properties);

        void Warning(Exception E, string Message, params object[] Properties);

        void Information(Exception E, string Message, params object[] Properties);

        void Debug(Exception E, string Message, params object[] Properties);

        void Verbose(Exception E, string Message, params object[] Properties);
    }
}