using System;
using UnityEngine;

namespace UnlimitedKaraoke.Runtime.UI.CanvasSystem
{
    [Serializable]
    public class DefaultSectionDisplay : ISectionDisplay
    {
        public CanvasSection Section => section;

        [SerializeField] private CanvasSection section;
        [SerializeField] private Canvas canvas;

        public void Show()
        {
            canvas.gameObject.SetActive(true);
        }

        public void Hide()
        {
            canvas.gameObject.SetActive(false);
        }
    }
}