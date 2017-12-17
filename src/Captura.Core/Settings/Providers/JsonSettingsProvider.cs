using Newtonsoft.Json.Linq;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Captura
{
    public class JsonSettingsProvider : SettingsProviderBase
    {
        readonly JObject _settingsJson;

        readonly string _fileName;

        public override string Name => nameof(JsonSettingsProvider);

        public JsonSettingsProvider()
        {
            _fileName = Path.Combine(ServiceProvider.SettingsDir, "Settings.json");
            
            _settingsJson = LoadOrCreateSettings(_fileName);
        }
        
        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection properties)
        {
            // Only dirty settings are included in properties, and only ones relevant to this provider
            foreach (SettingsPropertyValue propertyValue in properties)
            {
                SetValue(propertyValue);
            }

            try
            {
                var jobj = new JObject(_settingsJson.Properties().OrderBy(j => j.Name).ToArray());

                File.WriteAllText(_fileName, jobj.ToString());
            }
            catch
            {

            }
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection props)
        {
            // Create new collection of values
            var values = new SettingsPropertyValueCollection();

            // Iterate through the settings to be retrieved
            foreach (SettingsProperty setting in props)
            {
                values.Add(GetValue(setting));
            }
            return values;
        }

        SettingsPropertyValue GetValue(SettingsProperty setting)
        {
            var value = new SettingsPropertyValue(setting)
            {
                IsDirty = false
            };

            if (_settingsJson.TryGetValue(setting.Name, out var token))
            {
                value.PropertyValue = token.ToObject(setting.PropertyType);
                value.Deserialized = true;
            }
            else
            {
                value.Deserialized = false;
                value.SerializedValue = setting.DefaultValue;
            }

            return value;
        }

        void SetValue(SettingsPropertyValue setting)
        {
            if (setting.PropertyValue != null)
                _settingsJson[setting.Name] = JToken.FromObject(setting.PropertyValue);
        }

        static JObject LoadOrCreateSettings(string FilePath)
        {
            try
            {
                return JObject.Parse(File.ReadAllText(FilePath));
            }
            catch
            {
                return new JObject();
            }
        }
        
        public override void Reset(SettingsContext context)
        {
            _settingsJson.RemoveAll();
        }
    }
}