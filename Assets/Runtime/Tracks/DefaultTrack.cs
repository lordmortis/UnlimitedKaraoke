using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnlimitedKaraoke.Runtime.Moises;

namespace UnlimitedKaraoke.Runtime.Tracks
{
    public class DefaultTrack : ITrack
    {
        public System.Guid Id { get; set; }

        public IJob MoisesJob { get; set;  }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public TrackState State { get; set; }
        public string Name { get; set; }

        public bool Rename(string newName)
        {
            Name = newName;
            return true;
        }

        public string SourcePath { get; set; }
        public string VocalPath { get; set; }
        public string MusicPath { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is not DefaultTrack otherTrack) return false;
            return otherTrack.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Id.ToString()} - {Name}";
        }
    }
}