using System.Configuration;
using System.IO;
using System.Runtime.CompilerServices;

namespace Captura
{
    [SettingsProvider(typeof(JsonSettingsProvider))]
    public partial class Settings : ApplicationSettingsBase
    {
        public static Settings Instance { get; } = (Settings)Synchronized(new Settings());
        
        Settings() { }
        
        T Get<T>([CallerMemberName] string PropertyName = null) => (T)this[PropertyName];

        void Set<T>(T Value, [CallerMemberName] string PropertyName = null) => this[PropertyName] = Value;
        
        public void EnsureOutPath()
        {
            if (!Directory.Exists(OutPath))
                Directory.CreateDirectory(OutPath);
        }
    }
}
