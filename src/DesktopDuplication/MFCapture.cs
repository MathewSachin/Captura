using System;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.MediaFoundation;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;

namespace DesktopDuplication
{
    public class MfCapture : IDisposable
    {
        readonly MediaSource _mediaSource;
        readonly SourceReader _sourceReader;
        readonly Size2 _size;

        public MfCapture(MfCaptureDevice Device)
        {
            var attribs = new MediaAttributes(1);
            //attribs.Set(SourceReaderAttributeKeys.EnableVideoProcessing, 1);
            attribs.Set(SourceReaderAttributeKeys.EnableAdvancedVideoProcessing, true);

            _mediaSource = Device.GetSource();
            _sourceReader = new SourceReader(_mediaSource, attribs);

            var mediaType = new MediaType();
            mediaType.Set(MediaTypeAttributeKeys.MajorType, MediaTypeGuids.Video);
            mediaType.Set(MediaTypeAttributeKeys.Subtype, VideoFormatGuids.Argb32);

            _sourceReader.SetCurrentMediaType(SourceReaderIndex.FirstVideoStream, mediaType);

            var currentMediaType = _sourceReader.GetCurrentMediaType(SourceReaderIndex.FirstVideoStream);
            var frameSize = new PackedLong
            {
                Value = currentMediaType.Get(MediaTypeAttributeKeys.FrameSize)
            };

            _size = new Size2(frameSize.High, frameSize.Low);
        }

        public Bitmap Read(Direct2DEditorSession EditorSession)
        {
            var sample = _sourceReader.ReadSample(
                SourceReaderIndex.FirstVideoStream,
                0,
                out _,
                out _,
                out _);

            if (sample == null)
                return null;

            using (sample)
            using (var buffer = sample.GetBufferByIndex(0))
            {
                try
                {
                    var ptr = buffer.Lock(out _, out var currentLength);

                    return new Bitmap(EditorSession.RenderTarget,
                        _size,
                        new DataPointer(ptr, currentLength),
                        _size.Width * 4,
                        new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Ignore)));
                }
                finally
                {
                    buffer.Unlock();
                }
            }
        }

        public void Dispose()
        {
            _mediaSource.Shutdown();
            _mediaSource.Dispose();
        }
    }
}