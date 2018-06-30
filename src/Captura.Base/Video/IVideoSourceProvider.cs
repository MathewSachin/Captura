using System.Collections.Generic;

namespace Captura.Models
{
    public interface IVideoSourceProvider : IEnumerable<IVideoItem>
    {
        string Name { get; }
    }
}