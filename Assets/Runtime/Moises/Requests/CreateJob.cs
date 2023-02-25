using Newtonsoft.Json;

namespace UnlimitedKaraoke.Runtime.Moises.Requests
{
    public class CreateJob
    {
        public class JobParams
        {
            [JsonProperty("inputUrl")]
            public string InputUrl { get; set; }
        }
        
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("workflow")] 
        public string Workflow { get; set; } = "moises/stems-vocals-accompaniment";

        [JsonProperty("params")]
        public JobParams Params { get; } = new();
    }
}