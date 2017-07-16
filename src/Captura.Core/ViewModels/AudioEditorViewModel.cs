namespace Captura.ViewModels
{
    public class AudioEditorViewModel : ViewModelBase
    {
        public string FilePath { get; }

        public AudioEditorViewModel(string FilePath)
        {
            this.FilePath = FilePath;
        }
    }
}
