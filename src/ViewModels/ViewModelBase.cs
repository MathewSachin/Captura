using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace Captura
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        public static bool AllExist(params string[] Paths)
        {
            foreach (var path in Paths)
                if (!File.Exists(path))
                    return false;

            return true;
        }
    }
}