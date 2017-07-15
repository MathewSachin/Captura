using Captura;
using Captura.Properties;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;

// Derived from http://www.codeproject.com/Articles/20917/Creating-a-Custom-Settings-Provider?msg=2934144#xx2934144xx
public class CustomSettingsProvider : SettingsProvider, IApplicationSettingsProvider
{
    const string SettingsRootName = "Settings";
    
    readonly XDocument SettingsXml;
    
    public static readonly string FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Resources.AppName, "Settings.xml");
    
    public CustomSettingsProvider()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FileName));
        
        SettingsXml = LoadOrCreateSettings(FileName);
    }

    public override void Initialize(string name, NameValueCollection collection)
    {
        base.Initialize(ApplicationName, collection);
    }

    public override string ApplicationName
    {
        get => Resources.AppName;
        set { }
    }

    public override string Name => nameof(CustomSettingsProvider);
    
    public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection properties)
    {
        // Only dirty settings are included in properties, and only ones relevant to this provider
        foreach (SettingsPropertyValue propertyValue in properties)
        {
            SetValue(propertyValue);
        }

        try
        {
            SettingsXml.Sort().Save(FileName);
        }
        catch
        {
            //Log.WriteError(string.Concat(Errors.Save_Settings_Error, ":", Environment.NewLine), ex);
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
        try
        {
            var node = SettingsXml.Root.XPathSelectElement($"./{setting.Name}");

            return node?.Value ?? setting.DefaultValue?.ToString() ?? "";
        }
        catch { return ""; }
    }

    void SetValue(SettingsPropertyValue setting)
    {
        try
        {
            var node = SettingsXml.Root.GetOrAddElement($"{setting.Name}");

            node.Value = setting.SerializedValue?.ToString() ?? "";
        }
        catch { }
    }

    static XDocument LoadOrCreateSettings(string filePath)
    {
        XDocument settingsXml = null;
        
        try
        {
            settingsXml = XDocument.Load(filePath);

            if (settingsXml.Root.Name.LocalName != SettingsRootName)
                throw new FormatException();
        }
        catch
        {
            //Create new document
            settingsXml = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(SettingsRootName, string.Empty)
            );
        }

        return settingsXml;
    }

    public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
    {
        return new SettingsPropertyValue(property) { PropertyValue = property.DefaultValue };
    }

    public void Reset(SettingsContext context)
    {
        SettingsXml.Root.RemoveNodes();
    }

    public void Upgrade(SettingsContext context, SettingsPropertyCollection properties) { }
}