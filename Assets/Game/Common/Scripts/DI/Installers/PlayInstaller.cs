using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Game.Common.Scripts.DI.Cores;
using Game.Common.Scripts.DI.Engines;
using Game.Common.Scripts.DI.Huds;

namespace Game.Common.Scripts.DI.Installers
{
    public class PlayInstaller : MonoInstaller
    {
        //======INSPECTOR

        [SerializeField] private PlayCore     sceneCore;
        [SerializeField] private PlayHud      sceneHud;
        [SerializeField] private PlayEngine   sceneEngine;
        //
        [SerializeField] private Camera       sceneCamera;
        [SerializeField] private Canvas       sceneCanvas;
        [SerializeField] private CanvasScaler sceneScaler;        
        [SerializeField] private PlayerEntity scenePlayer;

        //======BINDINGS

        public override void InstallBindings()
        {
            BindArchitecture();
            BindComponents();
        }

        public void BindArchitecture()
        {
            Container.BindInstance<BaseCore>(sceneCore).AsSingle().NonLazy();
            Container.BindInstance<PlayCore>(sceneCore).AsSingle().NonLazy();

            Container.BindInstance<BaseHud>(sceneHud).AsSingle().NonLazy();
            Container.BindInstance<PlayHud>(sceneHud).AsSingle().NonLazy();

            Container.BindInstance<BaseEngine>(sceneEngine).AsSingle().NonLazy();
            Container.BindInstance<PlayEngine>(sceneEngine).AsSingle().NonLazy();
        }

        public void BindComponents()
        {
            Container.BindInstance<Camera>(sceneCamera).AsSingle().NonLazy();
            Container.BindInstance<Canvas>(sceneCanvas).AsSingle().NonLazy();
            Container.BindInstance<CanvasScaler>(sceneScaler).AsSingle().NonLazy();
            Container.BindInstance<PlayerEntity>(scenePlayer).AsSingle().NonLazy();
        }
    }
}
