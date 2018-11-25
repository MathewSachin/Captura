using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UploadRecentSerializer : IRecentItemSerializer
    {
        readonly IEnumerable<IImageUploader> _imgUploaders;

        public UploadRecentSerializer(IEnumerable<IImageUploader> ImgUploaders)
        {
            _imgUploaders = ImgUploaders;
        }

        public bool CanSerialize(IRecentItem Item) => Item is UploadRecentItem;

        public bool CanDeserialize(JObject Item)
        {
            return Item.ContainsKey(nameof(UploadRecentModel.Type))
                   && Item[nameof(UploadRecentModel.Type)].ToString() == UploadRecentModel.IdValue;
        }

        class UploadRecentModel
        {
            public const string IdValue = "Upload";

            public string Type => IdValue;

            public string Link { get; set; }

            public string DeleteHash { get; set; }

            public string UploaderService { get; set; }
        }

        public JObject Serialize(IRecentItem Item)
        {
            if (Item is UploadRecentItem item)
            {
                return JObject.FromObject(new UploadRecentModel
                {
                    Link = item.Link,
                    DeleteHash = item.DeleteHash,
                    UploaderService = item.UploaderService.UploadServiceName
                });
            }

            return null;
        }

        public IRecentItem Deserialize(JObject Item)
        {
            try
            {
                var model = Item.ToObject<UploadRecentModel>();

                var uploader = _imgUploaders.FirstOrDefault(M => M.UploadServiceName == model.UploaderService);

                return uploader != null
                    ? new UploadRecentItem(model.Link, model.DeleteHash, uploader)
                    : null;
            }
            catch
            {
                return null;
            }
        }
    }
}