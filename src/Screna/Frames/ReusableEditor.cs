using System.Drawing;

namespace Screna
{
    public class ReusableEditor : EditorBase
    {
        public ReusableEditor(Graphics Graphics) : base(Graphics) { }

        public override void Dispose()
        {
            lock (Graphics)
            {
                Graphics.Flush();
            }
        }

        public void Destroy()
        {
            lock (Graphics)
            {
                Graphics.Dispose();
            }
        }
    }
}