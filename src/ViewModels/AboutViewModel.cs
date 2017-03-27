using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace Captura
{
    public class AboutViewModel : ViewModelBase
    {
        public static CultureInfo DefaultLanguage { get; } = new CultureInfo("en-US");

        public static CultureInfo[] Languages { get; } =
        {
            DefaultLanguage,
            new CultureInfo("hi-IN"),
            new CultureInfo("ml-IN")
        };

        public AboutViewModel()
        {
            // Initial Language Setup
            Language = Language;
        }
        
        public CultureInfo Language
        {
            get { return new CultureInfo(Settings.Instance.Language); }
            set
            {
                value = value ?? DefaultLanguage;
                                
                Thread.CurrentThread.CurrentUICulture = value;

                Settings.Instance.Language = value.Name;

                var dict = new ResourceDictionary()
                {
                    Source = new Uri($"Languages/lang.{value.Name}.xaml", UriKind.Relative)
                };

                var mergedDicts = Application.Current.Resources.MergedDictionaries;

                var oldDict = mergedDicts.Where(d => d.Source != null && d.Source.OriginalString.StartsWith("Languages/lang."))
                                         .First();

                if (oldDict?.Source == dict.Source)
                    return;

                mergedDicts.Add(dict);

                if (oldDict != null)
                    mergedDicts.Remove(oldDict);
            }
        }
    }
}
