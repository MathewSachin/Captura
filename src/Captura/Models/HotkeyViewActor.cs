namespace Captura.Models
{
    public class HotkeyViewActor : IHotkeyActor
    {
        public void Act(ServiceName Service)
        {
            switch (Service)
            {
                case ServiceName.OpenImageEditor:
                    new ImageEditorWindow().ShowAndFocus();
                    break;

                case ServiceName.ShowMainWindow:
                    MainWindow.Instance.ShowAndFocus();
                    break;
            }
        }
    }
}