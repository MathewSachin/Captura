using System.IO;
using Newtonsoft.Json.Linq;

namespace Captura.Models
{
    public class FileRecentSerializer : IRecentItemSerializer
    {
        public bool CanSerialize(IRecentItem Item) => Item is FileRecentItem;

        public bool CanDeserialize(JObject Item)
        {
            return Item.ContainsKey(nameof(FileRecentModel.Type))
                   && Item[nameof(FileRecentModel.Type)].ToString() == FileRecentModel.IdValue;
        }

        class FileRecentModel
        {
            public const string IdValue = "File";

            public string Type => IdValue;

            public RecentFileType FileType { get; set; }

            public string FileName { get; set; }
        }

        public JObject Serialize(IRecentItem Item)
        {
            // Persist only if File exists or is a link.
            if (Item is FileRecentItem item && File.Exists(item.FileName))
            {
                return JObject.FromObject(new FileRecentModel
                {
                    FileName = item.FileName,
                    FileType = item.FileType
                });
            }

            return null;
        }

        public IRecentItem Deserialize(JObject Item)
        {
            try
            {
                var model = Item.ToObject<FileRecentModel>();

                // Restore only if file exists
                return File.Exists(model.FileName)
                    ? new FileRecentItem(model.FileName, model.FileType)
                    : null;
            }
            catch
            {
                return null;
            }
        }
    }
}