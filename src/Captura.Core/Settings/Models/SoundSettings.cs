using System.Collections.Generic;

namespace Captura.Audio
{
    public class SoundSettings : PropertyStore
    {
        public Dictionary<SoundKind, string> Items { get; } = new Dictionary<SoundKind, string>();
    }
}