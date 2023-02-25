using System.Collections.Generic;

namespace UnlimitedKaraoke.Runtime.Tracks
{
    public interface IManager
    {
        event System.Action<IReadOnlyList<ITrack>> OnTracksUpdated;
        IReadOnlyList<ITrack> Tracks { get; }
        
        public ITrack AddTrack(string sourcePath);

        public ITrack AddTrack(string sourcePath, string name);

        public void DeleteTrack(ITrack track);

        public ITrack RenameTrack(ITrack track, string newName);
    }
}