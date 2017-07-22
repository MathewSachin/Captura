using Captura.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Configuration;
using System.IO;

namespace Captura
{
    public class ArraySettingsProvider : SettingsProviderBase
    {
        static readonly string Dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Resources.AppName);
        
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
                    var text = File.ReadAllText(Path.Combine(Dir, $"{setting.Name}.json"));

                    value = JsonConvert.DeserializeObject(text, setting.PropertyType);
                }
                catch { }
                
                values.Add(new SettingsPropertyValue(setting)
                {
                    IsDirty = false,
                    PropertyValue = value,
                });
            }
            return values;
        }
        
        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            var enumConverter = new StringEnumConverter
            {
                AllowIntegerValues = false
            };
                        
            foreach (SettingsPropertyValue propertyValue in collection)
            {
                try
                {
                    var value = JsonConvert.SerializeObject(propertyValue.PropertyValue, Formatting.Indented, enumConverter);

                    File.WriteAllText(Path.Combine(Dir, $"{propertyValue.Name}.json"), value);
                }
                catch { }
            }
        }
    }
}