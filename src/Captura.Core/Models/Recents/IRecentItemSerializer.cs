using Newtonsoft.Json.Linq;

namespace Captura.Models
{
    public interface IRecentItemSerializer
    {
        bool CanSerialize(IRecentItem Item);

        bool CanDeserialize(JObject Item);

        JObject Serialize(IRecentItem Item);

        IRecentItem Deserialize(JObject Item);
    }
}