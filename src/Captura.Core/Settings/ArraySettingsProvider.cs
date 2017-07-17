using Captura.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;

namespace Captura
{
    public class ArraySettingsProvider : SettingsProvider, IApplicationSettingsProvider
    {
        public override string ApplicationName
        {
            get => Resources.AppName;
            set { }
        }

        public override void Initialize(string name, NameValueCollection collection)
        {
            base.Initialize(ApplicationName, collection);
        }

        public override string Name => nameof(ArraySettingsProvider);

        static readonly string Dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Resources.AppName);

        public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
        {
            return new SettingsPropertyValue(property) { PropertyValue = property.DefaultValue };
        }

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

        public void Reset(SettingsContext context)
        {
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

        public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
        {
        }
    }
}