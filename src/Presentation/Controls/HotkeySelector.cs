using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Button = System.Windows.Controls.Button;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace Captura
{
    public class HotkeySelector : Button
    {
        bool _editing;

        readonly Hotkey _hotkey;
        Keys _newKey;
        Modifiers _newModifiers;
        
        public HotkeySelector(Hotkey Hotkey)
        {
            _hotkey = Hotkey;

            Content = _hotkey.ToString();
        }

        void HotkeyEdited()
        {
            _hotkey.Change(_newKey, _newModifiers);

            Foreground = new SolidColorBrush(_hotkey.IsRegistered ? Colors.Black : Colors.DarkRed);

            Content = _hotkey.ToString();

            _editing = false;
        }
        
        protected override void OnClick()
        {
            base.OnClick();

            _editing = !_editing;

            Content = _editing ? "Press Hotkey" : _hotkey.ToString();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            if (_editing)
            {
                _editing = false;
                Content = _hotkey.ToString();
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.IsRepeat)
            {
                e.Handled = true;
                return;
            }

            if (_editing)
            {
                e.Handled = true;
                
                if (e.Key == Key.Escape)
                {                
                    _newKey = Keys.None;
                    _newModifiers = 0;

                    HotkeyEdited();
                }
                else if (e.Key != Key.None && !e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Windows)
                    && e.Key != Key.LeftCtrl && e.Key != Key.RightCtrl
                    && e.Key != Key.LeftAlt && e.Key != Key.RightAlt
                    && e.Key != Key.LeftShift && e.Key != Key.RightShift)
                {
                    _newKey = (Keys) KeyInterop.VirtualKeyFromKey(e.Key);
                    _newModifiers = (Modifiers)e.KeyboardDevice.Modifiers;

                    HotkeyEdited();
                }
                else
                {
                    var modifiers = e.KeyboardDevice.Modifiers;

                    Content = "";

                    if (modifiers.HasFlag(ModifierKeys.Control))
                        Content += "Ctrl + ";

                    if (modifiers.HasFlag(ModifierKeys.Alt))
                        Content += "Alt + ";

                    if (modifiers.HasFlag(ModifierKeys.Shift))
                        Content += "Shift + ";

                    Content += "...";
                }
            }

            base.OnPreviewKeyDown(e);
        }
    }
}