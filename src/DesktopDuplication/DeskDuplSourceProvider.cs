using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SharpDX.DXGI;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DeskDuplSourceProvider : NotifyPropertyChanged, IVideoSourceProvider
    {
        readonly LanguageManager _loc;

        public DeskDuplSourceProvider(LanguageManager Loc)
        {
            _loc = Loc;

            Loc.LanguageChanged += L => RaisePropertyChanged(nameof(Name));
        }

        public IEnumerator<IVideoItem> GetEnumerator()
        {
            var outputs = new Factory1()
                .Adapters1
                .SelectMany(M => M.Outputs
                    .Select(N => new
                    {
                        Adapter = M,
                        Output = N.QueryInterface<Output1>()
                    }));

            foreach (var output in outputs)
            {
                yield return new DeskDuplItem(output.Adapter, output.Output);
            }
        }

        public string Name => _loc.DesktopDuplication;

        public override string ToString() => Name;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}