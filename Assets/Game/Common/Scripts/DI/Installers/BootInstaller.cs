using UnityEngine;

using Zenject;

using Game.Common.Scripts.DI.Cores;
using Game.Common.Scripts.DI.Engines;
using Game.Common.Scripts.DI.Huds;
using Game.Common.Scripts.Services;
using Game.Common.Scripts.UI.Misc;
using UnityEngine.UI;

namespace Game.Common.Scripts.DI.Installers
{
    public class BootInstaller : MonoInstaller
    {
        [SerializeField] private BootCore     sceneCore;
        [SerializeField] private BootHud      sceneHud;
        [SerializeField] private BootEngine   sceneEngine;
        //
        [SerializeField] private Camera       sceneCamera;
        [SerializeField] private Canvas       sceneCanvas;
        [SerializeField] private CanvasScaler sceneScaler;

        public override void InstallBindings()
        {
            BindArchitecture();
            BindComponents();
        }

        public void BindArchitecture()
        {
            Container.BindInstance<BaseCore>(sceneCore).AsSingle().NonLazy();
            Container.BindInstance<BootCore>(sceneCore).AsSingle().NonLazy();

            Container.BindInstance<BaseHud>(sceneHud).AsSingle().NonLazy();
            Container.BindInstance<BootHud>(sceneHud).AsSingle().NonLazy();

            Container.BindInstance<BaseEngine>(sceneEngine).AsSingle().NonLazy();
            Container.BindInstance<BootEngine>(sceneEngine).AsSingle().NonLazy();
        }

        public void BindComponents()
        {
            Container.BindInstance<Camera>(sceneCamera).AsSingle().NonLazy();
            Container.BindInstance<Canvas>(sceneCanvas).AsSingle().NonLazy();
            Container.BindInstance<CanvasScaler>(sceneScaler).AsSingle().NonLazy();
        }
    }
}
