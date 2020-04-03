using Captura.Audio;
using System.Linq;
using Captura.FFmpeg;
using Captura.SharpAvi;
using Captura.Webcam;
using static System.Console;

namespace Captura
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ConsoleLister
    {
        static readonly string Underline = $"\n{new string('-', 30)}";

        readonly WebcamModel _webcam;
        readonly IAudioSource _audioSource;
        readonly IPlatformServices _platformServices;
        readonly FFmpegWriterProvider _ffmpegWriterProvider;
        readonly SharpAviWriterProvider _sharpAviWriterProvider;

        public ConsoleLister(WebcamModel Webcam,
            IAudioSource AudioSource,
            IPlatformServices PlatformServices,
            FFmpegWriterProvider FfmpegWriterProvider,
            SharpAviWriterProvider SharpAviWriterProvider)
        {
            _webcam = Webcam;
            _audioSource = AudioSource;
            _platformServices = PlatformServices;
            _ffmpegWriterProvider = FfmpegWriterProvider;
            _sharpAviWriterProvider = SharpAviWriterProvider;
        }

        public void List()
        {
            FFmpeg();

            SharpAvi();

            Windows();

            Screens();

            Audio();

            Webcam();
        }

        void Webcam()
        {
            if (_webcam.AvailableCams.Count > 1)
            {
                WriteLine("AVAILABLE WEBCAMS" + Underline);

                for (var i = 1; i < _webcam.AvailableCams.Count; ++i)
                {
                    WriteLine($"{(i - 1).ToString().PadRight(2)}: {_webcam.AvailableCams[i]}");
                }

                WriteLine();
            }
        }

        void Audio()
        {
            WriteLine($"Audio Source: {_audioSource.Name}");

            WriteLine();

            var mics = _audioSource
                .Microphones
                .ToArray();

            var speakers = _audioSource
                .Speakers
                .ToArray();

            // Microphones
            if (mics.Length > 0)
            {
                WriteLine("AVAILABLE MICROPHONES" + Underline);

                for (var i = 0; i < mics.Length; ++i)
                {
                    WriteLine($"{i.ToString().PadRight(2)}: {mics[i]}");
                }

                WriteLine();
            }

            // Speakers
            if (speakers.Length > 0)
            {
                WriteLine("AVAILABLE SPEAKER SOURCES" + Underline);

                for (var i = 0; i < speakers.Length; ++i)
                {
                    WriteLine($"{i.ToString().PadRight(2)}: {speakers[i]}");
                }

                WriteLine();
            }
        }

        void Screens()
        {
            WriteLine("AVAILABLE SCREENS" + Underline);

            var j = 0;

            // First is Full Screen, Second is Screen Picker
            foreach (var screen in _platformServices.EnumerateScreens())
            {
                WriteLine($"{j.ToString().PadRight(2)}: {screen.DeviceName}");

                ++j;
            }

            WriteLine();
        }

        void Windows()
        {
            WriteLine("AVAILABLE WINDOWS" + Underline);

            // Window Picker is skipped automatically
            foreach (var source in _platformServices.EnumerateWindows())
            {
                WriteLine($"{source.Handle.ToString().PadRight(10)}: {source.Title}");
            }

            WriteLine();
        }

        void SharpAvi()
        {
            var sharpAviExists = ServiceProvider.FileExists("SharpAvi.dll");

            WriteLine($"SharpAvi Available: {(sharpAviExists ? "YES" : "NO")}");

            WriteLine();

            if (!sharpAviExists)
                return;

            WriteLine("SharpAvi ENCODERS" + Underline);

            var i = 0;

            foreach (var codec in _sharpAviWriterProvider)
            {
                WriteLine($"{i.ToString().PadRight(2)}: {codec}");
                ++i;
            }

            WriteLine();
        }

        void FFmpeg()
        {
            var ffmpegExists = FFmpegService.FFmpegExists;

            WriteLine($"FFmpeg Available: {(ffmpegExists ? "YES" : "NO")}");

            WriteLine();

            if (!ffmpegExists)
                return;

            WriteLine("FFmpeg ENCODERS" + Underline);

            var i = 0;

            foreach (var codec in _ffmpegWriterProvider)
            {
                WriteLine($"{i.ToString().PadRight(2)}: {codec}");
                ++i;
            }

            WriteLine();
        }
    }
}