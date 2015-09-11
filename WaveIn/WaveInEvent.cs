using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace NWaveIn
{
    /// <summary>
    /// Recording using waveIn api with event callbacks.
    /// Use this for recording in non-gui applications
    /// Events are raised as recorded buffers are made available
    /// </summary>
    public class WaveInEvent
    {
        readonly AutoResetEvent callbackEvent;
        readonly SynchronizationContext syncContext;
        IntPtr waveInHandle;
        volatile bool recording;
        WaveInBuffer[] buffers;

        /// <summary>
        /// Indicates recorded data is available 
        /// </summary>
        public event EventHandler<WaveInEventArgs> DataAvailable;

        /// <summary>
        /// Indicates that all recorded data has now been received.
        /// </summary>
        public event Action<object, Exception> RecordingStopped;

        /// <summary>
        /// Prepares a Wave input device for recording
        /// </summary>
        public WaveInEvent()
        {
            callbackEvent = new AutoResetEvent(false);
            syncContext = SynchronizationContext.Current;
            DeviceNumber = 0;
            WaveFormat = new WaveFormat(8000, 16, 1);
            BufferMilliseconds = 100;
            NumberOfBuffers = 3;
        }

        /// <summary>
        /// Returns the number of Wave In devices available in the system
        /// </summary>
        public static int DeviceCount { get { return WaveInterop.waveInGetNumDevs(); } }

        /// <summary>
        /// Retrieves the capabilities of a waveIn device
        /// </summary>
        /// <param name="devNumber">Device to test</param>
        /// <returns>The WaveIn device capabilities</returns>
        public static WaveInCapabilities GetCapabilities(int devNumber)
        {
            WaveInCapabilities caps = new WaveInCapabilities();
            int structSize = Marshal.SizeOf(caps);
            MmException.Try(WaveInterop.waveInGetDevCaps((IntPtr)devNumber, out caps, structSize), "waveInGetDevCaps");
            return caps;
        }

        /// <summary>
        /// Milliseconds for the buffer. Recommended value is 100ms
        /// </summary>
        public int BufferMilliseconds { get; set; }

        /// <summary>
        /// Number of Buffers to use (usually 2 or 3)
        /// </summary>
        public int NumberOfBuffers { get; set; }

        /// <summary>
        /// The device number to use
        /// </summary>
        public int DeviceNumber { get; set; }

        void CreateBuffers()
        {
            // Default to three buffers of 100ms each
            int bufferSize = BufferMilliseconds * WaveFormat.AverageBytesPerSecond / 1000;
            if (bufferSize % WaveFormat.BlockAlign != 0) bufferSize -= bufferSize % WaveFormat.BlockAlign;

            buffers = new WaveInBuffer[NumberOfBuffers];
            for (int n = 0; n < buffers.Length; n++) buffers[n] = new WaveInBuffer(waveInHandle, bufferSize);
        }

        void OpenWaveInDevice()
        {
            CloseWaveInDevice();
            MmResult result = WaveInterop.waveInOpenWindow(out waveInHandle, (IntPtr)DeviceNumber, WaveFormat,
                callbackEvent.SafeWaitHandle.DangerousGetHandle(), IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackEvent);
            MmException.Try(result, "waveInOpen");
            CreateBuffers();
        }

        /// <summary>
        /// Start recording
        /// </summary>
        public void StartRecording()
        {
            if (recording)
                throw new InvalidOperationException("Already recording");
            OpenWaveInDevice();
            MmException.Try(WaveInterop.waveInStart(waveInHandle), "waveInStart");
            recording = true;
            ThreadPool.QueueUserWorkItem((state) => RecordThread(), null);
        }

        void RecordThread()
        {
            Exception exception = null;
            try { DoRecording(); }
            catch (Exception e) { exception = e; }
            finally
            {
                recording = false;
                RaiseRecordingStoppedEvent(exception);
            }
        }

        void DoRecording()
        {
            foreach (var buffer in buffers) if (!buffer.InQueue) buffer.Reuse();

            while (recording)
            {
                if (callbackEvent.WaitOne())
                {
                    // requeue any buffers returned to us
                    if (recording)
                    {
                        foreach (var buffer in buffers)
                        {
                            if (buffer.Done)
                            {
                                if (DataAvailable != null) DataAvailable(this, new WaveInEventArgs(buffer.Data, buffer.BytesRecorded));
                                buffer.Reuse();
                            }
                        }
                    }
                }
            }
        }

        void RaiseRecordingStoppedEvent(Exception e)
        {
            var handler = RecordingStopped;
            if (handler != null)
            {
                if (syncContext == null) handler(this, e);
                else syncContext.Post(state => handler(this, e), null);
            }
        }
        /// <summary>
        /// Stop recording
        /// </summary>
        public void StopRecording()
        {
            recording = false;
            this.callbackEvent.Set(); // signal the thread to exit
            MmException.Try(WaveInterop.waveInStop(waveInHandle), "waveInStop");
        }

        /// <summary>
        /// WaveFormat we are recording in
        /// </summary>
        public WaveFormat WaveFormat { get; set; }

        /// <summary>
        /// Dispose pattern
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (recording) StopRecording();

                CloseWaveInDevice();
            }
        }

        void CloseWaveInDevice()
        {
            // Some drivers need the reset to properly release buffers
            WaveInterop.waveInReset(waveInHandle);
            if (buffers != null)
            {
                for (int n = 0; n < buffers.Length; n++) buffers[n].Dispose();
                buffers = null;
            }
            WaveInterop.waveInClose(waveInHandle);
            waveInHandle = IntPtr.Zero;
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
