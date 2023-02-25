namespace UnlimitedKaraoke.Runtime.UI.CanvasSystem
{
    public interface IManager
    {
        void ShowSection(CanvasSection sectionToShow);
        void RegisterDisplay(ISectionDisplay display);
    }
}