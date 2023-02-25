namespace UnlimitedKaraoke.Runtime.UI.CanvasSystem
{
    public interface ISectionDisplay
    {
        CanvasSection Section { get; }

        public void Show();
        public void Hide();
    }
}