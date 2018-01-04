using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;


namespace Captura
{
    public class FFMpegCmdOptions
    {
        [Option("install", DefaultValue = null, HelpText = "Install FFMPeg to specified folder.")]
        public string install { get; set; }
    }
}
