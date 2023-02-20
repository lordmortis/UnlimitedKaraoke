namespace UnlimitedKaraoke.Runtime.UI.Config
{
    public class ConfigController
    {
        //TODO: really, these should be decoupled via some application-level manager...
        private Settings.IManager settings;
        
        [Zenject.Inject]
        public void InjectDependencies(Settings.IManager settings)
        {
            this.settings = settings;
        }

        public void Start()
        {
            settings.Load();
        }
    }
}