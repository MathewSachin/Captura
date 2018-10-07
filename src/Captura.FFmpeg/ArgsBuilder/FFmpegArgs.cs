using System.Collections.Generic;

namespace Captura.Models
{
    public abstract class FFmpegArgs
    {
        protected readonly List<string> Args = new List<string>();

        public virtual string GetArgs()
        {
            var args = "";

            foreach (var arg in Args)
            {
                args += arg + " ";
            }

            return args;
        }
    }
}