using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Captura
{
    class HotKey : IDisposable
    {
        const string DllName = "user32";

        [DllImport(DllName)]
        static extern bool UnregisterHotKey(IntPtr Hwnd, int Id);

        [DllImport(DllName)]
        static extern bool RegisterHotKey(IntPtr Hwnd, int Id, int Modifiers, int VirtualKey);

        public const int Alt = 1;
        public const int Ctrl = 2;
        public const int Shift = 4;

        #region Fields
        WindowInteropHelper _host;
        readonly IntPtr _hWnd;
        bool _isDisposed;
        readonly int _identifier;
        #endregion

        public HotKey(Window Window, Keys Key, int Modifiers)
        {
            _host = new WindowInteropHelper(Window);
            _hWnd = _host.Handle;

            _identifier = new Random().Next();

            if (!RegisterHotKey(_hWnd, _identifier, Modifiers, (int)Key))
                throw new Exception("Unable to register hotkey!");

            ComponentDispatcher.ThreadPreprocessMessage += ProcessMessage;
        }

        void ProcessMessage(ref MSG Message, ref bool Handled)
        {
            if ((Message.message == 786) && (Message.wParam.ToInt32() == _identifier))
                Triggered?.Invoke();
        }

        public event Action Triggered;

        public void Dispose()
        {
            if (!_isDisposed)
            {
                UnregisterHotKey(_hWnd, _identifier);
                _host = null;
            }

            _isDisposed = true;
            Triggered = null;
        }
    }
}