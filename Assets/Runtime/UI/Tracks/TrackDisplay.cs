using UnityEngine;
using UnityEngine.UI;

namespace UnlimitedKaraoke.Runtime.UI.Tracks
{
    public class TrackDisplay : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text trackName;
        [SerializeField] private TMPro.TMP_Text status;
        [SerializeField] private Button playTrack;
        
        public Runtime.Tracks.ITrack Track { get; private set; }
        public TrackListController Controller { get; set; }

        public void Awake()
        {
            playTrack.onClick.AddListener(OnPlayTrack);
        }

        private void OnPlayTrack()
        {
            Controller.PlayTrack(Track);
        }

        public void SetTrack(Runtime.Tracks.ITrack track)
        {
            trackName.text = track.Name;
            status.text = track.State.ToString();
            playTrack.enabled = track.State == Runtime.Tracks.TrackState.Ready;
            Track = track;
        }
    }
}