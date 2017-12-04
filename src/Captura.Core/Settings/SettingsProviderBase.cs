using System.Collections.Specialized;
using System.Configuration;

namespace Captura
{
    public abstract class SettingsProviderBase : SettingsProvider, IApplicationSettingsProvider
    {
        public override string ApplicationName
        {
            get => "Captura";
            set { }
        }
        
        public override void Initialize(string name, NameValueCollection collection)
        {
            base.Initialize(ApplicationName, collection);
        }
        
        public virtual SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
        {
            return new SettingsPropertyValue(property) { PropertyValue = property.DefaultValue };
        }
        
        public virtual void Reset(SettingsContext context) { }
        
        public virtual void Upgrade(SettingsContext context, SettingsPropertyCollection properties) { }
    }
}