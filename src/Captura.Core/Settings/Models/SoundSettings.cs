using System.Collections.Generic;

namespace Captura.Models
{
    public class SoundSettings : PropertyStore
    {
        public Dictionary<SoundKind, string> Items { get; } = new Dictionary<SoundKind, string>();
    }
}