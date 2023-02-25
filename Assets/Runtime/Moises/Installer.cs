using UnityEngine;

namespace UnlimitedKaraoke.Runtime.Moises
{
    [AddComponentMenu("Unlimited Karaoke/Moises Installer")]
    public class Installer : Zenject.MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<DefaultManager>().AsSingle().NonLazy();
        }
    }
}