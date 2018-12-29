using System.Drawing;
using FFmpeg.AutoGen;

namespace Captura.FFmpeg.Interop
{
    public unsafe class FFmux : IVideoFileWriter
    {
        readonly FFmpegFormatContext _formatContext;

        readonly FFmpegVideoStream _videoStream;

        public FFmux(string FileName, Size FrameSize, int Fps)
        {
            _formatContext = new FFmpegFormatContext(FileName, null);

            var fmt = _formatContext.FormatContext->oformat;

            if (fmt->video_codec != AVCodecID.AV_CODEC_ID_NONE)
            {
                var codecInfo = new FFmpegVideoCodecInfo(fmt->video_codec, AVPixelFormat.AV_PIX_FMT_YUV420P);

                _videoStream = new FFmpegVideoStream(_formatContext.FormatContext, codecInfo, Fps, FrameSize);

                SetVideoCodecOptions(_videoStream.CodecContext, codecInfo.Id);

                _videoStream.OpenCodec();
            }

            if (fmt->audio_codec != AVCodecID.AV_CODEC_ID_NONE)
            {
                //audio_st = AddStream(oc, out var audio_codec, fmt->audio_codec);
            }

            _formatContext.BeginWriting();
        }

        void SetVideoCodecOptions(AVCodecContext* CodecContext, AVCodecID CodecId)
        {
            CodecContext->bit_rate = 4_000_000;
            CodecContext->gop_size = 12;
            CodecContext->max_b_frames = 1;

            if (CodecId == AVCodecID.AV_CODEC_ID_H264)
            {
                ffmpeg.av_opt_set(CodecContext->priv_data, "preset", "ultrafast", 0);
            }
        }

        void SetAudioCodecOptions(AVCodecContext* CodecContext, AVCodecID CodecId)
        {
            CodecContext->sample_fmt = AVSampleFormat.AV_SAMPLE_FMT_FLTP;
            CodecContext->bit_rate = 64_000;
            CodecContext->sample_rate = 44_100;
            CodecContext->channels = 2;
        }

        public void Dispose()
        {
            _formatContext.WriteTrailer();

            _videoStream.Dispose();

            _formatContext.Dispose();
        }

        public void WriteFrame(IBitmapFrame Image)
        {
            if (Image is RepeatFrame)
            {
                _videoStream.IncrementPts();
                return;
            }

            using (Image)
                Image.CopyTo(_videoStream.Buffer, _videoStream.Buffer.Length);

            _videoStream.WriteFrame();
        }

        public bool SupportsAudio => false;

        public void WriteAudio(byte[] Buffer, int Length) { }
    }
}