using System;

namespace UnlimitedKaraoke.Runtime.Moises
{
    public class DefaultJob : IJob
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public JobState State { get; set; }
        
        public string SourcePath { get; set; }
        public string SourceUrl { get; set; }
    }
}