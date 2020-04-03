using System;
using System.Threading;
using System.Threading.Tasks;

namespace Captura.Audio
{
    public class AudioRecorder : IRecorder
    {
        IAudioFileWriter _audioWriter;
        IAudioProvider _audioProvider;

        readonly ManualResetEvent _continueEvent = new ManualResetEvent(false),
            _stopEvent = new ManualResetEvent(false);

        byte[] _buffer;
        const int ReadInterval = 200;
        readonly Task _loopTask;

        public AudioRecorder(IAudioFileWriter AudioWriter, IAudioProvider AudioProvider)
        {
            _audioWriter = AudioWriter ?? throw new ArgumentNullException(nameof(AudioWriter));
            _audioProvider = AudioProvider ?? throw new ArgumentNullException(nameof(AudioProvider));

            var wf = _audioProvider.WaveFormat;

            var bufferSize = (int)
            (
                (ReadInterval / 1000.0)
                * wf.SampleRate
                * wf.Channels
                * (wf.BitsPerSample / 8.0)
            );

            _buffer = new byte[bufferSize];

            _loopTask = Task.Factory.StartNew(Loop, TaskCreationOptions.LongRunning);
        }

        public void Dispose()
        {
            _continueEvent.Set();
            _stopEvent.Set();

            _loopTask.Wait();

            _buffer = null;

            _audioWriter.Dispose();
            _audioWriter = null;

            _audioProvider.Dispose();
            _audioProvider = null;
        }

        public void Start()
        {
            _audioProvider.Start();

            _continueEvent.Set();
        }

        public void Stop()
        {
            _continueEvent.Reset();

            _audioProvider.Stop();
        }

        void Loop()
        {
            bool CanContinue()
            {
                try
                {
                    return _continueEvent.WaitOne() && !_stopEvent.WaitOne(0);
                }
                catch (ObjectDisposedException)
                {
                    return false;
                }
            }

            while (CanContinue())
            {
                var read = _audioProvider.Read(_buffer, 0, _buffer.Length);

                _audioWriter.Write(_buffer, 0, read);

                Thread.Sleep(ReadInterval);
            }
        }

        public event Action<Exception> ErrorOccurred;
    }
}