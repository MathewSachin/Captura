using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Captura.Native;

namespace Captura
{
    public class FileSavedNotification : INotification
    {
        readonly string _fileName;

        public FileSavedNotification(string FileName, string Message)
        {
            _fileName = FileName;

            PrimaryText = Message;
            SecondaryText = Path.GetFileName(FileName);

            var loc = ServiceProvider.Get<LanguageManager>();
            var icons = ServiceProvider.Get<IIconSet>();

            var deleteAction = new NotificationAction
            {
                Icon = icons.Delete,
                Name = loc.Delete,
                Color = "LightPink"
            };

            deleteAction.Click += () =>
            {
                if (File.Exists(_fileName))
                {
                    if (Shell32.FileOperation(_fileName, FileOperationType.Delete, 0) != 0)
                        return;
                }

                Remove();

                OnDelete?.Invoke();
            };

            Actions = new[] { deleteAction };
        }

        public event Action RemoveRequested;

        public event Action OnDelete;

        public void RaiseClick()
        {
            ServiceProvider.LaunchFile(new ProcessStartInfo(_fileName));
        }

        public void Remove() => RemoveRequested?.Invoke();

        public IEnumerable<NotificationAction> Actions { get; }

        int INotification.Progress => 0;

        public string PrimaryText { get; }

        public string SecondaryText { get; }

        bool INotification.Finished => true;
    }
}