namespace Captura.Video
{
    public interface IVideoSourceProvider
    {
        string Name { get; }

        string Description { get; }

        string Icon { get; }

        IVideoItem Source { get; }

        bool SupportsStepsMode { get; }

        IBitmapImage Capture(bool IncludeCursor);

        bool OnSelect();

        void OnUnselect();

        string Serialize();

        bool Deserialize(string Serialized);

        bool ParseCli(string Arg);
    }
}