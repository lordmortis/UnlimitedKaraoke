using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnlimitedKaraoke.Runtime.UI
{
    public class LoadAudio : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private string audioFile1;
        [SerializeField] private string audioFile2;
        
        private Audio.IAudioSystem audioSystem;

        [Zenject.Inject]
        public void InjectDependencies(Audio.IAudioSystem audioSystem)
        {
            this.audioSystem = audioSystem;
        }        
        
        public void OnPointerClick(PointerEventData eventData)
        {
            audioSystem.StartLoadingFile(Path.Join(Application.streamingAssetsPath,audioFile1), 0);
            audioSystem.StartLoadingFile(Path.Join(Application.streamingAssetsPath,audioFile2), 2);
        }
    }
}