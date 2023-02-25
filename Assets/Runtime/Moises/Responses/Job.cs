using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace UnlimitedKaraoke.Runtime.Moises.Responses
{
    public enum JobStatus
    {
        Unknown,
        Queued,
        Started,
        Succeeded,
        Failed
    }

    public class Job
    {
        [JsonProperty("id")]
        public System.Guid Id { get; set; }
        
        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public JobStatus JobStatus { get; set; }

        [JsonProperty("result")]
        public Dictionary<string, string> Results;
    }
}