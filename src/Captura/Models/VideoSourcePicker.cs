using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Screna;

namespace Captura.Models
{
    public class VideoSourcePicker : IVideoSourcePicker
    {
        public Window PickWindow(IEnumerable<IntPtr> SkipWindows = null)
        {
            var picker = new WindowPicker(SkipWindows);

            picker.ShowDialog();

            return picker.SelectedWindow;
        }

        public Screen PickScreen()
        {
            var picker = new ScreenPicker();

            picker.ShowDialog();

            return picker.SelectedScreen;
        }
    }
}