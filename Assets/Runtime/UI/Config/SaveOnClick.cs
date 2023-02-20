using UnityEngine;
using UnityEngine.EventSystems;

namespace UnlimitedKaraoke.Runtime.UI.Config
{
    public class SaveOnClick : MonoBehaviour, IPointerClickHandler
    {
        private Settings.IManager settings;
        
        [Zenject.Inject]
        public void InjectDependencies(Settings.IManager settings)
        {
            this.settings = settings;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            settings.Save();
        }
    }
}