using System;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace Captura
{
    [Flags]
    enum ModifierKeyCodes : uint
    {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8
    }

    /// <summary>
    /// Virtual Key Codes
    /// </summary>
    enum VirtualKeyCodes : uint
    {
        A = 65,
        B = 66,
        C = 67,
        D = 68,
        E = 69,
        F = 70,
        G = 71,
        H = 72,
        I = 73,
        J = 74,
        K = 75,
        L = 76,
        M = 77,
        N = 78,
        O = 79,
        P = 80,
        Q = 81,
        R = 82,
        S = 83,
        T = 84,
        U = 85,
        V = 86,
        W = 87,
        X = 88,
        Y = 89,
        Z = 90
    }

    class KeyboardHook : IDisposable
    {
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, ModifierKeyCodes fdModifiers, VirtualKeyCodes vk);

        #region Fields
        WindowInteropHelper host;
        bool IsDisposed = false;
        int Identifier;

        public Window Window { get; private set; }

        public VirtualKeyCodes Key { get; private set; }

        public ModifierKeyCodes Modifiers { get; private set; }
        #endregion
        
        public KeyboardHook(Window Window, VirtualKeyCodes Key, ModifierKeyCodes Modifiers)
        {
            this.Key = Key;
            this.Modifiers = Modifiers;

            this.Window = Window;
            host = new WindowInteropHelper(Window);

            Identifier = Window.GetHashCode();

            RegisterHotKey(host.Handle, Identifier, Modifiers, Key);

            ComponentDispatcher.ThreadPreprocessMessage += ProcessMessage;
        }

        void ProcessMessage(ref MSG msg, ref bool handled)
        {
            if ((msg.message == 786) && (msg.wParam.ToInt32() == Identifier) && (Triggered != null))
                Triggered();
        }

        public event Action Triggered;

        public void Dispose()
        {
            if (!IsDisposed)
            {
                ComponentDispatcher.ThreadPreprocessMessage -= ProcessMessage;

                UnregisterHotKey(host.Handle, Identifier);
                Window = null;
                host = null;
            }
            IsDisposed = true;
        }
    }
}