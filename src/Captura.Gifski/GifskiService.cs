using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Captura
{
    public static class GifskiService
    {
        const string ExeName = "gifski.exe";

        public static string ExePath => @"C:\Users\luis9\Downloads\gifski-0.9.1\win\gifski.exe";

        public static Process StartGifski(string Arguments, Action<int> progress)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = ExePath,
                    Arguments = Arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                },
                EnableRaisingEvents = true
            };

            //Frame 2 / 90  #_............................................................ 2m 
            var progressRegex = new Regex(@"Frame ([0-9]+) \/ ([0-9]+)", RegexOptions.Compiled);

            process.OutputDataReceived += (s, e) =>
            {
                if (string.IsNullOrEmpty(e.Data))
                {
                    return;
                }

                var match = progressRegex.Match(e.Data);
                if (match.Success && match.Groups.Count == 3)
                {
                    var current = match.Groups[1].Value;
                    var total = match.Groups[2].Value;
                    var normalized = int.Parse(current) / (float) int.Parse(total);
                    progress((int) (normalized * 100));
                }
            };

            process.Start();
            process.BeginOutputReadLine();

            return process;
        }
    }
}
