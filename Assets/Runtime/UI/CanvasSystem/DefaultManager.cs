using System.Collections.Generic;
using UnityEngine;

namespace UnlimitedKaraoke.Runtime.UI.CanvasSystem
{
    public class DefaultManager : IManager
    {
        private Dictionary<CanvasSection, ISectionDisplay> displays = new();

        private ISectionDisplay currentDisplay;
        
        public void ShowSection(CanvasSection sectionToShow)
        {
            if (!displays.TryGetValue(sectionToShow, out var display))
            {
                Debug.LogError($"could not find display for: {sectionToShow.Name}");
                return;
            }
            
            if (currentDisplay != null) currentDisplay.Hide();
            currentDisplay = display;
            currentDisplay.Show();
        }

        public void RegisterDisplay(ISectionDisplay display)
        {
            if (displays.ContainsKey(display.Section))
            {
                Debug.LogWarning($"Already contains a display for section: {display.Section.Name}");
            }
            displays.Add(display.Section, display);
        }
    }
}