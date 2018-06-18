using Newtonsoft.Json;

namespace Captura.Models
{
    public class ImgurUploadResponse : ImgurResponse
    {
        [JsonProperty("data")]
        public ImgurData Data { get; set; }
    }
}