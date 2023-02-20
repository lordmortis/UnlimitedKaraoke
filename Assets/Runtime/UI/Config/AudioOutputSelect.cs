using System.Collections.Generic;
using UnityEngine;

namespace UnlimitedKaraoke.Runtime.UI.Config
{
    [RequireComponent(typeof(TMPro.TMP_Dropdown))]
    public class AudioOutputSelect : MonoBehaviour
    {
        private TMPro.TMP_Dropdown dropdown;
        //TODO: really, these should be decoupled via some application-level manager...
        private Audio.IAudioSystem audioSystem;
        private Settings.IManager settings;
        private readonly List<string> options = new();
        
        [Zenject.Inject]
        public void InjectDependencies(Audio.IAudioSystem audioSystem, Settings.IManager settings)
        {
            this.audioSystem = audioSystem;
            this.settings = settings;
        }
        
        public void Awake()
        {
            dropdown = GetComponent<TMPro.TMP_Dropdown>();
            dropdown.onValueChanged.AddListener(OnDropdownChanged);
            try
            {
                settings.Load();
            }
            catch (System.IO.FileNotFoundException)
            {
                return;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void OnDropdownChanged(int index)
        {
            audioSystem.SetOutput(audioSystem.Outputs[index]);
            settings.AudioOutput = audioSystem.Outputs[index];
        }

        public void OnEnable()
        {
            SetupOutputs(audioSystem.Outputs);
            audioSystem.OnOutputsUpdated += SetupOutputs;
        }

        public void OnDisable()
        {
            audioSystem.OnOutputsUpdated -= SetupOutputs;
        }

        private void SetupOutputs(IReadOnlyList<Audio.IAudioOutput> outputs)
        {
            options.Clear();
            if (outputs.Count == 0) return;

            int selectedIndex = 0;
            Audio.IAudioOutput selectedOutput = outputs[0];
            for (int i = 0; i < outputs.Count; i++)
            {
                var output = outputs[i];
                if (output.Equals(settings.AudioOutput))
                {
                    selectedOutput = output;
                    selectedIndex = i;
                }
                options.Add(output.Name);
            }
            
            dropdown.ClearOptions();
            dropdown.AddOptions(options);
            dropdown.value = selectedIndex;
            audioSystem.SetOutput(selectedOutput);
            settings.AudioOutput ??= selectedOutput;
        }
    }
}