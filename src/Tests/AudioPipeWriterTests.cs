using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Captura.FFmpeg;
using Xunit;

namespace Captura.Tests
{
    [Collection(nameof(Tests))]
    public class AudioPipeWriterTests
    {
        static readonly TimeSpan MaxWait = TimeSpan.FromSeconds(1);

        // Blocks every write until the gate opens, like a pipe FFmpeg has stopped reading.
        class BlockingStream : Stream
        {
            public readonly ManualResetEventSlim Gate = new ManualResetEventSlim();
            public readonly MemoryStream Received = new MemoryStream();
            public Exception Failure;
            public int MaxConcurrentWrites;

            int _concurrentWrites;

            public override void Write(byte[] Buffer, int Offset, int Count)
            {
                var concurrent = Interlocked.Increment(ref _concurrentWrites);

                lock (Received)
                    MaxConcurrentWrites = Math.Max(MaxConcurrentWrites, concurrent);

                try
                {
                    Gate.Wait();

                    if (Failure != null)
                        throw Failure;

                    lock (Received)
                        Received.Write(Buffer, Offset, Count);
                }
                finally
                {
                    Interlocked.Decrement(ref _concurrentWrites);
                }
            }

            // Unlike the base Stream, a pipe does not serialize async writes.
            public override Task WriteAsync(byte[] Buffer, int Offset, int Count, CancellationToken CancellationToken)
            {
                return Task.Run(() => Write(Buffer, Offset, Count));
            }

            public override bool CanRead => false;
            public override bool CanSeek => false;
            public override bool CanWrite => true;
            public override long Length => throw new NotSupportedException();
            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }
            public override void Flush() { }
            public override int Read(byte[] Buffer, int Offset, int Count) => throw new NotSupportedException();
            public override long Seek(long Offset, SeekOrigin Origin) => throw new NotSupportedException();
            public override void SetLength(long Value) => throw new NotSupportedException();
        }

        static void Run(Action Action)
        {
            Assert.True(Task.Run(Action).Wait(MaxWait), $"Blocked for more than {MaxWait}");
        }

        [Fact]
        public void WriteDoesNotWaitForTheStream()
        {
            var stream = new BlockingStream();
            var writer = new AudioPipeWriter(stream, 1000);

            try
            {
                Run(() =>
                {
                    writer.Write(new byte[10], 0, 10);
                    writer.Write(new byte[10], 0, 10);
                });
            }
            finally
            {
                stream.Gate.Set();
            }
        }

        [Fact]
        public void BytesArriveInOrderAndAreCopied()
        {
            var stream = new BlockingStream();
            var writer = new AudioPipeWriter(stream, 1000);
            var buffer = new byte[] { 1, 2, 3, 4, 5 };

            try
            {
                Run(() =>
                {
                    writer.Write(buffer, 1, 3);
                    new byte[] { 6, 7, 8, 9, 10 }.CopyTo(buffer, 0);

                    writer.Write(buffer, 0, 2);
                    new byte[] { 0, 0, 0, 0, 0 }.CopyTo(buffer, 0);
                });

                // The second write must not reach the stream while the first is blocked.
                Assert.True(SpinWait.SpinUntil(() => stream.MaxConcurrentWrites > 0, MaxWait), "First write did not reach the stream");
                Thread.Sleep(300);
                Assert.Equal(1, stream.MaxConcurrentWrites);
            }
            finally
            {
                stream.Gate.Set();
            }

            Run(writer.Wait);

            Assert.Equal(new byte[] { 2, 3, 4, 6, 7 }, stream.Received.ToArray());
        }

        [Fact]
        public void BlocksWhenPendingExceedsLimit()
        {
            var stream = new BlockingStream();
            var writer = new AudioPipeWriter(stream, 4);

            try
            {
                Run(() => writer.Write(new byte[8], 0, 8));

                var write = Task.Run(() => writer.Write(new byte[1], 0, 1));

                Assert.False(write.Wait(300), "Write past the limit did not wait for the stream");

                stream.Gate.Set();

                Assert.True(write.Wait(MaxWait), "Write did not resume after the stream drained");
            }
            finally
            {
                stream.Gate.Set();
            }

            Run(writer.Wait);

            Assert.Equal(9, stream.Received.Length);

            // Written bytes no longer count as pending: 0 -> 4 -> 5 stays within the limit.
            stream.Gate.Reset();

            try
            {
                Run(() =>
                {
                    writer.Write(new byte[4], 0, 4);
                    writer.Write(new byte[1], 0, 1);
                });
            }
            finally
            {
                stream.Gate.Set();
            }
        }

        [Fact]
        public void FailedWriteSurfacesOnNextWrite()
        {
            var stream = new BlockingStream { Failure = new IOException("Pipe is broken.") };
            stream.Gate.Set();

            var writer = new AudioPipeWriter(stream, 1000);

            Run(() => writer.Write(new byte[4], 0, 4));

            var e = Assert.Throws<AggregateException>(() => Run(writer.Wait));
            Assert.IsType<IOException>(e.GetBaseException());

            e = Assert.Throws<AggregateException>(() => Run(() => writer.Write(new byte[4], 0, 4)));
            Assert.IsType<IOException>(e.GetBaseException());
        }
    }
}
