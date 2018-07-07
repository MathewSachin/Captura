using System.Diagnostics;
using System.Threading;
using Xunit;

namespace Captura.Tests.Console
{
    [Collection(nameof(Tests))]
    public class ConsoleStartTests
    {
        static Process Start(string Arguments)
        {
            var path = TestManagerFixture.GetCliPath();

            var process = new Process
            {
                StartInfo =
                {
                    FileName = path,
                    Arguments = Arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                },
                EnableRaisingEvents = true
            };

            process.Start();

            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            void Write(string Data, string Prefix)
            {
                if (string.IsNullOrWhiteSpace(Data))
                    return;

                Trace.WriteLine($"{Prefix}: {Data}");
            }

            process.ErrorDataReceived += (S, E) => Write(E.Data, "Err");
            process.OutputDataReceived += (S, E) => Write(E.Data, "Out");

            return process;
        }
        
        [Fact]
        public void StartGif()
        {
            var process = Start("start --encoder gif");

            Thread.Sleep(1000);

            process.StandardInput.WriteLine('q');

            Assert.True(process.WaitForExit(5000), "Timeout");

            Assert.Equal(0, process.ExitCode);
        }

        [Fact]
        public void StartGifFixedDuration()
        {
            var process = Start("start --encoder gif --length 1");

            Assert.True(process.WaitForExit(5000), "Timeout");

            Assert.Equal(0, process.ExitCode);
        }
    }
}
