using System;
using System.Collections.Concurrent;
using SharpDX.Direct3D11;
using SharpDX.MediaFoundation;

namespace DesktopDuplication
{
    public class TextureAllocator : AsyncCallbackBase, IDisposable
    {
        ConcurrentStack<Texture2D> _mFreeStack;
        static readonly Guid D3D11Texture2D = new Guid("6f15aaf2-d208-4e89-9ab4-489535d34f9c");

        readonly Texture2DDescription _textureDescription;
        readonly Device _device;

        // If all textures are the exact same size and color format,
        // consider making those parameters private class members and
        // requiring they be specified as arguments to the constructor.
        public TextureAllocator(Texture2DDescription TextureDescription, Device Device)
        {
            _textureDescription = TextureDescription;
            _device = Device;
            _mFreeStack = new ConcurrentStack<Texture2D>();
        }

        bool _disposedValue;

        protected override void Dispose(bool Disposing)
        {
            if (!_disposedValue)
            {
                if (Disposing)
                {
                    // Dispose managed resources here
                }

                if (_mFreeStack != null)
                {
                    while (_mFreeStack.TryPop(out var texture))
                    {
                        texture.Dispose();
                    }

                    _mFreeStack = null;
                }

                _disposedValue = true;
            }

            base.Dispose(Disposing);
        }

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TextureAllocator()
        {
            Dispose(false);
        }

        Texture2D InternalAllocateNewTexture()
        {
            return new Texture2D(_device, _textureDescription);
        }

        public Texture2D AllocateTexture()
        {
            if (_mFreeStack.TryPop(out var existingTexture))
            {
                return existingTexture;
            }

            return InternalAllocateNewTexture();
        }

        public Sample CreateSample(Texture2D Texture)
        {
            // Create the video sample. This function returns an IMFTrackedSample per MSDN
            var sample = MediaFactory.CreateVideoSampleFromSurface(null);
            
            // Query the IMFSample to see if it implements IMFTrackedSample
            var trackedSample = sample.QueryInterface<TrackedSample>();
            
            // Create the media buffer from the texture
            MediaFactory.CreateDXGISurfaceBuffer(D3D11Texture2D, Texture, 0, false, out var mediaBuffer);

            // Set the owning instance of this class as the allocator
            // for IMFTrackedSample to notify when the sample is released
            trackedSample.SetAllocator(this, null);

            mediaBuffer.CurrentLength = mediaBuffer.QueryInterface<Buffer2D>().ContiguousLength;

            // Attach the created buffer to the sample
            sample.AddBuffer(mediaBuffer);

            return sample;
        }

        // This is public so any textures you allocate but don't make IMFSamples 
        // out of can be returned to the allocator manually.
        void ReturnFreeTexture(Texture2D FreeTexture)
        {
            _mFreeStack.Push(FreeTexture);
        }

        public override void Invoke(AsyncResult Result)
        {
            var sample = Result.QueryInterface<Sample>();

            // Based on your implementation, there should only be one 
            // buffer attached to one sample, so we can always grab the
            // first buffer. You could add some error checking here to make
            // sure the sample has a buffer count that is 1.
            var mediaBuffer = sample.GetBufferByIndex(0);

            var dxgiBuffer = mediaBuffer.QueryInterface<DXGIBuffer>();
            
            // Got an IMFDXGIBuffer, so we can extract the internal 
            // ID3D11Texture2D and make a new SharpDX.Texture2D wrapper.
            dxgiBuffer.GetResource(D3D11Texture2D, out var texturePtr);

            ReturnFreeTexture(new Texture2D(texturePtr));
        }
    }
}