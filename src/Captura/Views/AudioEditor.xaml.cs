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
            if (DataContext is AudioEditorViewModel vm)
                vm.Load(FileName);
        }
    }
}
