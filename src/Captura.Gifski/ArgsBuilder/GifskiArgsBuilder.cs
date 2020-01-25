namespace Captura.Models
{
    class GifskiArgsBuilder
    {
        string _outputFile;
        string _inputFile;

        public GifskiArgsBuilder AddOutputFile(string FileName)
        {
            _outputFile = FileName;
            return this;
        }

        public GifskiArgsBuilder AddInputFile(string FileName)
        {
            _inputFile = FileName;
            return this;
        }

        public string GetArgs()
        {
            return $"-o {_outputFile} {_inputFile}";
        }
    }
}
