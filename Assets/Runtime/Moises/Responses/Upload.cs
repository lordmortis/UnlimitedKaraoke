using Unity.Plastic.Newtonsoft.Json;

namespace UnlimitedKaraoke.Runtime.Moises.Responses
{
    public class Upload
    {
        [JsonProperty("uploadUrl")]
        public string UploadUrl { get; set; }

        [JsonProperty("downloadUrl")]
        public string DownloadUrl { get; set; }
    }
}