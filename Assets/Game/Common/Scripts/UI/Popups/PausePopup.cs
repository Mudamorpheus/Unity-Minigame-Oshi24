using UnityEngine;

using Zenject;

using Game.Common.Scripts.DI.Engines;
using Game.Common.Scripts.DI.Huds;
using Game.Common.Scripts.UI.Misc;

namespace Game.Common.Scripts.UI.Popups
{
    public class PausePopup : BasePopup
    {
        //======FACTORY

        public class Factory : PlaceholderFactory<PausePopup>
        {
            //Prefab
            private static PausePopup factoryPrefab;
            public static void BindPrefab(PausePopup factoryPrefab)
            {
                Factory.factoryPrefab = factoryPrefab;
            }

            //Init
            public PausePopup Create(BaseHud hud, BaseEngine engine)
            {
                var popup = Instantiate(factoryPrefab, hud.Canvas.transform);
                popup.Hud = hud;
                popup.Engine = engine;

                popup.Init();
                popup.Open();

                return popup;
            }
        }

        //======BASE

        public override void Open()
        {
            base.Open();
            sceneHud.Disable();
            sceneEngine.Pause();
            sceneHud.SetButtonsInteractable(false);
        }
        public override void Close()
        {
            base.Close();
            sceneHud.Enable();
            sceneEngine.Resume();
            sceneHud.SetButtonsInteractable(true);
        }
    }
}