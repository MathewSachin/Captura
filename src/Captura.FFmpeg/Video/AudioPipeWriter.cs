using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Captura.FFmpeg
{
    /// <summary>
    /// Queues ordered writes to a <see cref="Stream"/> so the caller only waits when more than MaxPendingBytes are pending.
    /// </summary>
    class AudioPipeWriter
    {
        readonly Stream _stream;
        readonly int _maxPendingBytes;

        Task _lastWrite = Task.CompletedTask;
        int _pendingBytes;

        public AudioPipeWriter(Stream Stream, int MaxPendingBytes)
        {
            _stream = Stream;
            _maxPendingBytes = MaxPendingBytes;
        }

        public void Write(byte[] Buffer, int Offset, int Length)
        {
            if (_lastWrite.IsFaulted || _pendingBytes > _maxPendingBytes)
            {
                Wait();
            }

            // The caller reuses its buffer.
            var data = new byte[Length];
            Array.Copy(Buffer, Offset, data, 0, Length);

            Interlocked.Add(ref _pendingBytes, Length);

            var previous = _lastWrite;

            _lastWrite = Task.Run(async () =>
            {
                await previous;
                await _stream.WriteAsync(data, 0, Length);
                Interlocked.Add(ref _pendingBytes, -Length);
            });
        }

        public void Wait() => _lastWrite.Wait();
    }
}
