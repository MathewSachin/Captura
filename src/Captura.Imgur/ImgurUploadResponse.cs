using Newtonsoft.Json;

namespace Captura.Models
{
    class ImgurUploadResponse : ImgurResponse
    {
        [JsonProperty("data")]
        public ImgurData Data { get; set; }
    }
}