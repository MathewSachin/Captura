using Captura.FFmpeg;
using Moq;
using Xunit;

namespace Captura.Tests
{
    [Collection(nameof(Tests))]
    public class FFmpegExceptionTests
    {
        const string OldMessage = "An Error Occurred with FFmpeg, Exit Code: -1.\nSee FFmpeg Log for more info.";

        static FFmpegLogItem CreateLog() => new FFmpegLogItem("out.mp4", "-i input.raw out.mp4");

        [Fact]
        public void MessageShowsFirstErrorLinesOfCascade()
        {
            // From "Cannot load" on, the cascade FFmpeg 8.1.1 prints for NVENC without an NVIDIA GPU.
            var log = CreateLog();
            log.Write("ffmpeg version 8.1.1");
            log.Write("frame=    1 fps=0.0 q=0.0 size=       0KiB time=N/A bitrate=N/A speed=N/A");
            log.Write("[h264_nvenc @ 0000029eb9f02380] Cannot load nvcuda.dll");
            log.Write("[vost#0:0/h264_nvenc @ 0000029eb9f01100] [enc:h264_nvenc @ 0000029eb7e89b40] Error while opening encoder - maybe incorrect parameters such as bit_rate, rate, width or height.");
            log.Write("[vf#0:0 @ 0000029eb9f02740] Error sending frames to consumers: Operation not permitted");
            log.Write("[vf#0:0 @ 0000029eb9f02740] Task finished with error code: -1 (Operation not permitted)");
            log.Write("[vf#0:0 @ 0000029eb9f02740] Terminating thread with return code -1 (Operation not permitted)");
            log.Write("[vost#0:0/h264_nvenc @ 0000029eb9f01100] [enc:h264_nvenc @ 0000029eb7e89b40] Could not open encoder before EOF");
            log.Write("[vost#0:0/h264_nvenc @ 0000029eb9f01100] Task finished with error code: -22 (Invalid argument)");
            log.Write("[vost#0:0/h264_nvenc @ 0000029eb9f01100] Terminating thread with return code -22 (Invalid argument)");
            log.Write("[out#0/mp4 @ 0000029eb7e53900] Nothing was written into output file, because at least one of its streams received no packets.");
            log.Write("frame=    0 fps=0.0 q=0.0 Lsize=       0KiB time=N/A bitrate=N/A speed=N/A elapsed=0:00:00.03");
            log.Write("Conversion failed!");

            var message = new FFmpegException(-1, log).Message;

            Assert.Contains("[h264_nvenc] Cannot load nvcuda.dll", message);
            Assert.Contains("Error while opening encoder", message);
            Assert.DoesNotContain(" @ 0000", message);
            Assert.DoesNotContain("Task finished", message);
            Assert.DoesNotContain("Conversion failed!", message);
            Assert.DoesNotContain("frame=", message);
        }

        [Fact]
        public void MessageShowsDriverVersionError()
        {
            // Outdated NVIDIA driver. Not a captured log: the first line is FFmpeg 8.1.1's format string
            // with illustrative versions, the cascade after it is the one from the test above.
            var log = CreateLog();
            log.Write("ffmpeg version 8.1.1");
            log.Write("[h264_nvenc @ 0000029eb9f02380] Driver does not support the required nvenc API version. Required: 13.0 Found: 12.1");
            log.Write("[vost#0:0/h264_nvenc @ 0000029eb9f01100] [enc:h264_nvenc @ 0000029eb7e89b40] Error while opening encoder - maybe incorrect parameters such as bit_rate, rate, width or height.");
            log.Write("[vf#0:0 @ 0000029eb9f02740] Error sending frames to consumers: Operation not permitted");
            log.Write("[vf#0:0 @ 0000029eb9f02740] Task finished with error code: -1 (Operation not permitted)");
            log.Write("[vf#0:0 @ 0000029eb9f02740] Terminating thread with return code -1 (Operation not permitted)");
            log.Write("[vost#0:0/h264_nvenc @ 0000029eb9f01100] [enc:h264_nvenc @ 0000029eb7e89b40] Could not open encoder before EOF");
            log.Write("[vost#0:0/h264_nvenc @ 0000029eb9f01100] Task finished with error code: -22 (Invalid argument)");
            log.Write("[vost#0:0/h264_nvenc @ 0000029eb9f01100] Terminating thread with return code -22 (Invalid argument)");
            log.Write("[out#0/mp4 @ 0000029eb7e53900] Nothing was written into output file, because at least one of its streams received no packets.");
            log.Write("Conversion failed!");

            var message = new FFmpegException(-1, log).Message;

            Assert.Contains("does not support the required nvenc API version", message);
            Assert.DoesNotContain("Task finished", message);
        }

        [Fact]
        public void MessageShowsQsvUnsupportedError()
        {
            // From "Low power mode" on, the cascade FFmpeg 8.1.1 prints for Captura's
            // hevc_qsv arguments on an Intel UHD Graphics laptop.
            var log = CreateLog();
            log.Write("ffmpeg version 8.1.1");
            log.Write("[hevc_qsv @ 0000020bbbd9b600] Low power mode is unsupported");
            log.Write("[hevc_qsv @ 0000020bbbd9b600] some encoding parameters are not supported by the QSV runtime. Please double check the input parameters.");
            log.Write("[vost#0:0/hevc_qsv @ 0000020bbbd9b3c0] [enc:hevc_qsv @ 0000020bbbd48fc0] Error while opening encoder - maybe incorrect parameters such as bit_rate, rate, width or height.");
            log.Write("[vf#0:0 @ 0000020bbbd9c240] Error sending frames to consumers: Function not implemented");
            log.Write("[vf#0:0 @ 0000020bbbd9c240] Task finished with error code: -40 (Function not implemented)");
            log.Write("[vf#0:0 @ 0000020bbbd9c240] Terminating thread with return code -40 (Function not implemented)");
            log.Write("[vost#0:0/hevc_qsv @ 0000020bbbd9b3c0] [enc:hevc_qsv @ 0000020bbbd48fc0] Could not open encoder before EOF");
            log.Write("[vost#0:0/hevc_qsv @ 0000020bbbd9b3c0] Task finished with error code: -22 (Invalid argument)");
            log.Write("[vost#0:0/hevc_qsv @ 0000020bbbd9b3c0] Terminating thread with return code -22 (Invalid argument)");
            log.Write("[out#0/mp4 @ 0000020bbbd10800] Nothing was written into output file, because at least one of its streams received no packets.");
            log.Write("frame=    0 fps=0.0 q=0.0 Lsize=       0KiB time=N/A bitrate=N/A speed=N/A elapsed=0:00:00.28");
            log.Write("Conversion failed!");

            var message = new FFmpegException(-1, log).Message;

            Assert.Contains("Low power mode is unsupported", message);
        }

        [Fact]
        public void MessageFallsBackToLastLines()
        {
            var log = CreateLog();
            log.Write("ffmpeg version 8.1.1");
            log.Write("Stream mapping:");
            log.Write("  Stream #0:0 -> #0:0 (rawvideo (native) -> h264 (libx264))");
            log.Write("frame=   10 fps=0.0");
            log.Write("size=       0KiB time=00:00:00.00 bitrate=N/A speed=N/A");
            log.Write("Conversion failed!");

            var message = new FFmpegException(-1, log).Message;

            Assert.Contains("Stream mapping:", message);
            Assert.Contains("Conversion failed!", message);
            Assert.DoesNotContain("ffmpeg version", message);
            Assert.DoesNotContain("frame=", message);
            Assert.DoesNotContain("size=", message);
        }

        [Fact]
        public void LogWithoutOutputKeepsOldMessage()
        {
            Assert.Equal(OldMessage, new FFmpegException(-1, CreateLog()).Message);
        }

        [Fact]
        public void NoLogKeepsOldMessage()
        {
            Assert.Equal(OldMessage, new FFmpegException(-1, Log: null).Message);
        }

        [Fact]
        public void LogWithoutOutputHeaderKeepsOldMessage()
        {
            var log = Mock.Of<IFFmpegLogEntry>(L => L.GetCompleteLog() == "Cannot load nvcuda.dll");

            Assert.Equal(OldMessage, new FFmpegException(-1, log).Message);
        }
    }
}
