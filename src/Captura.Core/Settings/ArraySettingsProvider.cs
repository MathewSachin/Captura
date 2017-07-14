using Captura.Properties;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;

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
            var value = "";

            try
            {
                value = File.ReadAllText(Path.Combine(Dir, $"{setting.Name}.xml"));
            }
            catch { }

            values.Add(new SettingsPropertyValue(setting)
            {
                IsDirty = false,
                SerializedValue = value,
            });
        }
        return values;
    }

    public void Reset(SettingsContext context)
    {
    }

    public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
    {
        foreach (SettingsPropertyValue propertyValue in collection)
        {
            try
            {
                File.WriteAllText(Path.Combine(Dir, $"{propertyValue.Name}.xml"), propertyValue.SerializedValue?.ToString() ?? "");
            }
            catch { }
        }
    }

    public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
    {
    }
}