using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Captura.Hotkeys;
using Button = System.Windows.Controls.Button;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace Captura
{
    public class HotkeySelector : Button
    {
        bool _editing;

        static readonly SolidColorBrush RedBrush = new SolidColorBrush(WpfExtensions.ParseColor("#ef5350"));
        static readonly SolidColorBrush GreenBrush = new SolidColorBrush(WpfExtensions.ParseColor("#43a047"));

        static readonly SolidColorBrush WhiteBrush = new SolidColorBrush(Colors.White);

        public static readonly DependencyProperty HotkeyModelProperty = DependencyProperty.Register(nameof(HotkeyModel),
            typeof(Hotkey),
            typeof(HotkeySelector),
            new UIPropertyMetadata(HotkeyModelChangedCallback));

        static void HotkeyModelChangedCallback(DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
        {
            if (Sender is HotkeySelector selector && Args.NewValue is Hotkey hotkey)
            {
                selector.TextColor();

                hotkey.PropertyChanged += (S, E) =>
                {
                    if (E.PropertyName == nameof(Hotkey.IsActive))
                        selector.TextColor();
                };

                selector.Content = hotkey.ToString();
            }
        }

        public Hotkey HotkeyModel
        {
            get => (Hotkey) GetValue(HotkeyModelProperty);
            set => SetValue(HotkeyModelProperty, value);
        }

        void HotkeyEdited(Key NewKey, Modifiers NewModifiers)
        {
            HotkeyEdited((Keys) KeyInterop.VirtualKeyFromKey(NewKey), NewModifiers);
        }

        void TextColor()
        {
            if (HotkeyModel.IsActive)
            {
                Background = HotkeyModel.IsRegistered ? GreenBrush : RedBrush;

                Foreground = WhiteBrush;
            }
            else
            {
                ClearValue(BackgroundProperty);

                ClearValue(ForegroundProperty);
            }
        }

        void HotkeyEdited(Keys NewKey, Modifiers NewModifiers)
        {
            HotkeyModel.Change(NewKey, NewModifiers);

            // Red Text on Error
            TextColor();

            Content = HotkeyModel.ToString();

            _editing = false;
        }
        
        protected override void OnClick()
        {
            base.OnClick();

            _editing = !_editing;

            Content = _editing ? "Press new Hotkey..." : HotkeyModel.ToString();
        }

        protected override void OnLostFocus(RoutedEventArgs E)
        {
            base.OnLostFocus(E);

            CancelEditing();
        }

        void CancelEditing()
        {
            if (!_editing)
                return;

            _editing = false;
            Content = HotkeyModel.ToString();
        }

        static bool IsValid(KeyEventArgs E)
        {
            return E.Key != Key.None // Some key must pe pressed
                && !E.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Windows) // Windows Key is reserved by OS
                && E.Key != Key.LeftCtrl && E.Key != Key.RightCtrl // Modifier Keys alone are not supported
                && E.Key != Key.LeftAlt && E.Key != Key.RightAlt
                && E.Key != Key.LeftShift && E.Key != Key.RightShift;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs E)
        {
            // Ignore Repeats
            if (E.IsRepeat)
            {
                E.Handled = true;
                return;
            }

            if (_editing)
            {
                // Suppress event propagation
                E.Handled = true;

                switch (E.Key)
                {
                    case Key.Escape:
                        CancelEditing();
                        break;

                    case Key.System:
                        if (E.SystemKey == Key.LeftAlt || E.SystemKey == Key.RightAlt)
                            Content = "Alt + ...";
                        else HotkeyEdited(E.SystemKey, Modifiers.Alt);
                        break;

                    default:
                        if (IsValid(E))
                            HotkeyEdited(E.Key, (Modifiers)E.KeyboardDevice.Modifiers);

                        else
                        {
                            var modifiers = E.KeyboardDevice.Modifiers;

                            Content = "";

                            if (modifiers.HasFlag(ModifierKeys.Control))
                                Content += "Ctrl + ";

                            if (modifiers.HasFlag(ModifierKeys.Alt))
                                Content += "Alt + ";

                            if (modifiers.HasFlag(ModifierKeys.Shift))
                                Content += "Shift + ";

                            Content += "...";
                        }
                        break;
                }
            }

            base.OnPreviewKeyDown(E);
        }

        protected override void OnPreviewKeyUp(KeyEventArgs E)
        {
            // Ignore Repeats
            if (E.IsRepeat)
                return;

            if (_editing)
            {
                // Suppress event propagation
                E.Handled = true;

                // PrintScreen is not recognized in KeyDown
                switch (E.Key)
                {
                    case Key.Snapshot:
                        HotkeyEdited(Keys.PrintScreen, (Modifiers)E.KeyboardDevice.Modifiers);
                        break;

                    case Key.System when E.SystemKey == Key.Snapshot:
                        HotkeyEdited(Keys.PrintScreen, Modifiers.Alt);
                        break;
                }
            }

            base.OnPreviewKeyUp(E);
        }
    }
}