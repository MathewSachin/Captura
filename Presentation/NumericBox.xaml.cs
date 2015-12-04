using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Captura
{
    public partial class NumericBox : UserControl
    {
        public static readonly ICommand IncreaseCommand = new RoutedUICommand(),
            DecreaseCommand = new RoutedUICommand();

        public NumericBox()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(IncreaseCommand,
                (s, e) => Value++,
                (s, e) => e.CanExecute = Value < Maximum));

            CommandBindings.Add(new CommandBinding(DecreaseCommand,
                (s, e) => Value--,
                (s, e) => e.CanExecute = Value > Minimum));

            TextBOX.InputBindings.Add(new InputBinding(IncreaseCommand, new KeyGesture(Key.Up)));
            TextBOX.InputBindings.Add(new InputBinding(DecreaseCommand, new KeyGesture(Key.Down)));

            Last = Value;
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(int), typeof(NumericBox), new UIPropertyMetadata(0));

        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(NumericBox), new UIPropertyMetadata(int.MaxValue));

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(NumericBox), new UIPropertyMetadata(0));

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set 
            {
                if (Value != value)
                {
                    TextBOX.Text = value.ToString();
                    SetValue(ValueProperty, value);
                }
            }
        }

        int Last = 0;

        void TextBOX_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBOX.Text))
                return;

            try { Value = int.Parse(TextBOX.Text); }
            catch { Value = Last; }

            if (Value > Maximum) Value = Maximum;
            else if (Value < Minimum) Value = Minimum;

            Last = Value;
        }

        void TextBOX_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBOX.Text)) Value = 0;
        }
    }
}
