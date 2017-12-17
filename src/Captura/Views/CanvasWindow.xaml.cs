using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Captura
{
    public partial class CanvasWindow
    {
        public CanvasWindow()
        {
            InitializeComponent();

            ModesBox.SelectedIndex = 0;
            ColorPicker.SelectedColor = Color.FromRgb(27, 27, 27);
            SizeBox.Value = 5;
            FitToCurveCheckbox.IsChecked = true;
        }

        public KeyValuePair<InkCanvasEditingMode, string>[] Modes { get; } =
        {
            new KeyValuePair<InkCanvasEditingMode, string>(InkCanvasEditingMode.Ink, "Pencil"),
            new KeyValuePair<InkCanvasEditingMode, string>(InkCanvasEditingMode.EraseByPoint, "Eraser"),
            new KeyValuePair<InkCanvasEditingMode, string>(InkCanvasEditingMode.EraseByStroke, "Stroke Eraser"),
            new KeyValuePair<InkCanvasEditingMode, string>(InkCanvasEditingMode.Select, nameof(InkCanvasEditingMode.Select))
        };

        void ModesBox_OnSelectionChanged(object Sender, SelectionChangedEventArgs E)
        {
            if (ModesBox.SelectedValue is InkCanvasEditingMode mode)
            {
                InkCanvas.EditingMode = mode;
            }
        }

        void ColorPicker_OnSelectedColorChanged(object Sender, RoutedPropertyChangedEventArgs<Color?> E)
        {
            if (E.NewValue != null && InkCanvas != null)
                InkCanvas.DefaultDrawingAttributes.Color = E.NewValue.Value;
        }

        void Highlight_OnClick(object Sender, RoutedEventArgs E)
        {
            if (InkCanvas != null && Sender is CheckBox checkBox)
                InkCanvas.DefaultDrawingAttributes.IsHighlighter = checkBox.IsChecked.GetValueOrDefault();
        }

        void FitToCurve_OnClick(object Sender, RoutedEventArgs E)
        {
            if (InkCanvas != null && Sender is CheckBox checkBox)
                InkCanvas.DefaultDrawingAttributes.FitToCurve = checkBox.IsChecked.GetValueOrDefault();
        }

        void SizeBox_OnValueChanged(object Sender, RoutedPropertyChangedEventArgs<object> E)
        {
            if (InkCanvas != null && E.NewValue is int i)
                InkCanvas.DefaultDrawingAttributes.Height = InkCanvas.DefaultDrawingAttributes.Width = i;
        }
    }
}
