using Captura.Models;
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
        
        public HotkeySelector(Hotkey Hotkey)
        {
            _hotkey = Hotkey;

            Content = _hotkey.ToString();
        }

        void HotkeyEdited(Key NewKey, Modifiers NewModifiers)
        {
            HotkeyEdited((Keys) KeyInterop.VirtualKeyFromKey(NewKey), NewModifiers);
        }

        void HotkeyEdited(Keys NewKey, Modifiers NewModifiers)
        {
            _hotkey.Change(NewKey, NewModifiers);

            // Red Text on Error
            Foreground = new SolidColorBrush(_hotkey.IsRegistered ? Colors.Black : Colors.DarkRed);

            Content = _hotkey.ToString();

            _editing = false;
        }
        
        protected override void OnClick()
        {
            base.OnClick();

            _editing = !_editing;

            Content = _editing ? "Press new Hotkey..." : _hotkey.ToString();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            CancelEditing();
        }

        void CancelEditing()
        {
            if (_editing)
            {
                _editing = false;
                Content = _hotkey.ToString();
            }
        }

        static bool IsValid(KeyEventArgs e)
        {
            return e.Key != Key.None // Some key must pe pressed
                && !e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Windows) // Windows Key is reserved by OS
                && e.Key != Key.LeftCtrl && e.Key != Key.RightCtrl // Modifier Keys alone are not supported
                && e.Key != Key.LeftAlt && e.Key != Key.RightAlt
                && e.Key != Key.LeftShift && e.Key != Key.RightShift;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            // Ignore Repeats
            if (e.IsRepeat)
            {
                e.Handled = true;
                return;
            }

            if (_editing)
            {
                // Suppress event propagation
                e.Handled = true;
                
                if (e.Key == Key.Escape)
                    CancelEditing();

                // Special Handling for Alt
                else if (e.Key == Key.System)
                {
                    if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
                        Content = "Alt + ...";
                    else HotkeyEdited(e.SystemKey, Modifiers.Alt);
                }

                else if (IsValid(e))
                    HotkeyEdited(e.Key, (Modifiers)e.KeyboardDevice.Modifiers);

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

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            // Ignore Repeats
            if (e.IsRepeat)
                return;

            if (_editing)
            {
                // Suppress event propagation
                e.Handled = true;

                // PrintScreen is not recognized in KeyDown
                if (e.Key == Key.Snapshot)
                    HotkeyEdited(Keys.PrintScreen, (Modifiers)e.KeyboardDevice.Modifiers);
                
                // Special handling for Alt
                else if (e.Key == Key.System && e.SystemKey == Key.Snapshot)
                    HotkeyEdited(Keys.PrintScreen, Modifiers.Alt);
            }

            base.OnPreviewKeyUp(e);
        }
    }
}