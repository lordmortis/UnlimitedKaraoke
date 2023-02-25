namespace UnlimitedKaraoke.Runtime.Moises
{
    public enum JobState
    {
        NotStarted,
        Uploading,
        Uploaded,
        Processing,
        Processed,
        Failed,
        Downloading,
        Downloaded,
        Complete,
    }
}