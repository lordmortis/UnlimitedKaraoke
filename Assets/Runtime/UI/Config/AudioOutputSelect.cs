using System.Collections.Generic;
using UnityEngine;

namespace UnlimitedKaraoke.Runtime.UI.Config
{
    [RequireComponent(typeof(TMPro.TMP_Dropdown))]
    public class AudioOutputSelect : MonoBehaviour
    {
        private TMPro.TMP_Dropdown dropdown;
        private Audio.IAudioSystem audioSystem;
        private readonly List<string> options = new();
        
        [Zenject.Inject]
        public void InjectDependencies(Audio.IAudioSystem audioSystem)
        {
            this.audioSystem = audioSystem;
        }
        
        public void Awake()
        {
            dropdown = GetComponent<TMPro.TMP_Dropdown>();
            dropdown.onValueChanged.AddListener(OnDropdownChanged);
        }

        private void OnDropdownChanged(int index)
        {
            audioSystem.SetOutput(audioSystem.Outputs[index]);
            Debug.Log($"Selected -> {index}:{options[index]}");
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
            foreach (var output in outputs) options.Add(output.Name);
            dropdown.ClearOptions();
            dropdown.AddOptions(options);
            if (audioSystem.Outputs.Count == 0) return;
            audioSystem.SetOutput(audioSystem.Outputs[0]);
        }
    }
}