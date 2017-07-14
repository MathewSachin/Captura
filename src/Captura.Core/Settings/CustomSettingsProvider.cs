using Captura.Properties;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Xml;

// Derived from http://www.codeproject.com/Articles/20917/Creating-a-Custom-Settings-Provider?msg=2934144#xx2934144xx
public class CustomSettingsProvider : SettingsProvider, IApplicationSettingsProvider
{
    const string SettingsRootName = "Settings";
    
    readonly XmlDocument SettingsXml;
    
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
            SettingsXml.Save(FileName);
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
        string ret = "";

        try
        {
            var node = SettingsXml.SelectSingleNode($"{SettingsRootName}/{setting.Name}");

            ret = node?.InnerText ?? setting.DefaultValue?.ToString() ?? "";
        }
        catch
        {
        }

        return ret;
    }

    void SetValue(SettingsPropertyValue setting)
    {
        XmlElement node = null;

        try
        {
            node = SettingsXml.SelectSingleNode($"{SettingsRootName}/{setting.Name}") as XmlElement;
        }
        catch { }

        if (node == null)
            node = SettingsXml.CreateElement(setting.Name);

        var value = setting.SerializedValue?.ToString() ?? "";
        
        node.InnerText = value;

        SettingsXml.SelectSingleNode(SettingsRootName).AppendChild(node);
    }

    static XmlDocument LoadOrCreateSettings(string filePath)
    {
        var settingsXml = new XmlDocument();
        
        try
        {
            settingsXml.Load(filePath);
        }
        catch
        {
            //Create new document
            settingsXml = new XmlDocument();
                        
            var dec = settingsXml.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
            settingsXml.AppendChild(dec);
            
            var nodeRoot = settingsXml.CreateNode(XmlNodeType.Element, SettingsRootName, "");
            settingsXml.AppendChild(nodeRoot);
        }

        return settingsXml;
    }

    public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
    {
        return new SettingsPropertyValue(property) { PropertyValue = property.DefaultValue };
    }

    public void Reset(SettingsContext context)
    {
        SettingsXml.RemoveAll();
    }

    public void Upgrade(SettingsContext context, SettingsPropertyCollection properties) { }
}