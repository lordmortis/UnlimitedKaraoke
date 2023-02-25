using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace UnlimitedKaraoke.Runtime.Moises
{
    public class DefaultJob : IJob
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(StringEnumConverter))] public JobState State { get; set; }
        public string VocalResultPath { get; set; }
        public string AccompanimentResultPath { get; set; }
        public string SourcePath { get; set; }
        public string SourceUrl { get; set; }

        [JsonIgnore] public Dictionary<string, string> Results;
    }
}