using Newtonsoft.Json;

namespace UnlimitedKaraoke.Runtime.Moises.Responses
{
    public class CreateJob
    {
        [JsonProperty("jobId")]
        public  System.Guid JobId { get; set; }
        [JsonProperty("id")]
        public  System.Guid Id { get; set; }
    }
}