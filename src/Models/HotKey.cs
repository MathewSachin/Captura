using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Captura
{
    static class HotKey
    {
        const string DllName = "user32";

        [DllImport(DllName)]
        static extern bool UnregisterHotKey(IntPtr Hwnd, int Id);

        [DllImport(DllName)]
        static extern bool RegisterHotKey(IntPtr Hwnd, int Id, int Modifiers, int VirtualKey);

        const int Alt = 1;
        const int Ctrl = 2;
        const int Shift = 4;
        
        static int _recordId, _pauseId, _screenShotId;
        
        public static void RegisterAll()
        {
            var r = new Random();

            _recordId = r.Next();

            if (!RegisterHotKey(IntPtr.Zero, _recordId, Ctrl | Alt | Shift, (int)Keys.R))
                throw new Exception("Unable to register hotkey!");
            
            _pauseId = r.Next();

            if (!RegisterHotKey(IntPtr.Zero, _pauseId, Ctrl | Alt | Shift, (int)Keys.P))
                throw new Exception("Unable to register hotkey!");
            
            _screenShotId = r.Next();

            if (!RegisterHotKey(IntPtr.Zero, _screenShotId, Ctrl | Alt | Shift, (int)Keys.S))
                throw new Exception("Unable to register hotkey!");

            ComponentDispatcher.ThreadPreprocessMessage += ProcessMessage;
        }
        
        static void ProcessMessage(ref MSG Message, ref bool Handled)
        {
            if (Message.message == 786)
            {
                var id = Message.wParam.ToInt32();
                
                if (id == _recordId)
                {
                    var command = App.MainViewModel.RecordCommand;

                    if (command.CanExecute(null))
                        command.Execute(null);
                }
                else if (id == _pauseId)
                {
                    var command = App.MainViewModel.PauseCommand;

                    if (command.CanExecute(null))
                        command.Execute(null);
                }
                else if (id == _screenShotId)
                {
                    var command = App.MainViewModel.ScreenShotCommand;

                    if (command.CanExecute(null))
                        command.Execute(null);
                }
            }
        }
        
        public static void UnRegisterAll()
        {
            UnregisterHotKey(IntPtr.Zero, _recordId);
            UnregisterHotKey(IntPtr.Zero, _pauseId);
            UnregisterHotKey(IntPtr.Zero, _screenShotId);
        }
    }
}