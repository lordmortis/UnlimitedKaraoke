using UnityEngine;

namespace UnlimitedKaraoke.Runtime.Audio
{
    [AddComponentMenu("Unlimited Karaoke/Audio Installer")]
    public class Installer : Zenject.MonoInstaller
    {
        
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<FModAudioSystem>().AsSingle().Lazy();
        }
    }
}