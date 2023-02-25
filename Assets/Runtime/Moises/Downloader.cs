using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine.Device;
using UnityEngine.Networking;

namespace UnlimitedKaraoke.Runtime.Moises
{
    public class Downloader
    {
        public bool Busy { get; private set; }
        private readonly string apiKey;
        
        public Downloader(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public async UniTaskVoid Start(DefaultJob job)
        {
            Busy = true;
            job.State = JobState.Downloading;

            var tempPath = Path.Join(
                Application.temporaryCachePath,
                job.Id.ToString());
            if (Directory.Exists(tempPath)) Directory.Delete(tempPath, true);
            Directory.CreateDirectory(tempPath);

            Dictionary<string, string> pathsForResults = new();

            foreach (KeyValuePair<string,string> pair in job.Results)
            {
                using UnityWebRequest getUrlRequest = new UnityWebRequest(pair.Value, "GET");
                var downloadPath = Path.Join(tempPath, $"{pair.Key}.wav");
                getUrlRequest.downloadHandler = new DownloadHandlerFile(downloadPath);
                await getUrlRequest.SendWebRequest();
                if (getUrlRequest.result != UnityWebRequest.Result.Success)
                {
                    UnityEngine.Debug.LogError($"web request error! {getUrlRequest.error}");
                    Busy = false;
                    job.State = JobState.Failed;
                    continue;
                }

                pathsForResults[pair.Key] = downloadPath;
            }

            job.State = JobState.Downloaded;
            if (pathsForResults.TryGetValue("accompaniment", out string path))
            {
                job.AccompanimentResultPath = path;
            } 
            
            if (pathsForResults.TryGetValue("vocals", out path))
            {
                job.VocalResultPath = path;
            } 

            Busy = false;
        }
    }
}