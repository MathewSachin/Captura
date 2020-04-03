using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Captura.Loc
{
    public class LanguageManager : LanguageFields
    {
        readonly JObject _defaultLanguage;
        JObject _currentLanguage;
        readonly string _langDir;
        
        public static LanguageManager Instance { get; } = new LanguageManager();

        LanguageManager()
        {
            var appDir = ServiceProvider.AppDir;

            var cultures = new List<CultureInfo>();

            if (appDir != null)
            {
                _langDir = Path.Combine(appDir, "Languages");
                
                if (Directory.Exists(_langDir))
                {
                    foreach (var file in Directory.EnumerateFiles(_langDir, "*.json"))
                    {
                        var cultureName = Path.GetFileNameWithoutExtension(file);

                        try
                        {
                            if (cultureName == null)
                                continue;

                            var culture = CultureInfo.GetCultureInfo(cultureName);

                            cultures.Add(culture);
                        }
                        catch
                        {
                            // Ignore
                        }
                    }
                }
            }

            _defaultLanguage = LoadLang("en");

            if (_currentCulture == null)
                CurrentCulture = new CultureInfo("en");

            if (cultures.Count == 0)
                cultures.Add(CurrentCulture);

            cultures.Sort((X, Y) => string.Compare(X.DisplayName, Y.DisplayName, StringComparison.Ordinal));

            AvailableCultures = cultures;
        }

        public IReadOnlyList<CultureInfo> AvailableCultures { get; }

        CultureInfo _currentCulture;

        public CultureInfo CurrentCulture
        {
            get => _currentCulture;
            set
            {
                _currentCulture = value;

                CultureInfo.CurrentUICulture = value;

                _currentLanguage = LoadLang(value.Name);

                LanguageChanged?.Invoke(value);

                RaiseAllChanged();
            }
        }

        public override event Action<CultureInfo> LanguageChanged;

        JObject LoadLang(string LanguageId)
        {
            try
            {
                var filePath = Path.Combine(_langDir, $"{LanguageId}.json");

                return JObject.Parse(File.ReadAllText(filePath));
            }
            catch
            {
                return new JObject();
            }
        }

        public string this[string Key]
        {
            get
            {
                if (Key == null)
                    return "";

                if (_currentLanguage != null
                    && _currentLanguage.TryGetValue(Key, out var value)
                    && value.ToString() is string s
                    && !string.IsNullOrWhiteSpace(s))
                    return s;

                if (_defaultLanguage != null
                    && _defaultLanguage.TryGetValue(Key, out value)
                    && value.ToString() is string t
                    && !string.IsNullOrWhiteSpace(t))
                    return t;

                return Key;
            }
        }

        protected override string GetValue(string PropertyName) => this[PropertyName];
    }
}
