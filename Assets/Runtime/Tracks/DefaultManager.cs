using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Zenject;


namespace UnlimitedKaraoke.Runtime.Tracks
{
    // ReSharper disable once ClassNeverInstantiated.Global - instantiated via Zenject
    public class DefaultManager : IManager, ITickable
    {
        private const string TrackDataFile = "trackinfo.json";
        
        public event System.Action<IReadOnlyList<ITrack>> OnTracksUpdated;

        public IReadOnlyList<ITrack> Tracks { get; private set; }

        private string currentDataPath = null;
        private readonly List<DefaultTrack> tracks = new(); 
        private readonly JsonSerializer serializer;
        private bool listUpdated;

        public DefaultManager(Settings.IManager settings)
        {
            serializer = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.Auto,
            };

            Tracks = tracks.AsReadOnly();
            settings.OnSettingsUpdated += SettingsUpdated;
            SettingsUpdated(settings);
        }

        public void Tick()
        {
            if (!listUpdated) return;

            OnTracksUpdated?.Invoke(Tracks);
            listUpdated = false;
        }        
        

        private void SettingsUpdated(Settings.IManager settings)
        {
            if (currentDataPath == settings.DataDirectory) return;
            tracks.Clear();
            foreach (var directory in Directory.GetDirectories(settings.DataDirectory))
            {
#if UNITY_EDITOR
                UnityEngine.Debug.Log($"Checking {directory}");
#endif
                var trackPath = Path.Join(directory, TrackDataFile);
                if (!File.Exists(trackPath)) continue;

                DefaultTrack trackData;

                using var rawStream = new StreamReader(trackPath);
                using(var jsonStream = new JsonTextReader(rawStream))
                {
                    trackData = serializer.Deserialize<DefaultTrack>(jsonStream);
                }

                if (trackData == null) continue;
                tracks.Add(trackData);
            }
            currentDataPath = settings.DataDirectory;
            listUpdated = true;
        }

        public ITrack AddTrack(string sourcePath) => AddTrack(sourcePath, "New Track");

        public ITrack AddTrack(string sourcePath, string name)
        {
            if (!File.Exists(sourcePath))
            {
                UnityEngine.Debug.LogError("Could not find source path");
                return null;
            }

            if (currentDataPath == null)
            {
                UnityEngine.Debug.LogError("Current data path is null");
                return null;
            }

            var trackData = new DefaultTrack()
            {
                Id = System.Guid.NewGuid(),
                Name = name,
                SourcePath = sourcePath,
            };

            Update(trackData);
            tracks.Add(trackData);
            listUpdated = true;
            return trackData;
        }

        public void DeleteTrack(ITrack track)
        {
            if (track is not DefaultTrack realTrack) return;
            string dirPath = Path.Join(currentDataPath, realTrack.Id.ToString());
            if (!Directory.Exists(dirPath)) return;
            Directory.Delete(dirPath, true);
            tracks.Remove(realTrack);
            listUpdated = true;
        }

        public ITrack RenameTrack(ITrack track, string newName)
        {
            if (track is not DefaultTrack realTrack) return null;
            realTrack.Name = newName;
            Update(realTrack);
            listUpdated = true;
            return realTrack;
        }

        private void Update(DefaultTrack trackData)
        {
            string dirPath = Path.Join(currentDataPath, trackData.Id.ToString());
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            var tempPath = Path.Join(
                UnityEngine.Application.temporaryCachePath, 
                $"{trackData.Id.ToString()}-trackData.json");

            using var rawStream = new StreamWriter(tempPath);
            using var jsonStream = new JsonTextWriter(rawStream);
            serializer.Serialize(jsonStream, trackData);

            string dataPath = Path.Join(dirPath, TrackDataFile);
            File.Delete(dataPath);
            File.Move(tempPath, dataPath);
        }
    }
}