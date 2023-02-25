using UnityEngine;

namespace UnlimitedKaraoke.Runtime.Tracks
{
    [AddComponentMenu("Unlimited Karaoke/Tracks Installer")]
    public class Installer : Zenject.MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<DefaultManager>().AsSingle().Lazy();
        }
    }
}