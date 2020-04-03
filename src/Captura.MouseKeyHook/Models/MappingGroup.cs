using System.Collections.Generic;
using System.Windows.Forms;

namespace Captura.MouseKeyHook
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MappingGroup
    {
        public List<ModifierStates> On { get; set; }

        public Dictionary<Keys, string> Keys { get; } = new Dictionary<Keys, string>();
    }
}