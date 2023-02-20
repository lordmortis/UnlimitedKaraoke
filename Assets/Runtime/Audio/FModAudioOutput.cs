namespace UnlimitedKaraoke.Runtime.Audio
{
    public class FModAudioOutput : IAudioOutput
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public FMOD.SPEAKERMODE SpeakerMode { get; set; }
        public int SpeakerModeChannels { get; set; }
        public int SamplingRate { get; set; }
    }
}