using UnityEngine;

using Zenject;

using Game.Common.Scripts.Services;
using Game.Common.Scripts.UI.Popups;

namespace Game.Common.Scripts.DI.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        //======INSPECTOR

        [SerializeField] private PausePopup uiPopupPausePrefab;
        [SerializeField] private DefeatPopup uiPopupDefeatPrefab;

        //======BINDINGS

        public override void InstallBindings()
        {
            BindServices();
            BindFactories();
        }

        public void BindServices()
        {
            Container.Bind<AccountService>().FromNew().AsSingle().NonLazy();
            Container.Bind<ShopService>().FromNew().AsSingle().NonLazy();
        }

        public void BindFactories()
        {
            Container.BindFactory<PausePopup, PausePopup.Factory>();
            PausePopup.Factory.BindPrefab(uiPopupPausePrefab);
            Container.BindFactory<DefeatPopup, DefeatPopup.Factory>();
            DefeatPopup.Factory.BindPrefab(uiPopupDefeatPrefab);
        }
    }
}