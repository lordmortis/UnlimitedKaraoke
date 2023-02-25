using System.Collections.Generic;

namespace UnlimitedKaraoke.Runtime.Moises
{
    public interface IManager
    {
        IReadOnlyList<IJob> Jobs { get; }
        event System.Action<IJob> OnJobUpdate;
        event System.Action<IReadOnlyList<IJob>> OnJobsUpdate;

        IJob AddTrack(Tracks.ITrack track);
        IJob JobFor(Tracks.ITrack track);
        void AddExistingJob(Tracks.ITrack track, IJob job);
    }
}