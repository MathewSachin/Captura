using Newtonsoft.Json.Linq;

namespace Captura.Models
{
    public class FileRecentSerializer : IRecentItemSerializer
    {
        public bool CanSerialize(IRecentItem Item) => Item is ImgurRecentItem;

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
            if (Item is FileRecentItem item)
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

                return new FileRecentItem(model.FileName, model.FileType);
            }
            catch
            {
                return null;
            }
        }
    }
}