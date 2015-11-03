using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using ManagedWin32.Api;

namespace Captura
{
    public class KeyboardHookList : IDisposable
    {
        Random R;

        #region Fields
        WindowInteropHelper host;
        bool IsDisposed = false;
        Dictionary<int, Action> Keys = new Dictionary<int, Action>();
        #endregion

        public KeyboardHookList(Window Window)
        {
            host = new WindowInteropHelper(Window);

            ComponentDispatcher.ThreadPreprocessMessage += ProcessMessage;

            R = new Random(DateTime.Now.Millisecond);
        }

        public void Register(KeyCode Key, ModifierKeyCodes Modifiers, Action Callback)
        {
            int Identifier = R.Next();

            User32.RegisterHotKey(host.Handle, Identifier, Modifiers, Key);

            Keys.Add(Identifier, Callback);
        }

        void ProcessMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message == (int)WindowsMessage.HOTKEY)
                foreach (var Key in Keys)
                    if (msg.wParam.ToInt32() == Key.Key)
                        Key.Value();
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                ComponentDispatcher.ThreadPreprocessMessage -= ProcessMessage;

                foreach (var Identifier in Keys.Keys)
                    User32.UnregisterHotKey(host.Handle, Identifier);
                host = null;
            }
            IsDisposed = true;
        }
    }
}