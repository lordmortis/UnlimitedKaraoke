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
        private Moises.IManager moises;

        [Zenject.Inject]
        public void InjectDependencies(Tracks.IManager trackManager, Moises.IManager moises)
        {
            this.trackManager = trackManager;
            this.moises = moises;
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
                    moises.OnJobsUpdate += OnCountUpdated;
                    OnCountUpdated(moises.Jobs);
                    break;
                case Type.TotalTracks:
                    trackManager.OnTracksUpdated += OnCountUpdated;
                    OnCountUpdated(trackManager.Tracks);
                    break;
            }
        }

        public void OnDisable()
        {
            switch (labelType)
            {
                case Type.ProcessingTracks:
                    moises.OnJobsUpdate -= OnCountUpdated;
                    break;
                case Type.TotalTracks:
                    trackManager.OnTracksUpdated -= OnCountUpdated;
                    break;
            }
        }
        
        private void OnCountUpdated<T>(IReadOnlyList<T> tracks) 
        {
            if (tracks.Count == lastCount) return;
            lastCount = tracks.Count;
            label.text = lastCount.ToString();
        }
    }
}