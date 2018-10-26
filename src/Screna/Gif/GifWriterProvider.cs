using System.Collections;
using System.Collections.Generic;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GifWriterProvider : NotifyPropertyChanged, IVideoWriterProvider
    {
        readonly LanguageManager _loc;
        readonly GifItem _gifItem;

        public GifWriterProvider(LanguageManager Loc, GifItem GifItem)
        {
            _loc = Loc;
            _gifItem = GifItem;

            _loc.LanguageChanged += L => RaisePropertyChanged(nameof(Name));
        }
        
        public string Name => "Gif";

        public IEnumerator<IVideoWriterItem> GetEnumerator()
        {
            yield return _gifItem;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => Name;

        public string Description => @"Use internal Gif encoder.
Variable Frame Rate mode is recommended.";
    }
}
