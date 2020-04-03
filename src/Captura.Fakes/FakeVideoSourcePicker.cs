using System;
using System.Drawing;
using Captura.Video;

namespace Captura.Fakes
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FakeVideoSourcePicker : IVideoSourcePicker
    {
        FakeVideoSourcePicker() { }

        public static FakeVideoSourcePicker Instance { get; } = new FakeVideoSourcePicker();

        public IWindow SelectedWindow { get; set; }

        public IWindow PickWindow(Predicate<IWindow> Filter = null) => SelectedWindow;

        public IScreen SelectedScreen { get; set; }

        public IScreen PickScreen() => SelectedScreen;

        public Rectangle? PickRegion() => null;
    }
}