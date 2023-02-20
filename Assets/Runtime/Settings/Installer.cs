using UnityEngine;

namespace UnlimitedKaraoke.Runtime.Settings
{
    [AddComponentMenu("Unlimited Karaoke/Settings Installer")]
    public class Installer : Zenject.MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<DefaultManager>().AsSingle().Lazy();
        }
    }
}