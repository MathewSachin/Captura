using System.Drawing;
using Captura;
using Captura.Models;
using SharpDX.MediaFoundation;

namespace DesktopDuplication
{
    public class MfCapturePreview : IWebcamCapture
    {
        readonly CaptureEngine _captureEngine;
        readonly MediaSource _mediaSource;
        readonly SourceReader _sourceReader;
        readonly Size _size;
        readonly CapturePreviewSink _previewSink;

        const long MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM_FOR_VIDEO_PREVIEW = 0xFFFFFFFA;

        public int Width => _size.Width;

        public int Height => _size.Height;

        public MfCapturePreview(MfCaptureDevice Device)
        {
            using (var captureEngineClassFactory = new CaptureEngineClassFactory())
            {
                _captureEngine = new CaptureEngine(captureEngineClassFactory);
            }

            var attribs = new MediaAttributes(1);
            attribs.Set(CaptureEngineAttributeKeys.UseVideoDeviceOnly, true);

            _mediaSource = Device.GetSource();

            _captureEngine.Initialize(attribs, null, _mediaSource);

            _captureEngine.GetSink(CaptureEngineSinkType.Preview, out var captureSink);

            using (captureSink)
            {
                _previewSink = captureSink.QueryInterface<CapturePreviewSink>();
            }

            using (var source = _captureEngine.Source)
            {
                const int mfVideoStream = unchecked((int) MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM_FOR_VIDEO_PREVIEW);

                source.GetCurrentDeviceMediaType(mfVideoStream, out var mediaType);

                mediaType.Set(MediaTypeAttributeKeys.AllSamplesIndependent, 1);

                _previewSink.AddStream(mfVideoStream, mediaType, null, out _);
            }

            //using (var captureSource = _captureEngine.Source)
            //{
            //    var guid = typeof(SourceReader).GUID;

            //    captureSource.GetService(guid, guid, out var srcReader);
            //    srcReader.QueryInterface(ref guid, out var ptr);
            //    _sourceReader = new SourceReader(ptr);
            //}

            //var mediaType = new MediaType();
            //mediaType.Set(MediaTypeAttributeKeys.MajorType, MediaTypeGuids.Video);
            //mediaType.Set(MediaTypeAttributeKeys.Subtype, VideoFormatGuids.Argb32);

            //_sourceReader.SetCurrentMediaType(SourceReaderIndex.FirstVideoStream, mediaType);

            //var currentMediaType = _sourceReader.GetCurrentMediaType(SourceReaderIndex.FirstVideoStream);

            //var frameSize = new PackedLong
            //{
            //    Long = currentMediaType.Get(MediaTypeAttributeKeys.FrameSize)
            //};

            //_size = new Size(frameSize.High, frameSize.Low);
        }

        public void Dispose()
        {
            _captureEngine.StopPreview();
            _previewSink.Dispose();
            _captureEngine.Dispose();

            using (_mediaSource)
                _mediaSource.Shutdown();
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
            _captureEngine.StopPreview();

            if (Window != null)
            {
                _previewSink.RenderHandle = Window.Handle;
            }

            _captureEngine.StartPreview();

            var rawRect = new SharpDX.Mathematics.Interop.RawRectangle(Location.Left,
                Location.Top,
                Location.Right,
                Location.Bottom);

            _previewSink.UpdateVideo(null, rawRect, null);
        }
    }
}