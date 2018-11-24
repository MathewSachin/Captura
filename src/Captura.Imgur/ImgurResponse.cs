using Newtonsoft.Json;

namespace Captura.Models
{
    class ImgurResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }
    }
}