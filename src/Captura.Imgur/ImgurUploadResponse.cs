using Newtonsoft.Json;

namespace Captura.Imgur
{
    class ImgurUploadResponse : ImgurResponse
    {
        [JsonProperty("data")]
        public ImgurData Data { get; set; }
    }
}