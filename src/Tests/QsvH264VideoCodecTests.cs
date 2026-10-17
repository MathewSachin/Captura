using System.Linq;
using Captura.FFmpeg;
using Captura.Video;
using Xunit;

namespace Captura.Tests
{
    [Collection(nameof(Tests))]
    public class QsvH264VideoCodecTests
    {
        [Theory]
        [InlineData(100, 1)]
        [InlineData(70, 15)]
        [InlineData(1, 51)]
        public void ArgsForQuality(int Quality, int ExpectedGlobalQuality)
        {
            var output = new FFmpegOutputArgs("out.mp4");

            new QsvH264VideoCodec().Apply(new FFmpegSettings(), new VideoWriterArgs { VideoQuality = Quality }, output);

            var args = output.GetArgs();

            Assert.Contains("-vcodec h264_qsv", args);
            Assert.Contains($"-global_quality {ExpectedGlobalQuality} ", args);
        }

        [Fact]
        public void ExistingCliIndicesAreKept()
        {
            // captura-cli prints each encoder's ToString() next to the index used by --encoder ffmpeg:N
            var names = new FFmpegWriterProvider(new FFmpegSettings())
                .Take(6)
                .Select(M => M.ToString());

            Assert.Equal(new[]
            {
                "Mp4 (H.264, AAC)",
                "Avi (Xvid, Mp3)",
                "Intel QuickSync: Mp4 (HEVC, AAC)",
                "NVenc: Mp4 (H.264, AAC)",
                "NVenc: Mp4 (HEVC, AAC)",
                "Intel QuickSync: Mp4 (H.264, AAC)"
            }, names);
        }
    }
}