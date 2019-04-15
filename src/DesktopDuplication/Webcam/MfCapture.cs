using System;
using System.Drawing;
using Captura;
using Captura.Models;
using SharpDX.MediaFoundation;

namespace DesktopDuplication
{
    public class MfCapture : IWebcamCapture
    {
        readonly MediaSource _mediaSource;
        readonly SourceReader _sourceReader;
        readonly Size _size;

        public int Width => _size.Width;

        public int Height => _size.Height;

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
                Long = currentMediaType.Get(MediaTypeAttributeKeys.FrameSize)
            };

            _size = new Size(frameSize.High, frameSize.Low);
        }

        public void Dispose()
        {
            _mediaSource.Shutdown();
            _mediaSource.Dispose();
        }

        public IBitmapImage Capture(IBitmapLoader BitmapLoader)
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
                    var ptr = buffer.Lock(out _, out _);

                    return BitmapLoader.CreateBitmapBgr32(_size, ptr, _size.Width * 4);
                }
                finally
                {
                    buffer.Unlock();
                }
            }
        }

        public void UpdatePreview(IWindow Window, Rectangle Location)
        {
            // TODO: Implement this
            throw new NotImplementedException();
        }
    }
}