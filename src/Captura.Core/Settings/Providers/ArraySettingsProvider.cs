using Newtonsoft.Json;
using System.Configuration;
using System.IO;

namespace Captura
{
    public class ArraySettingsProvider : SettingsProviderBase
    {
        public override string Name => nameof(ArraySettingsProvider);

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            // Create new collection of values
            var values = new SettingsPropertyValueCollection();
            
            // Iterate through the settings to be retrieved
            foreach (SettingsProperty setting in collection)
            {
                object value = null;

                try
                {
                    var text = File.ReadAllText(Path.Combine(ServiceProvider.SettingsDir, $"{setting.Name}.json"));

                    value = JsonConvert.DeserializeObject(text, setting.PropertyType);
                }
                catch { }
                
                values.Add(new SettingsPropertyValue(setting)
                {
                    IsDirty = false,
                    PropertyValue = value
                });
            }
            return values;
        }
        
        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {   
            foreach (SettingsPropertyValue propertyValue in collection)
            {
                try
                {
                    var value = JsonConvert.SerializeObject(propertyValue.PropertyValue);

                    File.WriteAllText(Path.Combine(ServiceProvider.SettingsDir, $"{propertyValue.Name}.json"), value);
                }
                catch { }
            }
        }
    }
}