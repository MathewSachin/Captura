using System;

namespace Captura.Models
{
    [Flags]
    public enum Modifiers
    {
        None,
        Alt = 1,
        Ctrl = 2,
        Shift = 4
    }
}