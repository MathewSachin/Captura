using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Captura
{
    public partial class NumberBox
    {
        public DelegateCommand IncreaseCommand { get; }
        public DelegateCommand DecreaseCommand { get; }

        public NumberBox()
        {
            IncreaseCommand = new DelegateCommand(() => ++Value);
            DecreaseCommand = new DelegateCommand(() => --Value);

            InitializeComponent();

            UpdateCanSpin();
        }

        #region Range
        void UpdateCanSpin()
        {
            IncreaseCommand.RaiseCanExecuteChanged(Value < Maximum);
            DecreaseCommand.RaiseCanExecuteChanged(Value > Minimum);
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(int), typeof(NumberBox), new UIPropertyMetadata(0, OnMinChanged));

        static void OnMinChanged(DependencyObject O, DependencyPropertyChangedEventArgs PropertyChangedEventArgs)
        {
            if (O is NumberBox numberBox)
            {
                if (numberBox.Value < numberBox.Minimum)
                    numberBox.Value = numberBox.Minimum;

                numberBox.UpdateCanSpin();
            }
        }

        public int Minimum
        {
            get => (int)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(int), typeof(NumberBox), new UIPropertyMetadata(int.MaxValue, OnMaxChanged));

        static void OnMaxChanged(DependencyObject O, DependencyPropertyChangedEventArgs PropertyChangedEventArgs)
        {
            if (O is NumberBox numberBox)
            {
                if (numberBox.Value > numberBox.Maximum)
                    numberBox.Value = numberBox.Maximum;

                numberBox.UpdateCanSpin();
            }
        }

        public int Maximum
        {
            get => (int)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }
        #endregion

        bool _changing;

        #region Text
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(NumberBox), new UIPropertyMetadata("0", OnTextChanged));

        static void OnTextChanged(DependencyObject O, DependencyPropertyChangedEventArgs PropertyChangedEventArgs)
        {
            if (O is NumberBox numberBox)
            {
                numberBox.UpdateCanSpin();

                if (numberBox._changing)
                    return;

                numberBox._changing = true;

                try
                {
                    if (int.TryParse(numberBox.Text, out var val))
                    {
                        numberBox.Value = val;
                    }
                    else if (string.IsNullOrWhiteSpace(numberBox.Text))
                    {
                        numberBox.Text = (numberBox.Value = 0).ToString();
                    }
                    else numberBox.Text = numberBox.Value.ToString();
                }
                finally
                {
                    numberBox._changing = false;
                }
            }
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        #endregion

        #region Value
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(int), typeof(NumberBox),
                new FrameworkPropertyMetadata(0, OnValueChanged, OnCoerceValue)
                {
                    BindsTwoWayByDefault = true
                });

        static object OnCoerceValue(DependencyObject O, object BaseValue)
        {
            if (O is NumberBox numberBox && BaseValue is int val)
            {
                if (val < numberBox.Minimum)
                    return numberBox.Minimum;

                if (val > numberBox.Maximum)
                    return numberBox.Maximum;

                return val;
            }

            return BaseValue;
        }

        static void OnValueChanged(DependencyObject O, DependencyPropertyChangedEventArgs PropertyChangedEventArgs)
        {
            if (O is NumberBox numberBox)
            {
                numberBox.UpdateCanSpin();

                if (numberBox._changing)
                    return;

                numberBox._changing = true;

                try
                {
                    numberBox.Text = numberBox.Value.ToString();

                    numberBox.RaiseEvent(new RoutedPropertyChangedEventArgs<int>(
                        (int) PropertyChangedEventArgs.OldValue,
                        (int) PropertyChangedEventArgs.NewValue,
                        ValueChangedEvent));
                }
                finally
                {
                    numberBox._changing = false;
                }
            }
        }

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        
        static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(ValueChanged), RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<int>), typeof(NumberBox));

        public event RoutedPropertyChangedEventHandler<int> ValueChanged
        {
            add => AddHandler(ValueChangedEvent, value);
            remove => RemoveHandler(ValueChangedEvent, value);
        }
        #endregion

        void TextBox_OnGotKeyboardFocus(object Sender, KeyboardFocusChangedEventArgs E)
        {
            if (Sender is TextBox textBox)
                textBox.SelectAll();
        }

        void TextBox_OnPreviewMouseLeftButtonDown(object Sender, MouseButtonEventArgs E)
        {
            if (Sender is TextBox textBox)
            {
                if (!textBox.IsKeyboardFocused)
                {
                    textBox.Focus();

                    E.Handled = true;
                }
            }
        }
    }
}
