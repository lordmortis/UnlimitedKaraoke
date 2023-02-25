using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnlimitedKaraoke.Runtime.Tracks;
using Zenject;

namespace UnlimitedKaraoke.Runtime.Moises
{
    public class DefaultManager : IManager, ITickable
    {
        public const string MoisesBaseUrl = "https://developer-api.moises.ai";
        
        public IReadOnlyList<IJob> Jobs { get; }
        public event Action<IJob> OnJobUpdate;
        public event Action<IReadOnlyList<IJob>> OnJobsUpdate;

        private Uploader uploader;
        private readonly List<DefaultJob> jobs = new();
        private readonly Queue<DefaultJob> updatedJobs = new();
        private readonly Dictionary<ITrack, DefaultJob> jobsForTrack = new();
        private bool asyncBusy;
        private bool jobsModified;
        private string moisesKey;

        public DefaultManager(Settings.IManager settings)
        {
            settings.OnSettingsUpdated += SettingsUpdated;
            SettingsUpdated(settings);
            Jobs = jobs.AsReadOnly();
        }

        private void SettingsUpdated(Settings.IManager settings)
        {
            if (settings.MoisesKey == moisesKey) return;
            moisesKey = settings.MoisesKey;
            uploader = new Uploader(moisesKey); 
        }

        public void Tick()
        {
            if (updatedJobs.Count > 0)
            {
                while (updatedJobs.TryDequeue(out var job)) OnJobUpdate?.Invoke(job);
                OnJobsUpdate?.Invoke(Jobs);
            }

            foreach (var job in jobs)
            {
                if (job.State == JobState.NotStarted)
                {
                    if (uploader.Busy) continue;
                    uploader.StartUpload(job).Forget();
                }
            }
        }
        
        public IJob AddTrack(ITrack track)
        {
            DefaultJob job;
            if (jobsForTrack.TryGetValue(track, out job))
            {
                Debug.LogError($"Job exists for track {track}");
                return job;
            }

            job = new DefaultJob()
            {
                Name = track.Name,
                State = JobState.NotStarted,
                SourcePath = track.SourcePath,
            };
            
            jobsForTrack.Add(track, job);
            jobs.Add(job);
            updatedJobs.Enqueue(job);
            jobsModified = true;
            return job;
        }

        public IJob JobFor(ITrack track)
        {
            return jobsForTrack.TryGetValue(track, out var job) ? job : null;
        }
    }
}