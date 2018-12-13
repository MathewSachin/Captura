﻿using System.Windows.Forms;
using Ookii.Dialogs;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DialogService : IDialogService
    {
        public string PickFolder(string Current, string Description)
        {
            using (var dlg = new VistaFolderBrowserDialog
            {
                SelectedPath = Current,
                UseDescriptionForTitle = true,
                Description = Description
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    return dlg.SelectedPath;
            }

            return null;
        }

        public string PickFile(string InitialFolder, string Description)
        {
            var ofd = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                InitialDirectory = InitialFolder,
                Title = Description
            };

            return ofd.ShowDialog() == DialogResult.OK ? ofd.FileName : null;
        }
    }
}