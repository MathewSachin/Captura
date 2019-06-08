using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Captura
{
    [Verb("shot", HelpText = "Take Screenshots")]
    class ShotCmdOptions : CommonCmdOptions, ICmdlineVerb
    {
        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Take screenshot containing cursor", new ShotCmdOptions
                {
                    Cursor = true
                });

                yield return new Example("Save screenshot to out.png", new ShotCmdOptions
                {
                    FileName = "out.png"
                });

                yield return new Example("Take screenshot of second screen", new ShotCmdOptions
                {
                    Source = "screen:1"
                });
            }
        }

        public void Run()
        {
            using var manager = ServiceProvider.Get<ConsoleManager>();
            manager.Shot(this);
        }
    }
}
