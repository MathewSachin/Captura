using System;
using System.IO;
using System.Threading.Tasks;
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

        public void ReadLog(StreamReader ErrorStream, string FrameStart)
        {
            Task.Factory.StartNew(() =>
            {
                while (!ErrorStream.EndOfStream)
                {
                    var line = ErrorStream.ReadLine();

                    if (line.StartsWith(FrameStart))
                        Frame = line;
                    else Content += line + Environment.NewLine;
                }
            });
        }

        public string Frame
        {
            get => _frame;
            set
            {
                _frame = value;

                OnPropertyChanged();
            }
        }

        public string Content
        {
            get => _content;
            set
            {
                _content = value;

                OnPropertyChanged();
            }
        }

        public ICommand CopyToClipboardCommand { get; }
    }
}
