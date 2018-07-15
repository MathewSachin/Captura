using System;
using Captura;

namespace Screna
{
    public class MultiRecorder : IRecorder
    {
        readonly IRecorder[] _recorders;

        public MultiRecorder(params IRecorder[] Recorders)
        {
            if (Recorders == null || Recorders.Length < 2)
            {
                throw new ArgumentException("Atleast two recorders required.", nameof(Recorders));
            }

            _recorders = Recorders;

            foreach (var recorder in Recorders)
            {
                recorder.ErrorOccurred += E => ErrorOccurred?.Invoke(E);
            }
        }

        public void Dispose()
        {
            foreach (var recorder in _recorders)
            {
                recorder.Dispose();
            }
        }

        public void Start()
        {
            foreach (var recorder in _recorders)
            {
                recorder.Start();
            }
        }

        public void Stop()
        {
            foreach (var recorder in _recorders)
            {
                recorder.Stop();
            }
        }

        public event Action<Exception> ErrorOccurred;
    }
}