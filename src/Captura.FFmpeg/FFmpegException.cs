using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Captura.FFmpeg
{
    public class FFmpegException : Exception
    {
        // FFmpeg prints the cause first, the lines after it are a generic cascade ending in "Conversion failed!".
        static readonly string[] ErrorWords = { "error", "cannot", "unsupported", "not support" };

        public FFmpegException(int ExitCode, IFFmpegLogEntry Log = null, Exception InnerException = null)
            : base($"An Error Occurred with FFmpeg, Exit Code: {ExitCode}.\n{GetErrorLines(Log)}See FFmpeg Log for more info.", InnerException) { }

        static string GetErrorLines(IFFmpegLogEntry Log)
        {
            // The complete log starts with the arguments, FFmpeg's own output comes after this header.
            var header = $"OUTPUT:{Environment.NewLine}-------------{Environment.NewLine}";

            var complete = Log?.GetCompleteLog();
            var start = complete?.IndexOf(header, StringComparison.Ordinal) ?? -1;

            if (start == -1)
                return "";

            var lines = complete.Substring(start + header.Length)
                .Split('\r', '\n')
                .Where(L => !string.IsNullOrWhiteSpace(L) && !L.StartsWith("frame=") && !L.StartsWith("size="))
                .ToArray();

            var errors = lines.Where(L => ErrorWords.Any(W => L.IndexOf(W, StringComparison.OrdinalIgnoreCase) != -1))
                .Take(3)
                .ToArray();

            var shown = errors.Length > 0 ? errors : lines.Skip(lines.Length - 3);

            // "[h264_nvenc @ 0000024e0fb9cec0]" -> "[h264_nvenc]", the addresses only widen the error dialog.
            return string.Concat(shown.Select(L => Regex.Replace(L, @"\s@\s[0-9A-Fa-f]+(?=\])", "") + "\n"));
        }
    }
}