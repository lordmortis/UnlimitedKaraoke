using System.Collections.Generic;
using UnityEngine;

namespace UnlimitedKaraoke.Runtime.UI.Tracks
{
    public class TrackListController : MonoBehaviour
    {
        [SerializeField] private TrackDisplay displayArchetype;
        
        private Runtime.Tracks.IManager trackManager;
        private Audio.IAudioSystem audioSystem;

        private readonly List<TrackDisplay> displays = new();
        private readonly Queue<TrackDisplay> availableDisplays = new();

        [Zenject.Inject]
        public void InjectDependencies(Runtime.Tracks.IManager trackManager, Audio.IAudioSystem audioSystem)
        {
            this.trackManager = trackManager;
            this.audioSystem = audioSystem;
        }

        public void Awake()
        {
            displayArchetype.gameObject.SetActive(false);
        }

        public void OnEnable()
        {
            trackManager.OnTracksUpdated += OnTracksUpdated;
            OnTracksUpdated(trackManager.Tracks);
        }

        public void OnDisable()
        {
            trackManager.OnTracksUpdated -= OnTracksUpdated;
        }

        public void PlayTrack(Runtime.Tracks.ITrack track)
        {
            audioSystem.Play(trackManager.TrackDirectory(track), track);
        }
        
        private void OnTracksUpdated(IReadOnlyList<Runtime.Tracks.ITrack> tracks)
        {
            foreach (var display in displays)
            {
                display.gameObject.SetActive(false);
                availableDisplays.Enqueue(display);
            }
            
            displays.Clear();

            foreach (var track in tracks)
            {
                if (!availableDisplays.TryDequeue(out var display))
                {
                    display = Instantiate(displayArchetype, transform);
                    display.Controller = this;
                }
                display.SetTrack(track);
                display.gameObject.SetActive(true);
                displays.Add(display);
            }
        }
    }
}