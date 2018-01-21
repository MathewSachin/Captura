using System.Windows.Data;

namespace Captura
{
    public class Loc : Binding
    {
        public Loc(string Name) : base($"[{Name}]")
        {
            Mode = BindingMode.OneWay;
            Source = LanguageManager.Instance;
        }
    }
}