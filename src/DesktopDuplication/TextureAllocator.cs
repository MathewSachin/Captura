using System;
using System.Collections.Concurrent;
using SharpDX.Direct3D11;

namespace DesktopDuplication
{
    public class TextureAllocator : IDisposable
    {
        readonly Texture2DDescription _textureDescription;
        readonly Device _device;

        const int TextureCount = 15;
        readonly AllocatedTexture[] _textures = new AllocatedTexture[TextureCount];
        int _currentTexture = -1;

        // If all textures are the exact same size and color format,
        // consider making those parameters private class members and
        // requiring they be specified as arguments to the constructor.
        public TextureAllocator(Texture2DDescription TextureDescription, Device Device)
        {
            _textureDescription = TextureDescription;
            _device = Device;
        }

        bool _disposedValue;

        public void Dispose()
        {
            if (!_disposedValue)
            {
                foreach (var texture in _textures)
                {
                    texture?.Dispose();
                }

                _disposedValue = true;
            }
        }
        
        AllocatedTexture InternalAllocateNewTexture()
        {
            return new AllocatedTexture(new Texture2D(_device, _textureDescription));
        }

        public AllocatedTexture AllocateTexture()
        {
            _currentTexture = ++_currentTexture % TextureCount;

            return _textures[_currentTexture] ?? (_textures[_currentTexture] = InternalAllocateNewTexture());
        }
    }
}