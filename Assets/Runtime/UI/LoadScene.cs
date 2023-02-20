using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace UnlimitedKaraoke.Runtime.UI
{
    public class LoadScene : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private SceneReference scene;

        public void OnPointerClick(PointerEventData eventData)
        {
            SceneManager.LoadScene(scene.Name);
        }
    }
}