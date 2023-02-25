using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnlimitedKaraoke.Runtime.UI.Config
{
    public class DataDirectoryButton : MonoBehaviour, IPointerClickHandler
    {
        private Settings.IManager settings;
        
        [Zenject.Inject]
        public void InjectDependencies(Settings.IManager settings)
        {
            this.settings = settings;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            FileBrowser.ShowLoadDialog(
                DialogSuccess, DialogCancel, 
                FileBrowser.PickMode.Folders);
        }

        private void DialogCancel()
        {
        }

        private void DialogSuccess(string[] paths)
        {
            if (paths.Length == 0) return;
            settings.DataDirectory = paths[0];
        }
    }
}