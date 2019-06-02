using System.Linq;
using System.Windows.Media;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Captura
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class InkCanvasViewModel
    {
        public InkCanvasViewModel(ImageEditorSettings Settings)
        {
            SelectedColor = Settings
                .ToReactivePropertyAsSynchronized(
                    M => M.BrushColor,
                    M => M.ToWpfColor(),
                    M => M.ToDrawingColor());

            BrushSize = Settings
                .ToReactivePropertyAsSynchronized(M => M.BrushSize);

            SelectedTool = new ReactivePropertySlim<ExtendedInkTool>(ExtendedInkTool.Tools.First());
        }

        public IReactiveProperty<Color> SelectedColor { get; }

        public IReactiveProperty<int> BrushSize { get; }

        public IReactiveProperty<ExtendedInkTool> SelectedTool { get; }
    }
}