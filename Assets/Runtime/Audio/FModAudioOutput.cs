using System;

namespace UnlimitedKaraoke.Runtime.Audio
{
    public class FModAudioOutput : IAudioOutput, IEquatable<IAudioOutput>
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public FMOD.SPEAKERMODE SpeakerMode { get; set; }
        public int SpeakerModeChannels { get; set; }
        public int SamplingRate { get; set; }
        public System.Guid DeviceGuid { get; set; }

        public bool Equals(IAudioOutput other)
        {
            if (other is not FModAudioOutput fmodOutput) return false;
            return DeviceGuid.Equals(fmodOutput.DeviceGuid);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((FModAudioOutput) obj);
        }

        public override int GetHashCode()
        {
            return DeviceGuid.GetHashCode();
        }
        
        public static bool operator ==(FModAudioOutput lhs, FModAudioOutput rhs)
        {
            if (lhs is null) return rhs is null;
            // Equals handles the case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(FModAudioOutput lhs, FModAudioOutput rhs) => !(lhs == rhs);       
    }
}