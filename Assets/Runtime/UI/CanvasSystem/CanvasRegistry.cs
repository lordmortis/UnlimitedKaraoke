using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnlimitedKaraoke.Runtime.UI.CanvasSystem
{
    public class CanvasRegistry : MonoBehaviour
    {
        [SerializeField] private List<DefaultSectionDisplay> displays = new();
        [SerializeField] private CanvasSection defaultDisplay = null;

        private IManager canvasManager;

        [Zenject.Inject]
        public void InjectDependencies(IManager canvasManager)
        {
            this.canvasManager = canvasManager;
        }

        public void OnEnable()
        {
            foreach (var display in displays)
            {
                display.Hide();
                canvasManager.RegisterDisplay(display);
            }
        }

        public void Start()
        {
            if (defaultDisplay != null) canvasManager.ShowSection(defaultDisplay);
        }
    }
}