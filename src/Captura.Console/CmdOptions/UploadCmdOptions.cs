using Captura.Models;
using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.IO;

namespace Captura
{
    [Verb("upload", HelpText = "Upload a file to a specified service.")]
    class UploadCmdOptions : ICmdlineVerb
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

        public void Run()
        {
            if (!File.Exists(FileName))
            {
                Console.WriteLine("File not found");
                return;
            }

            switch (Service)
            {
                case UploadService.imgur:
                    var imgSystem = ServiceProvider.Get<IImagingSystem>();
                    var img = imgSystem.LoadBitmap(FileName);
                    var uploader = ServiceProvider.Get<IImageUploader>();

                    // TODO: Show progress (on a single line)
                    var result = uploader.Upload(img, ImageFormats.Png, P => { }).Result;

                    Console.WriteLine(result.Url);
                    break;
            }
        }
    }
}
