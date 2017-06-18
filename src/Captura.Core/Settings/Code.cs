using System.Configuration;
using System.Runtime.CompilerServices;

namespace Captura
{
    public partial class Settings : ApplicationSettingsBase
    {
        public static Settings Instance { get; } = (Settings)Synchronized(new Settings());
        
        Settings()
        {
            // Upgrade settings from Previous version
            if (UpdateRequired)
            {
                Upgrade();
                UpdateRequired = false;
            }
        }
        
        T Get<T>([CallerMemberName] string PropertyName = null) => (T)this[PropertyName];

        void Set<T>(T Value, [CallerMemberName] string PropertyName = null) => this[PropertyName] = Value;
        
        [UserScopedSetting]
        [DefaultSettingValue("True")]
        public bool UpdateRequired
        {
            get => Get<bool>();
            set => Set(value);
        }
    }
}
