using System.Windows.Forms;
using Ookii.Dialogs;

namespace Captura.Models
{
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
    }
}