using System.Drawing;

namespace Screna
{
    class OneTimeEditor : EditorBase
    {
        public OneTimeEditor(Graphics Graphics) : base(Graphics) { }

        public override void Dispose()
        {
            lock (Graphics)
            {
                Graphics.Dispose();
            }
        }
    }
}