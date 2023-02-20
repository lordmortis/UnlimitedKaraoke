using UnlimitedKaraoke.Runtime.Audio;

namespace UnlimitedKaraoke.Runtime.Settings
{
    public interface IManager
    {
        event System.Action<IManager> OnSettingsUpdated;
        IAudioOutput AudioOutput { get; set; }
        string DataDirectory { get; set; }

        bool Load();
        void Save();
    }
}