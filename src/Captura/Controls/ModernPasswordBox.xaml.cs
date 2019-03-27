using System.Windows;

namespace Captura
{
    public partial class ModernPasswordBox
    {
        public ModernPasswordBox()
        {
            InitializeComponent();

            PswBox.PasswordChanged += (S, E) => Password = PswBox.Password;
        }

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(
            nameof(Password),
            typeof(string),
            typeof(ModernPasswordBox),
            new UIPropertyMetadata((S, E) =>
            {
                if (S is ModernPasswordBox modernPswBox && E.NewValue is string psw)
                {
                    if (modernPswBox.PswBox.Password != psw)
                        modernPswBox.PswBox.Password = psw;
                }
            }));

        public string Password
        {
            get => (string) GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        public static readonly DependencyProperty PasswordVisibleProperty = DependencyProperty.Register(
            nameof(PasswordVisible),
            typeof(bool),
            typeof(ModernPasswordBox));

        public bool PasswordVisible
        {
            get => (bool)GetValue(PasswordVisibleProperty);
            set => SetValue(PasswordVisibleProperty, value);
        }

        void ToggleVisibility(object Sender, RoutedEventArgs E)
        {
            PasswordVisible = !PasswordVisible;
        }
    }
}
