using Captura.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Captura
{
    public class JsonSettingsProvider : SettingsProviderBase
    {
        readonly JObject SettingsJson;
        
        public static readonly string FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Resources.AppName, "Settings.json");

        public JsonSettingsProvider()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FileName));

            SettingsJson = LoadOrCreateSettings(FileName);
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
                var jobj = new JObject(SettingsJson.Properties().OrderBy(j => j.Name).ToArray());
                File.WriteAllText(FileName, jobj.ToString());
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
                values.Add(new SettingsPropertyValue(setting)
                {
                    IsDirty = false,
                    SerializedValue = GetValue(setting),
                });
            }
            return values;
        }

        object GetValue(SettingsProperty setting)
        {
            var value = (string)SettingsJson[setting.Name];

            if (string.IsNullOrEmpty(value))
                return setting.DefaultValue?.ToString() ?? "";

            return value;
        }

        void SetValue(SettingsPropertyValue setting)
        {
            SettingsJson[setting.Name] = setting.SerializedValue?.ToString() ?? "";
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
            SettingsJson.RemoveAll();
        }
    }
}