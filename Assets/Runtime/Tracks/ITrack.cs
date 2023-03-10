namespace UnlimitedKaraoke.Runtime.Tracks
{
    public interface ITrack
    {
        public System.Guid Id { get; }
        public Moises.IJob MoisesJob { get; }
        public TrackState State { get; }

        public string Name { get; }
        
        public string SourcePath { get; }
        public string VocalPath { get; }
        public string MusicPath { get; }
    }
}