using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SharpDX.DXGI;

namespace Captura.Models
{
    public class DeskDuplSourceProvider : IVideoSourceProvider
    {
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

        public string Name => "Desktop Duplication";

        public override string ToString() => Name;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}