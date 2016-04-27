using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Captura
{
    [Flags]
    public enum ModifierKeyCodes : uint
    {
        Alt = 1,
        Control = 2,
        Shift = 4
    }

    public enum KeyCode { R = 0x52, S = 0x53 }

    public class KeyboardHookList : IDisposable
    {
        readonly Random _r;

        [DllImport("user32.dll")]
        static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, ModifierKeyCodes fdModifiers, KeyCode vk);

        #region Fields
        WindowInteropHelper _host;
        readonly IntPtr _handle;
        bool _isDisposed;
        readonly Dictionary<int, Action> _keys = new Dictionary<int, Action>();
        #endregion

        ~KeyboardHookList() { Dispose(); }

        public KeyboardHookList(Window Window)
        {
            _host = new WindowInteropHelper(Window);
            _handle = _host.Handle;

            ComponentDispatcher.ThreadPreprocessMessage += ProcessMessage;

            _r = new Random(DateTime.Now.Millisecond);
        }

        public void Register(KeyCode Key, ModifierKeyCodes Modifiers, Action Callback)
        {
            var identifier = _r.Next();

            RegisterHotKey(_handle, identifier, Modifiers, Key);

            _keys.Add(identifier, Callback);
        }

        void ProcessMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message != 786)
                return;

            foreach (var key in _keys)
                if (msg.wParam.ToInt32() == key.Key)
                    key.Value();
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                foreach (var identifier in _keys.Keys)
                    UnregisterHotKey(_handle, identifier);
                _host = null;
            }
            _isDisposed = true;
        }
    }
}