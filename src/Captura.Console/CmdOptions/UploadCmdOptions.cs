using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace Captura
{
    enum UploadService
    {
        imgur,
        youtube
    }

    [Verb("upload", HelpText = "Upload a file to a specified service.")]
    class UploadCmdOptions
    {
        [Value(0, HelpText = "The service to upload to")]
        public UploadService Service { get; set; }

        [Value(1, HelpText = "The file to upload")]
        public string FileName { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Upload image to Imgur", new UploadCmdOptions
                {
                    Service = UploadService.imgur,
                    FileName = "image.png"
                });
            }
        }
    }
}
