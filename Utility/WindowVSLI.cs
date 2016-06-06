using Screna;

namespace Captura
{
    class WindowVSLI : IVideoSourceListItem
    {
        public Window Window { get; }

        public static readonly WindowVSLI Desktop = new WindowVSLI(Window.DesktopWindow, "[Desktop]"),
            TaskBar = new WindowVSLI(Window.Taskbar, "[TaskBar]");

        public WindowVSLI(Window Window)
        {
            this.Window = Window;
            Name = Window.Title;
        }

        public WindowVSLI(Window Window, string Name)
        {
            this.Window = Window;
            this.Name = Name;
        }

        public string Name { get; }
    }
}