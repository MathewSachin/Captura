namespace Captura
{
    public partial class AudioEditor
    {
        public AudioEditor()
        {
            InitializeComponent();
        }        

        public void Load(string FileName)
        {
            (DataContext as AudioEditorViewModel).Load(FileName);
        }
    }
}
