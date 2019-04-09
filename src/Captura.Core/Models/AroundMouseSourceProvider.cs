using System;

namespace Captura.Models
{
    class AroundMouseSourceProvider : IVideoSourceProvider
    {
        public AroundMouseSourceProvider(IIconSet Icons,
            AroundMouseItem AroundMouseItem)
        {
            Icon = Icons.Cursor;
            Source = AroundMouseItem;
        }

        public string Name => "Around Mouse";

        public string Description => "Capture region surrounding mouse";

        public string Icon { get; }

        public IVideoItem Source { get; }

        public event Action UnselectRequested;

        public bool Deserialize(string Serialized) => false;

        public bool OnSelect() => true;

        public void OnUnselect()
        {
        }

        public bool ParseCli(string Arg) => false;

        public string Serialize() => "";
    }
}
