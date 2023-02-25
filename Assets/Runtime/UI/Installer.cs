using UnityEngine;

namespace UnlimitedKaraoke.Runtime.UI
{
    [AddComponentMenu("Unlimited Karaoke/UI Installer")]
    public class Installer : Zenject.MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<CanvasSystem.DefaultManager>().AsSingle().Lazy();
        }
    }
}