using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace UnlimitedKaraoke.Runtime.Moises
{
    public class Uploader
    {
        public bool Busy { get; private set; }

        private readonly string apiKey;
        
        public Uploader(string apiKey)
        {
            this.apiKey = apiKey;
        }
        
        public async UniTaskVoid StartUpload(DefaultJob job)
        {
            Busy = true;
            job.State = JobState.Uploading;
            string url = DefaultManager.MoisesBaseUrl + "/api/upload";
            
            using UnityWebRequest getUrlRequest = new UnityWebRequest(url, "GET");
            getUrlRequest.SetRequestHeader("Authorization", apiKey);
            getUrlRequest.downloadHandler = new DownloadHandlerBuffer();
            await getUrlRequest.SendWebRequest();

            if (getUrlRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"web request error! {getUrlRequest.error}");
                Busy = false;
                job.State = JobState.Failed;
                return;
            }

            string responseString = Encoding.UTF8.GetString(getUrlRequest.downloadHandler.data);
            var response = JsonConvert.DeserializeObject<Responses.Upload>(responseString);

            url = response.UploadUrl;
            
            using UnityWebRequest uploadRequest = new UnityWebRequest(url, "PUT");
            uploadRequest.uploadHandler = new UploadHandlerFile(job.SourcePath);
            uploadRequest.downloadHandler = new DownloadHandlerBuffer();
            await uploadRequest.SendWebRequest();

            if (uploadRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"web request error! {getUrlRequest.error}");
                Busy = false;
                job.State = JobState.Failed;
                return;
            }

            job.State = JobState.Uploaded;
            job.SourceUrl = response.DownloadUrl;
            Busy = false;
        }
        
        
    }
}