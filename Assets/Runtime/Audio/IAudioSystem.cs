using System.Collections.Generic;

namespace UnlimitedKaraoke.Runtime.Audio
{
    public interface IAudioSystem
    {
        IReadOnlyList<IAudioOutput> Outputs { get; }

        event System.Action<IReadOnlyList<IAudioOutput>> OnOutputsUpdated;

        void SetOutput(IAudioOutput output);
        
        void Play(string rootDirectory, Tracks.ITrack track);

        void Pause();
    }
}