using UnityEngine;
using UnityEngine.EventSystems;

namespace UnlimitedKaraoke.Runtime.UI
{
    public class PlayAudio : MonoBehaviour, IPointerClickHandler
    {
        private Audio.IAudioSystem audioSystem;

        [Zenject.Inject]
        public void InjectDependencies(Audio.IAudioSystem audioSystem)
        {
            this.audioSystem = audioSystem;
        }        
        
        public void OnPointerClick(PointerEventData eventData)
        {
            audioSystem.Play();
        }
    }
}