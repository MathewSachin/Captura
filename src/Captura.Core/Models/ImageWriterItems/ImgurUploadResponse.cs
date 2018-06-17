using Newtonsoft.Json;

namespace Captura.Models
{
    public class ImgurUploadResponse
    {
        [JsonProperty("data")]
        public ImgurData Data { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }
    }
}