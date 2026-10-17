using Captura.FFmpeg;
using Captura.Video;
using Xunit;

namespace Captura.Tests
{
    [Collection(nameof(Tests))]
    public class X264VideoCodecTests
    {
        static string GetArgs(int Quality)
        {
            var output = new FFmpegOutputArgs("out.mp4");

            new X264VideoCodec().Apply(new FFmpegSettings(), new VideoWriterArgs { VideoQuality = Quality }, output);

            return output.GetArgs();
        }

        [Theory]
        [InlineData(100, 1)]
        [InlineData(99, 1)]
        [InlineData(98, 1)]
        [InlineData(70, 15)]
        [InlineData(1, 51)]
        public void CrfForQuality(int Quality, int ExpectedCrf)
        {
            Assert.Contains($"-crf {ExpectedCrf} ", GetArgs(Quality));
        }

        [Fact]
        public void NeverLossless()
        {
            for (var quality = 1; quality <= 100; ++quality)
            {
                Assert.DoesNotContain("-crf 0 ", GetArgs(quality));
            }
        }
    }
}