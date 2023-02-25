using UnityEngine;
using UnityEngine.EventSystems;

namespace UnlimitedKaraoke.Runtime.UI.CanvasSystem
{
    public class ShowCanvas : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private CanvasSection sectionToShow;

        private IManager canvasManager;

        [Zenject.Inject]
        public void InjectDependencies(IManager canvasManager)
        {
            this.canvasManager = canvasManager;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            canvasManager.ShowSection(sectionToShow);
        }
    }
}