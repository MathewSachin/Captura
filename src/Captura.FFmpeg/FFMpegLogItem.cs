using System;
using System.Text;
using System.Windows.Input;

namespace Captura.Models
{
    public class FFmpegLogItem : NotifyPropertyChanged
    {
        public string Name { get; }

        public string Args { get; }

        public FFmpegLogItem(string Name,
            string Args,
            IClipboardService ClipboardService)
        {
            this.Name = Name;
            this.Args = Args;

            _complete.AppendLine("ARGS:");
            _complete.AppendLine("-------------");
            _complete.AppendLine(Args);
            _complete.AppendLine();
            _complete.AppendLine("OUTPUT:");
            _complete.AppendLine("-------------");

            CopyToClipboardCommand = new DelegateCommand(() =>
            {
                ClipboardService.SetText(_complete.ToString());
            });

            RemoveCommand = new DelegateCommand(() => RemoveRequested?.Invoke());
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
            private set
            {
                _frame = value;

                OnPropertyChanged();
            }
        }

        public string Content
        {
            get => _content;
            private set
            {
                _content = value;

                OnPropertyChanged();
            }
        }

        public ICommand CopyToClipboardCommand { get; }

        public ICommand RemoveCommand { get; }

        public event Action RemoveRequested;
    }
}