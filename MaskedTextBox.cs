using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Captura
{
    public class MaskedTextBox : TextBox
    {
        public MaskedTextBox()
        {
            this.Mask = Mask;
            HorizontalContentAlignment = HorizontalAlignment.Right;
            VerticalContentAlignment = VerticalAlignment.Center;
            Value = 0;

            PreviewTextInput += TextBox_PreviewTextInput;
            DataObject.AddPastingHandler(this, (DataObjectPastingEventHandler)TextBoxPastingEventHandler);

            PreviewKeyDown += (s, e) =>
            {
                try
                {
                    if (Mask != MaskType.Any && e.Key == Key.Up && (double.IsNaN(Maximum) || Value < Maximum)) ++Value;
                    else if (Mask != MaskType.Any && e.Key == Key.Down && (double.IsNaN(Minimum) || Value > Minimum)) --Value;
                }
                catch { }
            };
        }

        public enum MaskType { Any, Integer, Decimal }

        #region MinimumValue Property
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(MaskedTextBox), new UIPropertyMetadata(double.NaN,
                new PropertyChangedCallback((dobj, e) => (dobj as MaskedTextBox).ValidateTextBox())));
        #endregion

        #region MaximumValue Property
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(MaskedTextBox), new UIPropertyMetadata(double.NaN,
                new PropertyChangedCallback((dobj, e) => (dobj as MaskedTextBox).ValidateTextBox())));
        #endregion

        #region Mask Property
        public MaskType Mask
        {
            get { return (MaskType)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Mask.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaskProperty =
            DependencyProperty.Register("Mask", typeof(MaskType), typeof(MaskedTextBox), new UIPropertyMetadata(MaskType.Any,
                new PropertyChangedCallback((dobj, e) => (dobj as MaskedTextBox).ValidateTextBox())));
        #endregion

        public double Value
        {
            get { return Convert.ToDouble(Text.Trim()); }
            set { Text = value.ToString(); }
        }

        #region Private Methods
        void ValidateTextBox()
        {
            if (Mask != MaskType.Any)
                Text = ValidateValue(Mask, Text, Minimum, Maximum);
        }

        void TextBoxPastingEventHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (Mask != MaskType.Any)
            {
                string clipboard = e.DataObject.GetData(typeof(string)) as string;
                clipboard = ValidateValue(Mask, clipboard, Minimum, Maximum);

                if (!string.IsNullOrEmpty(clipboard)) Text = clipboard;

                e.CancelCommand();
                e.Handled = true;
            }
        }

        void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Mask != MaskType.Any)
            {
                bool isValid = IsSymbolValid(Mask, e.Text);
                e.Handled = !isValid;

                if (isValid)
                {
                    int caret = CaretIndex;
                    string text = Text;
                    bool textInserted = false;
                    int selectionLength = 0;

                    if (SelectionLength > 0)
                    {
                        text = text.Substring(0, SelectionStart) +
                                text.Substring(SelectionStart + SelectionLength);
                        caret = SelectionStart;
                    }

                    if (e.Text == NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    {
                        while (true)
                        {
                            int ind = text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
                            if (ind == -1) break;

                            text = text.Substring(0, ind) + text.Substring(ind + 1);
                            if (caret > ind) caret--;
                        }

                        if (caret == 0)
                        {
                            text = "0" + text;
                            caret++;
                        }
                        else
                        {
                            if (caret == 1 && string.Empty + text[0] == NumberFormatInfo.CurrentInfo.NegativeSign)
                            {
                                text = NumberFormatInfo.CurrentInfo.NegativeSign + "0" + text.Substring(1);
                                caret++;
                            }
                        }

                        if (caret == text.Length)
                        {
                            selectionLength = 1;
                            textInserted = true;
                            text = text + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "0";
                            caret++;
                        }
                    }
                    else if (e.Text == NumberFormatInfo.CurrentInfo.NegativeSign)
                    {
                        textInserted = true;
                        if (Text.Contains(NumberFormatInfo.CurrentInfo.NegativeSign))
                        {
                            text = text.Replace(NumberFormatInfo.CurrentInfo.NegativeSign, string.Empty);
                            if (caret != 0) caret--;
                        }
                        else
                        {
                            text = NumberFormatInfo.CurrentInfo.NegativeSign + Text;
                            caret++;
                        }
                    }

                    if (!textInserted)
                    {
                        text = text.Substring(0, caret) + e.Text +
                            ((caret < Text.Length) ? text.Substring(caret) : string.Empty);

                        caret++;
                    }

                    try
                    {
                        double val = Convert.ToDouble(text);
                        double newVal = ValidateLimits(Minimum, Maximum, val);
                        if (val != newVal) text = newVal.ToString();

                        else if (val == 0)
                            if (!text.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
                                text = "0";
                    }
                    catch { text = "0"; }

                    while (text.Length > 1 && text[0] == '0' && string.Empty + text[1] != NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    {
                        text = text.Substring(1);
                        if (caret > 0) caret--;
                    }

                    while (text.Length > 2 && string.Empty + text[0] == NumberFormatInfo.CurrentInfo.NegativeSign && text[1] == '0' && string.Empty + text[2] != NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    {
                        text = NumberFormatInfo.CurrentInfo.NegativeSign + text.Substring(2);
                        if (caret > 1) caret--;
                    }

                    if (caret > text.Length) caret = text.Length;

                    Text = text;
                    CaretIndex = caret;
                    SelectionStart = caret;
                    SelectionLength = selectionLength;
                    e.Handled = true;
                }
            }
        }

        string ValidateValue(MaskType mask, string value, double min, double max)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            value = value.Trim();
            switch (mask)
            {
                case MaskType.Integer:
                    try
                    {
                        Convert.ToInt64(value);
                        return value;
                    }
                    catch { return string.Empty; }


                case MaskType.Decimal:
                    try
                    {
                        Convert.ToDouble(value);

                        return value;
                    }
                    catch { return string.Empty; }
            }

            return value;
        }

        double ValidateLimits(double min, double max, double value)
        {
            if (!min.Equals(double.NaN))
                if (value < min)
                    return min;

            if (!max.Equals(double.NaN))
                if (value > max)
                    return max;

            return value;
        }

        bool IsSymbolValid(MaskType mask, string str)
        {
            switch (mask)
            {
                case MaskType.Any:
                    return true;

                case MaskType.Integer:
                    if (str == NumberFormatInfo.CurrentInfo.NegativeSign)
                        return true;
                    break;

                case MaskType.Decimal:
                    if (str == NumberFormatInfo.CurrentInfo.NumberDecimalSeparator ||
                        str == NumberFormatInfo.CurrentInfo.NegativeSign)
                        return true;
                    break;
            }

            if (mask.Equals(MaskType.Integer) || mask.Equals(MaskType.Decimal))
            {
                foreach (char ch in str)
                    if (!Char.IsDigit(ch))
                        return false;

                return true;
            }

            return false;
        }
        #endregion
    }
}
