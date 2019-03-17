using System;
using System.Text;

namespace Captura.Models
{
    public class FFmpegLogItem : NotifyPropertyChanged
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

        public void Write(string Text)
        {
            if (Text == null)
                return;

            _complete.AppendLine(Text);

            if (Text.StartsWith("frame=") || Text.StartsWith("size="))
            {
                Frame = Text;
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