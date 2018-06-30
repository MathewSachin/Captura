using System;

namespace Captura.Models {
    public interface IKeyRecord
    {
        DateTime TimeStamp { get; }

        string Display { get; }
    }
}