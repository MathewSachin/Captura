using System.Windows.Data;

namespace Captura
{
    public class Setting : Binding
    {
        public Setting(string Name) : base(Name)
        {
            Mode = BindingMode.TwoWay;
            Source = Settings.Instance;
        }
    }
}