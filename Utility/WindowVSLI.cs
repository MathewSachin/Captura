using Screna;

namespace Captura
{
    class WindowVSLI
    {
        public Window Window { get; }

        public static readonly WindowVSLI Desktop = new WindowVSLI(Window.DesktopWindow, "[Desktop]"),
            TaskBar = new WindowVSLI(Window.Taskbar, "[TaskBar]");

        public WindowVSLI(Window Window)
        {
            this.Window = Window;
            _name = Window.Title;
        }

        public WindowVSLI(Window Window, string Name)
        {
            this.Window = Window;
            _name = Name;
        }

        readonly string _name;
        
        public override string ToString() => _name;
    }
}