using System;
using System.Windows;

namespace Captura
{
    public partial class LayerFrame
    {
        public LayerFrame()
        {
            InitializeComponent();
        }

        public event Action<Rect> PositionUpdated;

        public void RaisePositionChanged(Rect Rect)
        {
            PositionUpdated?.Invoke(Rect);
        }
    }
}