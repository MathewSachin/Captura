using System;
using System.Windows.Input;

namespace Captura.Models
{
    public class FFMpegLogItem : NotifyPropertyChanged
    {
        public string Name { get; }

        public FFMpegLogItem(string Name)
        {
            this.Name = Name;
            CopyToClipboardCommand = new DelegateCommand(() =>
            {
                Content.WriteToClipboard();
            });

            RemoveCommand = new DelegateCommand(() => RemoveRequested?.Invoke());
        }

        string _content, _frame;

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

        public ICommand RemoveCommand { get; }

        public event Action RemoveRequested;
    }
}