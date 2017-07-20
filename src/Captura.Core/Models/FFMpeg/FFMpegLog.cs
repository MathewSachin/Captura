using System;
using System.Windows.Input;

namespace Captura.Models
{
    public class FFMpegLog : NotifyPropertyChanged
    {
        FFMpegLog()
        {
            CopyToClipboardCommand = new DelegateCommand(() =>
            {
                Content.WriteToClipboard();
            });
        }

        public static FFMpegLog Instance { get; } = new FFMpegLog();

        string _content, _frame;

        public void Reset()
        {
            Content = Frame = "";
        }

        public void Write(string Text)
        {
            if (Text == null)
                return;

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
    }
}
