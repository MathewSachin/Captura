using System;

namespace Captura.Models
{
    interface IKeyRecord
    {
        DateTime TimeStamp { get; }

        string Display { get; }
    }
}