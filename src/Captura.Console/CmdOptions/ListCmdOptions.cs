using CommandLine;

namespace Captura
{
    [Verb("list", HelpText = "Display available video sources, encoders, audio sources, etc.")]
    // ReSharper disable once ClassNeverInstantiated.Global
    class ListCmdOptions : ICmdlineVerb
    {
        public void Run()
        {
            var lister = ServiceProvider.Get<ConsoleLister>();

            lister.List();
        }
    }
}