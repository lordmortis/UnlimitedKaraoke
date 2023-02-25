using System.IO;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UnlimitedKaraoke.Runtime.UI
{
    public class AddTrackController : MonoBehaviour
    {
        [SerializeField] private Button pathButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private TMPro.TMP_InputField trackNameField;
        [FormerlySerializedAs("pathDisplay"),SerializeField] private TMPro.TMP_Text trackPathDisplay;

        private string trackName;
        private string trackPath;

        private Runtime.Tracks.IManager trackManager;

        [Zenject.Inject]
        public void InjectDependencies(Runtime.Tracks.IManager trackManager)
        {
            this.trackManager = trackManager;
        }
        
        public void OnEnable()
        {
            pathButton.onClick.AddListener(OnPathPressed);
            saveButton.onClick.AddListener(OnSavePressed);
            resetButton.onClick.AddListener(OnResetPressed);
            trackNameField.onValueChanged.AddListener(OnTrackNameChanged);
            saveButton.enabled = false;
        }

        public void OnDisable()
        {
            pathButton.onClick.RemoveListener(OnPathPressed);
            saveButton.onClick.RemoveListener(OnSavePressed);
            resetButton.onClick.RemoveListener(OnResetPressed);
            trackNameField.onValueChanged.RemoveListener(OnTrackNameChanged);
        }

        private void OnTrackNameChanged(string value)
        {
            trackName = value;
            CheckSaveEnable();
        }

        private void OnResetPressed()
        {
            trackName = "";
            trackPath = null;
            trackNameField.text = trackName;
            trackPathDisplay.text = "";
            CheckSaveEnable();
        }

        private void OnSavePressed()
        {
            if (!Valid()) return;
            trackManager.AddTrack(trackPath, trackName);
            OnResetPressed();
        }

        private bool Valid()
        {
            if (string.IsNullOrWhiteSpace(trackName)) return false;
            if (string.IsNullOrWhiteSpace(trackPath)) return false;
            if (!File.Exists(trackPath)) return false;
            return true;
        }

        private void CheckSaveEnable()
        {
            saveButton.enabled = Valid();
        }

        private void OnPathPressed()
        {
            FileBrowser.ShowLoadDialog(PathDialogSuccess, PathDialogCancel, FileBrowser.PickMode.Files);
        }

        private void PathDialogCancel()
        {
        }

        private void PathDialogSuccess(string[] paths)
        {
            if (paths.Length == 0) return;
            trackPathDisplay.text = paths[0];
            trackPath = paths[0];
            trackName = Path.GetFileNameWithoutExtension(trackPath);
            trackNameField.text = trackName;
            CheckSaveEnable();
        }
    }
}