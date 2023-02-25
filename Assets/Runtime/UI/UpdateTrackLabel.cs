using System.Collections.Generic;
using UnityEngine;

namespace UnlimitedKaraoke.Runtime.UI
{
    
    [RequireComponent(typeof(TMPro.TMP_Text))]
    public class UpdateTrackLabel : MonoBehaviour
    {
        private enum Type
        {
            TotalTracks,
            ProcessingTracks,
        }

        [SerializeField] private Type labelType;

        private TMPro.TMP_Text label;
        private Tracks.IManager trackManager;
        private int lastCount;
        
        [Zenject.Inject]
        public void InjectDependencies(Tracks.IManager trackManager)
        {
            this.trackManager = trackManager;
        }

        public void Awake()
        {
            label = GetComponent<TMPro.TMP_Text>();
        }

        public void OnEnable()
        {
            switch (labelType)
            {
                case Type.ProcessingTracks:
                    Debug.Log("Track processing not done");
                    break;
                case Type.TotalTracks:
                    trackManager.OnTracksUpdated += OnTracksUpdated;
                    OnTracksUpdated(trackManager.Tracks);
                    break;
            }
        }

        public void OnDisable()
        {
            switch (labelType)
            {
                case Type.ProcessingTracks:
                    Debug.Log("Track processing not done");
                    break;
                case Type.TotalTracks:
                    trackManager.OnTracksUpdated -= OnTracksUpdated;
                    break;
            }
        }
        
        private void OnTracksUpdated(IReadOnlyList<Tracks.ITrack> tracks)
        {
            if (tracks.Count == lastCount) return;
            lastCount = tracks.Count;
            label.text = lastCount.ToString();
        }
    }
}