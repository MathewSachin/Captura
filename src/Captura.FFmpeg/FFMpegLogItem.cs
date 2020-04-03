using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Captura.FFmpeg
{
    public class FFmpegLogItem : NotifyPropertyChanged, IFFmpegLogEntry
    {
        public string Name { get; }

        public string Args { get; }

        public FFmpegLogItem(string Name, string Args)
        {
            this.Name = Name;
            this.Args = Args;

            _complete.AppendLine("ARGS:");
            _complete.AppendLine("-------------");
            _complete.AppendLine(Args);
            _complete.AppendLine();
            _complete.AppendLine("OUTPUT:");
            _complete.AppendLine("-------------");
        }

        string _content = "", _frame = "";

        readonly StringBuilder _complete = new StringBuilder();

        public event Action<int> ProgressChanged;

        TimeSpan? _duration;
        int _lastReportedProgress;

        readonly Regex _durationRegex = new Regex(@"Duration: (\d{2}:\d{2}:\d{2}.\d{2})");
        readonly Regex _timeRegex = new Regex(@"time=(\d{2}:\d{2}:\d{2}.\d{2})");

        public void Write(string Text)
        {
            if (Text == null)
                return;

            _complete.AppendLine(Text);

            if (_duration == null)
            {
                var match = _durationRegex.Match(Text);

                if (match.Success)
                {
                    _duration = TimeSpan.Parse(match.Groups[1].Value);
                }
            }

            if (Text.StartsWith("frame=") || Text.StartsWith("size="))
            {
                Frame = Text;

                if (_duration != null)
                {
                    var match = _timeRegex.Match(Text);

                    if (match.Success)
                    {
                        var time = TimeSpan.Parse(match.Groups[1].Value);

                        var progress = (int)(time.TotalMilliseconds * 100 / _duration.Value.TotalMilliseconds);

                        if (progress > _lastReportedProgress)
                        {
                            _lastReportedProgress = progress;
                            ProgressChanged?.Invoke(progress);
                        }
                    }
                }
            }
            else Content += Text + Environment.NewLine;
        }

        public string Frame
        {
            get => _frame;
            private set => Set(ref _frame, value);
        }

        public string Content
        {
            get => _content;
            private set => Set(ref _content, value);
        }

        public string GetCompleteLog() => _complete.ToString();
    }
}