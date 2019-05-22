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
        public void StartSharpAvi()
        {
            var process = Start("start --encoder sharpavi:0");

            Thread.Sleep(1000);

            process.StandardInput.WriteLine('q');

            process.WaitForExit();

            Assert.Equal(0, process.ExitCode);
        }

        // TODO: Don't skip this test
        [Fact(Skip = "This is failing on AppVeyor")]
        public void StartSharpAviFixedDuration()
        {
            var process = Start("start --encoder sharpavi:0 --length 1");

            process.WaitForExit();

            Assert.Equal(0, process.ExitCode);
        }
    }
}
