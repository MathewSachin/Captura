using System.Collections.Generic;

namespace Captura.Models
{
    public interface IVideoWriterProvider : IEnumerable<IVideoWriterItem>
    {
        string Name { get; }

        string Description { get; }
    }
}