namespace UnlimitedKaraoke.Runtime.Tracks
{
    public class DefaultTrack : ITrack
    {
        public System.Guid Id { get; set; }
        public string Name { get; set; }

        public bool Rename(string newName)
        {
            Name = newName;
            return true;
        }

        public string SourcePath { get; set; }
        public string VocalPath { get; set; }
        public string MusicPath { get; set; }
    }
}