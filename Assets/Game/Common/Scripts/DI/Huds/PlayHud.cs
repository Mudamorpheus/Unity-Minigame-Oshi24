using UnityEngine;

using Zenject;
using TMPro;
using Game.Common.Scripts.DI.Engines;
using Game.Common.Scripts.Games.Controllers;

namespace Game.Common.Scripts.DI.Huds
{
    public class PlayHud : BaseHud
    {
        //======INSPECTOR

        [SerializeField] private TMP_Text[]      uiCurrentScoreTexts;
        [SerializeField] private TMP_Text[]      uiStreakTexts;
        [SerializeField] private TapController[] uiTapControllers;

        //======INJECTS

        [Inject] private new PlayEngine sceneEngine;

        //======PLAY

        public override void Init()
        {
            base.Init();
        }

        public override void Disable()
        {
            base.Disable();
            
            foreach(var controller in uiTapControllers)
            {
                if (controller) { controller.enabled = false; }
            }
        }

        public override void Enable()
        {
            base.Enable();

            foreach(var controller in uiTapControllers)
            {
                if (controller) { controller.enabled = true; }
            }
        }

        //======TEXT

        public void UpdateCurrentScoreTexts()
        {
            foreach(var text in uiCurrentScoreTexts)
            {
                text.text = "Score:"  + sceneEngine.Score.ToString();
            }
        }

        public void UpdateStreakTexts()
        {
            int streak = sceneEngine.Streak;
            foreach (var text in uiStreakTexts)
            {
                if(streak > 1)
                {
                    text.text = "Streak:" + streak.ToString();
                }                
            }
        }

        //======ACTIONS

        public void AddBalance(int balance)
        {
            sceneAccount.Data.Balance += balance;
            sceneAccount.Save();
        }

        public void SetBestScore(int score)
        {
            if (sceneAccount.Data.Score < score)
            {
                sceneAccount.Data.Score = score;
                sceneAccount.Save();
            }
        }

        public void CompleteTutorial()
        {
            sceneAccount.Data.Tutorial = false;
            sceneAccount.Save();
        }
    }
}

