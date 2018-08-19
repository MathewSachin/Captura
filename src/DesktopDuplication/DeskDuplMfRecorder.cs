﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Captura;
using DesktopDuplication;

namespace Screna
{
    public class DeskDuplMfRecorder : IRecorder
    {
        #region Fields
        readonly DeskDuplMediaFoundation _deskDupl;

        readonly Task _recordTask;

        readonly ManualResetEvent _stopCapturing = new ManualResetEvent(false),
            _continueCapturing = new ManualResetEvent(false);
        #endregion
        
        public DeskDuplMfRecorder(DeskDuplMediaFoundation DeskDupl)
        {
            _deskDupl = DeskDupl;

            // Not Actually Started, Waits for _continueCapturing to be Set
            _recordTask = Task.Factory.StartNew(async () => await Record());
        }

        /// <summary>
        /// Start recording.
        /// </summary>
        public void Start()
        {
            _continueCapturing.Set();
        }

        bool _disposed;

        void Dispose(bool ErrorOccurred)
        {
            if (_disposed)
                return;

            _disposed = true;

            // Resume if Paused
            _continueCapturing.Set();
            
            _stopCapturing.Set();

            _captureTask?.Wait();

            if (!ErrorOccurred)
                _recordTask.Wait();

            _deskDupl.Dispose();

            _continueCapturing.Dispose();
            _stopCapturing.Dispose();
        }

        /// <summary>
        /// Frees resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(false);
        }

        /// <summary>
        /// Override this method with the code to pause recording.
        /// </summary>
        public void Stop()
        {
            _continueCapturing.Reset();
        }

        Task _captureTask;

        async Task Record()
        {
            var frameInterval = TimeSpan.FromSeconds(1.0 / _deskDupl.Fps);

            try
            {
                while (!_stopCapturing.WaitOne(0) && _continueCapturing.WaitOne())
                {
                    var timestamp = DateTime.Now;

                    if (_captureTask != null)
                    {
                        await _captureTask;
                    }

                    _captureTask = Task.Factory.StartNew(() => _deskDupl.Capture());

                    //var timeTillNextFrame = timestamp + frameInterval - DateTime.Now;

                    //if (timeTillNextFrame > TimeSpan.Zero)
                    //    Thread.Sleep(timeTillNextFrame);
                }
            }
            catch (Exception e)
            {
                ErrorOccurred?.Invoke(e);

                Dispose(true);
            }
        }

        /// <summary>
        /// Fired when an Error occurs.
        /// </summary>
        public event Action<Exception> ErrorOccurred;
    }
}
