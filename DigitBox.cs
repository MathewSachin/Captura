using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Captura
{
    public class DigitBox : TextBox
    {
        public DigitBox()
        {
            HorizontalContentAlignment = HorizontalAlignment.Right;
            VerticalContentAlignment = VerticalAlignment.Center;
            TextChanged += new TextChangedEventHandler(OnTextChanged);
            KeyDown += new KeyEventHandler(OnKeyDown);
        }

        public new string Text
        {
            get { return base.Text; }
            set { base.Text = LeaveOnlyNumbers(value); }
        }

        public int Value
        {
            get
            {
                try { return int.Parse(Text); }
                catch
                {
                    Text = "0";
                    return 0;
                }
            }
            set { Text = value.ToString(); }
        }

        public int Minimum { get; set; }

        #region Functions
        bool IsNumberKey(Key inKey)
        {
            if (inKey < Key.D0 || inKey > Key.D9)
                if (inKey < Key.NumPad0 || inKey > Key.NumPad9)
                    return false;

            return true;
        }

        bool IsDelOrBackspaceOrTabKey(Key inKey)
        {
            return inKey == Key.Delete || inKey == Key.Back || inKey == Key.Tab;
        }

        string LeaveOnlyNumbers(string inString)
        {
            string tmp = inString;

            foreach (char c in inString.ToCharArray())
                if (!System.Text.RegularExpressions.Regex.IsMatch(c.ToString(), "^[0-9]*$"))
                    tmp = tmp.Replace(c.ToString(), "");

            return tmp;
        }
        #endregion

        #region Event Functions
        protected void OnKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = !IsNumberKey(e.Key) && !IsDelOrBackspaceOrTabKey(e.Key);
        }

        protected void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            base.Text = LeaveOnlyNumbers(Text);
        }
        #endregion
    }
}
