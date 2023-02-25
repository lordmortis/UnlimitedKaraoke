namespace UnlimitedKaraoke.Runtime.Moises
{
    public interface IJob
    {
        public System.Guid Id { get; }
        public string Name { get; }
        public JobState State { get; }
    }
}