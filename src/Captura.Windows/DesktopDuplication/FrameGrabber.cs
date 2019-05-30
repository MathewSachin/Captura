﻿using System;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.DXGI;

namespace DesktopDuplication
{
    public class FrameGrabber : IDisposable
    {
        readonly OutputDuplication _deskDupl;

        AcquireResult _acquireResult;
        readonly object _acquireResultLock = new object();

        Task _acquireTask;

        public FrameGrabber(OutputDuplication DeskDupl)
        {
            _deskDupl = DeskDupl;
        }

        void BeginAcquireFrame()
        {
            const int timeout = 50;

            _acquireTask = Task.Run(() =>
            {
                try
                {
                    var result = _deskDupl.TryAcquireNextFrame(timeout, out var frameInfo, out var desktopResource);

                    if (result == ResultCode.WaitTimeout)
                    {
                        lock (_acquireResultLock)
                        {
                            _acquireResult = null;
                        }

                        BeginAcquireFrame();
                    }
                    else
                    {
                        lock (_acquireResultLock)
                        {
                            _acquireResult = new AcquireResult(result, frameInfo, desktopResource);
                        }
                    }
                }
                catch
                {
                    lock (_acquireResultLock)
                    {
                        _acquireResult = new AcquireResult(Result.Fail);
                    }
                }
            });
        }

        AcquireResult GetAcquireResult()
        {
            if (_acquireTask == null)
            {
                BeginAcquireFrame();

                return null;
            }

            _acquireTask.Wait();

            lock (_acquireResultLock)
            {
                var val = _acquireResult;

                _acquireResult = null;

                return val;
            }
        }

        public AcquireResult Grab()
        {
            return GetAcquireResult();
        }

        public void Release()
        {
            _deskDupl.ReleaseFrame();

            BeginAcquireFrame();
        }

        public void Dispose()
        {
            try { _acquireTask?.Wait(); }
            catch { }
        }
    }
}