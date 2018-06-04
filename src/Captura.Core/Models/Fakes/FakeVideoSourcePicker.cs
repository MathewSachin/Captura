using Screna;

namespace Captura.Models
{
    public class FakeVideoSourcePicker : IVideoSourcePicker
    {
        public Window PickWindow()
        {
            return null;
        }
    }
}