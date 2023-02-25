using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
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

            bool checkProcessing = false;
            
            foreach (var job in jobs)
            {
                switch (job.State)
                {
                    case JobState.NotStarted:
                        if (uploader.Busy) continue;
                        uploader.StartUpload(job).Forget();
                        break;
                    case JobState.Uploaded:
                        Process(job).Forget();
                        break;
                    case JobState.Processing:
                        checkProcessing = true;
                        break;
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
            return job;
        }

        public IJob JobFor(ITrack track)
        {
            return jobsForTrack.TryGetValue(track, out var job) ? job : null;
        }
        
        private async UniTaskVoid Process(DefaultJob job)
        {
            job.State = JobState.Processing;

            var requestPayload = new Requests.CreateJob
            {
                Name = job.Name,
                Params =
                {
                    InputUrl = job.SourceUrl,
                },
            };

            string url = MoisesBaseUrl + "/api/job";
            using UnityWebRequest jobRequest = new UnityWebRequest(url, "POST");
            jobRequest.SetRequestHeader("Authorization", moisesKey);
            jobRequest.SetRequestHeader("content-type", "application/json; charset=utf-8");
            byte[] serializedPayload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(requestPayload));
            jobRequest.uploadHandler = new UploadHandlerRaw(serializedPayload);
            jobRequest.downloadHandler = new DownloadHandlerBuffer();
            await jobRequest.SendWebRequest();

            if (jobRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"web request error! {jobRequest.error}");
                job.State = JobState.Failed;
                return;
            }

            string responseString = Encoding.UTF8.GetString(jobRequest.downloadHandler.data);
            var response = JsonConvert.DeserializeObject<Responses.CreateJob>(responseString);

            job.Id = response.JobId;
            updatedJobs.Enqueue(job);
        }
    }
}