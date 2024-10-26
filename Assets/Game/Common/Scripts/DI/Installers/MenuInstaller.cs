using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Game.Common.Scripts.DI.Cores;
using Game.Common.Scripts.DI.Engines;
using Game.Common.Scripts.DI.Huds;

namespace Game.Common.Scripts.DI.Installers
{
    public class MenuInstaller : MonoInstaller
    {
        //======INSPECTOR

        [SerializeField] private MenuCore     sceneCore;
        [SerializeField] private MenuHud      sceneHud;
        [SerializeField] private MenuEngine   sceneEngine;
        //
        [SerializeField] private Camera       sceneCamera;
        [SerializeField] private Canvas       sceneCanvas;
        [SerializeField] private CanvasScaler sceneScaler;

        //======BINDINGS

        public override void InstallBindings()
        {
            BindArchitecture();
            BindComponents();
        }

        public void BindArchitecture()
        {
            Container.BindInstance<BaseCore>(sceneCore).AsSingle().NonLazy();
            Container.BindInstance<MenuCore>(sceneCore).AsSingle().NonLazy();

            Container.BindInstance<BaseHud>(sceneHud).AsSingle().NonLazy();
            Container.BindInstance<MenuHud>(sceneHud).AsSingle().NonLazy();

            Container.BindInstance<BaseEngine>(sceneEngine).AsSingle().NonLazy();
            Container.BindInstance<MenuEngine>(sceneEngine).AsSingle().NonLazy();
        }

        public void BindComponents()
        {
            Container.BindInstance<Camera>(sceneCamera).AsSingle().NonLazy();
            Container.BindInstance<Canvas>(sceneCanvas).AsSingle().NonLazy();
            Container.BindInstance<CanvasScaler>(sceneScaler).AsSingle().NonLazy();
        }    
    }
}
