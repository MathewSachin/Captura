using System;

namespace Captura
{
    public interface IRemoveRequester
    {
        event Action RemoveRequested;
    }
}