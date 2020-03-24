using System;
using System.Threading;
using System.Threading.Tasks;

namespace Captura.Models
{
    public static class SingleInstanceManager
    {
        // ReSharper disable once NotAccessedField.Local
        static Mutex _mutex;
        // ReSharper disable once NotAccessedField.Local
        static Task _task;

        // Mutex allows us to know whether another instance is already open
        const string MutexId = "captura-mutex-304bae7c-e520-4bfe-a308-c99476062091";

        // EventWaitHandle allows us to communicate to a already running instance
        const string EventWaitHandleId = "captura-wait-304bae7c-e520-4bfe-a308-c99476062091";

        public static void SingleInstanceCheck()
        {
            _mutex = new Mutex(true, MutexId, out var createdNew);

            if (!createdNew)
            {
                // Bring the already running instance to the foreground
                var handle = CreateWaitHandle();
                handle.Set();

                // Exit duplicate instance
                Environment.Exit(0);
            }
        }

        static EventWaitHandle CreateWaitHandle()
        {
            return new EventWaitHandle(false, EventResetMode.AutoReset, EventWaitHandleId);
        }

        public static void StartListening(Action Callback)
        {
            // First instance listens for events from other instances in a loop
            _task = Task.Run(() =>
            {
                var waitHandle = CreateWaitHandle();

                while (true)
                {
                    waitHandle.WaitOne();

                    // Callback should bring first instance to foreground
                    Callback.Invoke();
                }
            });
        }
    }
}