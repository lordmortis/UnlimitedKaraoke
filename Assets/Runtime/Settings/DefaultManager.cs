using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnlimitedKaraoke.Runtime.Audio;

namespace UnlimitedKaraoke.Runtime.Settings
{
    
    // ReSharper disable once ClassNeverInstantiated.Global - instantiated via Zenject
    public class DefaultManager : IManager
    {
        private const string PreferencesFile = "Preferences.json";

        public event Action<IManager> OnSettingsUpdated;

        public IAudioOutput AudioOutput
        {
            get => currentConfig?.AudioOutput;
            set
            {
                currentConfig ??= new DefaultConfig();
                currentConfig.AudioOutput = value;
                OnSettingsUpdated?.Invoke(this);
            }
        }

        public string DataDirectory
        {
            get => currentConfig?.DataDirectory;
            set
            {
                currentConfig ??= new DefaultConfig();
                currentConfig.DataDirectory = value;
                OnSettingsUpdated?.Invoke(this);
            }
        }

        public string MoisesKey
        {
            get => currentConfig?.MoisesKey;
            set
            {
                currentConfig ??= new DefaultConfig();
                currentConfig.MoisesKey = value;
                OnSettingsUpdated?.Invoke(this);
            }
        }

        private readonly string preferencesPath;
        private DefaultConfig currentConfig = new();
        private readonly JsonSerializer serializer;
 
            public DefaultManager()
        {
            preferencesPath = Path.Join(
                Application.persistentDataPath,
                PreferencesFile);

            serializer = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.Auto,
            };
#if UNITY_EDITOR
            Debug.Log($"Save path is: {preferencesPath}");
#endif
            try
            {
                Load();
            }
            catch (FileNotFoundException)
            {
                Debug.LogError("No preferences file");
            }
        }
        
        public void Save()
        {
            var tempPath = Path.Join(Application.temporaryCachePath, PreferencesFile);
            using (var rawStream = new StreamWriter(tempPath))
            using (var jsonStream = new JsonTextWriter(rawStream))
            {
                serializer.Serialize(jsonStream, currentConfig);
            }
            
            File.Delete(preferencesPath);
            File.Move(tempPath, preferencesPath);
        }

        public bool Load()
        {
            if (!File.Exists(preferencesPath))
            {
                throw new FileNotFoundException("Could not find preferences");
            }

            DefaultConfig newConfig;
            
            using (var rawStream = new StreamReader(preferencesPath))
            using (var jsonStream = new JsonTextReader(rawStream))
            {
                newConfig = serializer.Deserialize<DefaultConfig>(jsonStream);
            }
            
            if (newConfig == null) return false;
            if (newConfig == currentConfig) return false;
            currentConfig = newConfig;
            OnSettingsUpdated?.Invoke(this);
            return true;
        }
    }
}