using Screna;

namespace Captura
{
    public interface IVSLI
    {
        IImageProvider GetImageProvider(params IOverlay[] Overlays);
    }
}
