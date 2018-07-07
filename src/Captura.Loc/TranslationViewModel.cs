using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Newtonsoft.Json.Linq;

namespace Captura
{
    public class TranslationViewModel : NotifyPropertyChanged
    {
        JObject _currentLanguage;
        readonly string _langDir;
        
        public TranslationViewModel()
        {
            _langDir = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Languages");

            if (Directory.Exists(_langDir))
            {
                var files = Directory.EnumerateFiles(_langDir, "*.json");

                foreach (var file in files)
                {
                    var cultureName = Path.GetFileNameWithoutExtension(file);

                    try
                    {
                        if (cultureName == null)
                            continue;

                        var culture = CultureInfo.GetCultureInfo(cultureName);

                        _cultures.Add(culture);
                    }
                    catch
                    {
                        // Ignore
                    }
                }
            }

            if (_selectedCulture == null && _cultures.Count > 0)
                SelectedCulture = _cultures[0];

            AvailableCultures = new ReadOnlyObservableCollection<CultureInfo>(_cultures);
            
            Fields = new LanguageFields()
                .GetType()
                .GetProperties()
                .Select(M => new LanguageField(M.Name, this))
                .ToArray();

            SaveCommand = new DelegateCommand(Save);

            DiscardChangesCommand = new DelegateCommand(() => SelectedCulture = SelectedCulture);

            CleanAllCommand = new DelegateCommand(CleanAll);
        }

        readonly ObservableCollection<CultureInfo> _cultures = new ObservableCollection<CultureInfo>();

        public ReadOnlyObservableCollection<CultureInfo> AvailableCultures { get; }

        public LanguageField[] Fields { get; }

        CultureInfo _selectedCulture;

        public CultureInfo SelectedCulture
        {
            get => _selectedCulture;
            set
            {
                _selectedCulture = value;
                
                _currentLanguage = LoadLang(value.Name);
                
                OnPropertyChanged();
            }
        }
        
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

                if (_currentLanguage != null && _currentLanguage.TryGetValue(Key, out var value))
                    return value.ToString();
                
                return "";
            }
            set
            {
                if (_currentLanguage != null && _currentLanguage.TryGetValue(Key, out _))
                {
                    _currentLanguage[Key] = value;
                }
                else
                {
                    _currentLanguage?.Add(Key, value);
                }
            }
        }

        public ICommand SaveCommand { get; }

        void Save()
        {
            Save(_currentLanguage, _selectedCulture);
        }

        void Save(JObject Object, CultureInfo Culture)
        {
            try
            {
                var jenum = Object
                    .Properties()
                    .Where(M => Fields.Any(F => F.Key == M.Name))
                    .OrderBy(M => M.Name)
                    .Cast<object>()
                    .ToArray();

                var jobj = new JObject(jenum);

                var path = Path.Combine(_langDir, $"{Culture.Name}.json");

                File.WriteAllText(path, jobj.ToString());
            }
            catch { }
        }

        void CleanAll()
        {
            foreach (var culture in AvailableCultures)
            {
                var jobj = LoadLang(culture.Name);

                Save(jobj, culture);
            }
        }

        public ICommand DiscardChangesCommand { get; }

        public ICommand CleanAllCommand { get; }
    }
}