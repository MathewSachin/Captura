using System;
using Sentry;
using Serilog;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class SentryLogger : ILogger
    {
        public SentryLogger(ISentryApiKeys ApiKeys)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.Sentry(M =>
                {
                    M.Dsn = new Dsn(ApiKeys.SentryDsn);
                    M.AttachStacktrace = true;
                })
                .CreateLogger();
        }

        public void Fatal(Exception E, string Message, params object[] Properties)
        {
            Log.Fatal(E, Message, Properties);
        }

        public void Error(Exception E, string Message, params object[] Properties)
        {
            Log.Error(E, Message, Properties);
        }

        public void Warning(Exception E, string Message, params object[] Properties)
        {
            Log.Warning(E, Message, Properties);
        }

        public void Information(Exception E, string Message, params object[] Properties)
        {
            Log.Information(E, Message, Properties);
        }

        public void Debug(Exception E, string Message, params object[] Properties)
        {
            Log.Debug(E, Message, Properties);
        }

        public void Verbose(Exception E, string Message, params object[] Properties)
        {
            Log.Verbose(E, Message, Properties);
        }

        public void Dispose()
        {
            Log.CloseAndFlush();
        }
    }
}