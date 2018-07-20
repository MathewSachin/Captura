// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

using System.Collections.Generic;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Keymap
    {
        public string Name { get; set; }

        public List<MappingGroup> Mappings { get; } = new List<MappingGroup>();
    }
}