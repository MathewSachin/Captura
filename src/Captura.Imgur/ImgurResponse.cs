using Newtonsoft.Json;

namespace Captura.Imgur
{
    class ImgurResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }
    }
}