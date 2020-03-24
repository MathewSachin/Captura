using System;
using System.Threading;
using System.Threading.Tasks;

namespace Captura.Models
{
    public static class SingleInstanceManager
    {
        static Mutex _mutex;

        const string MutexId = "captura-mutex-304bae7c-e520-4bfe-a308-c99476062091";
        const string EventWaitHandleId = "captura-wait-304bae7c-e520-4bfe-a308-c99476062091";

        public static void SingleInstanceCheck()
        {
            _mutex = new Mutex(true, MutexId, out var createdNew);

            if (!createdNew)
            {
                var handle = CreateWaitHandle();

                handle.Set();

                Environment.Exit(0);
            }
        }

        static Task _task;

        static EventWaitHandle CreateWaitHandle()
        {
            return new EventWaitHandle(false, EventResetMode.AutoReset, EventWaitHandleId);
        }

        public static void StartListening(Action Callback)
        {
            _task = Task.Run(() =>
            {
                var waitHandle = CreateWaitHandle();

                while (true)
                {
                    waitHandle.WaitOne();

                    Callback.Invoke();
                }
            });
        }
    }
}