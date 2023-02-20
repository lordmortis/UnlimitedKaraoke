using System;
using UnityEngine;

namespace UnlimitedKaraoke.Runtime.UI.Config
{
    [RequireComponent(typeof(TMPro.TMP_Text))]
    public class DataPathText : MonoBehaviour
    {
        private TMPro.TMP_Text textBox;

        private Settings.IManager settings;
        
        [Zenject.Inject]
        public void InjectDependencies(Settings.IManager settings)
        {
            this.settings = settings;
        }        
        
        public void Awake()
        {
            textBox = GetComponent<TMPro.TMP_Text>();
        }
        
        public void OnEnable()
        {
            settings.OnSettingsUpdated += HandleSettingsUpdated;
            HandleSettingsUpdated(settings);
        }

        public void OnDisable()
        {
            settings.OnSettingsUpdated -= HandleSettingsUpdated;
        }
        
        private void HandleSettingsUpdated(Settings.IManager settings)
        {
            textBox.text = settings.DataDirectory ?? (textBox.text = "Unset");
        }
    }
}