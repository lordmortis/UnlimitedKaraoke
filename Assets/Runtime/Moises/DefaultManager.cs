using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace UnlimitedKaraoke.Runtime.Moises
{
    [UsedImplicitly]
    public class DefaultManager : IManager, ITickable
    {
        public const string MoisesBaseUrl = "https://developer-api.moises.ai";
        
        public IReadOnlyList<IJob> Jobs { get; }
        public event Action<IJob> OnJobUpdate;
        public event Action<IReadOnlyList<IJob>> OnJobsUpdate;

        private Uploader uploader;
        private Downloader downloader;
        private readonly List<DefaultJob> jobs = new();
        private readonly Queue<DefaultJob> updatedJobs = new();
        private readonly Dictionary<Tracks.ITrack, DefaultJob> jobsForTrack = new();
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
            downloader = new Downloader(moisesKey);
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
                    case JobState.Processed:
                        if (job.Results == null)
                        {
                            job.State = JobState.Processing;
                            checkProcessing = true;
                            break;
                        }
                        if (downloader.Busy) continue;
                        downloader.Start(job).Forget();
                        break;
                    case JobState.Downloaded:
                        job.State = JobState.Complete;
                        updatedJobs.Enqueue(job);
                        break;
                }
            }

            if (checkProcessing) RefreshJobStatus().Forget();
        }

        public IJob AddTrack(Tracks.ITrack track)
        {
            if (jobsForTrack.TryGetValue(track, out var job))
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

        public IJob JobFor(Tracks.ITrack track)
        {
            return jobsForTrack.TryGetValue(track, out var job) ? job : null;
        }

        public void AddExistingJob(Tracks.ITrack track, IJob job)
        {
            if (jobsForTrack.ContainsKey(track))
            {
                Debug.LogError($"already have a job for track: {track}");
                return;
            }

            if (job is not DefaultJob realJob) return;
            jobsForTrack[track] = realJob;
            jobs.Add(realJob);
        }

        public async UniTaskVoid RemoveJob(IJob job)
        {
            await UniTask.Delay(100);
        }

        private async UniTaskVoid Process(DefaultJob job)
        {
            job.State = JobState.Processing;

            var requestPayload = new Requests.CreateJob
            {
                Name = $"UK: {job.Name}",
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

        private async UniTaskVoid RefreshJobStatus()
        {
            if (asyncBusy) return;
            asyncBusy = true;

            string url = MoisesBaseUrl + "/api/job";
            using UnityWebRequest statusRequest = new UnityWebRequest(url, "GET");
            statusRequest.SetRequestHeader("Authorization", moisesKey);
            statusRequest.SetRequestHeader("content-type", "application/json; charset=utf-8");
            statusRequest.downloadHandler = new DownloadHandlerBuffer();
            await statusRequest.SendWebRequest();

            if (statusRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"web request error! {statusRequest.error}");
                return;
            }

            string responseString = Encoding.UTF8.GetString(statusRequest.downloadHandler.data);
            var jsonJobs = JsonConvert.DeserializeObject<Responses.Job[]>(responseString);

            foreach (var jsonJob in jsonJobs)
            {
                var job = jobs.Find((x) => x.Id == jsonJob.Id);
                if (job == null) continue;
                switch (jsonJob.JobStatus)
                {
                    case Responses.JobStatus.Failed:
                        job.State = JobState.Failed;
                        updatedJobs.Enqueue(job);
                        break;
                    case Responses.JobStatus.Succeeded:
                        job.State = JobState.Processed;
                        job.Results = jsonJob.Results;
                        updatedJobs.Enqueue(job);
                        break;
                }
            }
            asyncBusy = false;
        }
    }
}