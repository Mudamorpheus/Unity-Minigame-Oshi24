using UnityEngine;

using Zenject;

using Game.Common.Scripts.DI.Engines;
using Game.Common.Scripts.DI.Huds;
using Game.Common.Scripts.UI.Misc;
using TMPro;
using Game.Common.Scripts.UI.Leaderboards;

namespace Game.Common.Scripts.UI.Popups
{
    public class DefeatPopup : BasePopup
    {
        //======INSPECTOR

        [SerializeField] private TMP_Text         uiCurrentScoreText;
        [SerializeField] private TMP_Text         uiBestScoreText;
        [SerializeField] private ScoreLeaderboard uiScoreLeaderboard;

        //======FACTORY

        public class Factory : PlaceholderFactory<DefeatPopup>
        {
            //Prefab
            private static DefeatPopup factoryPrefab;
            public static void BindPrefab(DefeatPopup factoryPrefab)
            {
                Factory.factoryPrefab = factoryPrefab;
            }

            //Init
            public DefeatPopup Create(BaseHud hud, BaseEngine engine)
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
        }
        public override void Close()
        {
            base.Close();
            sceneHud.Enable();
            sceneEngine.Resume();
        }

        //======DEFEAT

        public override void Init()
        {
            base.Init();

            uiCurrentScoreText.text = "YOUR SCORE: " + ((PlayEngine)sceneEngine).Score.ToString();
            uiBestScoreText.text = "BEST SCORE: " + sceneHud.Account.Data.Score.ToString();
            uiScoreLeaderboard.Init(sceneHud);
        }
    }
}