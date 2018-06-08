using System.Windows.Forms;
using Screna;

namespace Captura.Models
{
    public interface IVideoSourcePicker
    {
        Window PickWindow();

        Screen PickScreen();
    }
}