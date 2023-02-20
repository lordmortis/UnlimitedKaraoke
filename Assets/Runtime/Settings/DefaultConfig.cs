using UnlimitedKaraoke.Runtime.Audio;

namespace UnlimitedKaraoke.Runtime.Settings
{
    public record DefaultConfig
    {
        public IAudioOutput AudioOutput { get; set; }
        public string DataDirectory { get; set; }
    }
}