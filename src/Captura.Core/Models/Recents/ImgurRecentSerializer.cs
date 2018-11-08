using Newtonsoft.Json.Linq;

namespace Captura.Models
{
    public class ImgurRecentSerializer : IRecentItemSerializer
    {
        public bool CanSerialize(IRecentItem Item) => Item is ImgurRecentItem;

        public bool CanDeserialize(JObject Item)
        {
            return Item.ContainsKey(nameof(ImgurRecentModel.Type))
                   && Item[nameof(ImgurRecentModel.Type)].ToString() == ImgurRecentModel.IdValue;
        }

        class ImgurRecentModel
        {
            public const string IdValue = "Imgur";

            public string Type => IdValue;

            public string Link { get; set; }

            public string DeleteHash { get; set; }
        }

        public JObject Serialize(IRecentItem Item)
        {
            if (Item is ImgurRecentItem item)
            {
                return JObject.FromObject(new ImgurRecentModel
                {
                    Link = item.Link,
                    DeleteHash = item.DeleteHash
                });
            }

            return null;
        }

        public IRecentItem Deserialize(JObject Item)
        {
            try
            {
                var model = Item.ToObject<ImgurRecentModel>();

                return new ImgurRecentItem(model.Link, model.DeleteHash);
            }
            catch
            {
                return null;
            }
        }
    }
}