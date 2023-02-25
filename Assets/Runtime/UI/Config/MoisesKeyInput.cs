using UnityEngine;

namespace UnlimitedKaraoke.Runtime.UI.Config
{
    [RequireComponent(typeof(TMPro.TMP_InputField))]
    public class MoisesKeyInput : MonoBehaviour
    {
        private TMPro.TMP_InputField inputField;

        private Settings.IManager settings;
        private string currentValue;

        [Zenject.Inject]
        public void InjectDependencies(Settings.IManager settings)
        {
            this.settings = settings;
        }

        public void Awake()
        {
            inputField = GetComponent<TMPro.TMP_InputField>();
            currentValue = settings.MoisesKey;
            inputField.text = currentValue ?? "";
            inputField.onValueChanged.AddListener(OnValueChanged);
        }
        
        public void OnEnable()
        {
            settings.OnSettingsUpdated += SettingsUpdated;
        }

        public void OnDisable()
        {
            settings.OnSettingsUpdated -= SettingsUpdated;
        }
        
        private void SettingsUpdated(Settings.IManager settingsManager)
        {
            if (currentValue == settings.MoisesKey) return;
            currentValue = settings.MoisesKey;
            inputField.text = currentValue ?? "";
        }

        private void OnValueChanged(string value)
        {
            settings.MoisesKey = value;
        }
    }
}