// ReSharper disable InconsistentNaming

namespace Captura.Native
{
    enum DrawIconExFlags
    {
        Compat = 0x04,
        DefaultSize = 0x08,
        Image = 0x02,
        Mask = 0x01,
        NoMirror = 0x10,
        Normal = Image | Mask
    }
}