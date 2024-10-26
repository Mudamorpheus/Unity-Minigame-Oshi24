using UnityEngine;

using TMPro;
using Zenject;

using Game.Common.Scripts.DI.Huds;
using Game.Common.Scripts.UI.Items;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms.Impl;

namespace Game.Common.Scripts.UI.Leaderboards
{
    public class ScoreLeaderboard : MonoBehaviour
    {
        //======TYPES

        class LeaderboardPosition
        {
            public string Name;
            public int    Score;
            public bool   Player;

            public LeaderboardPosition(string name, int score, bool player)
            {
                Name   = name;
                Score  = score;
                Player = player;
            }
        }

        //======INSPECTOR

        [SerializeField] private GameObject uiItemsList;
        [SerializeField] private ScoreItem  uiItemPrefab;
        [SerializeField] private Color      uiItemColorActive;

        //======INJECTS

        private BaseHud                   sceneHud;
        //
        private List<LeaderboardPosition> positionsList = new List<LeaderboardPosition>();

        //======SCORE

        public void Init(BaseHud hud)
        {
            sceneHud = hud;

            AddPosition("GMadX",      3210, false);
            AddPosition("irispowwwe", 3090, false);
            AddPosition("mewingguy",  3010, false);
            AddPosition(sceneHud.Account.Data.Name, sceneHud.Account.Data.Score, true);

            positionsList.Sort((x, y) => y.Score - x.Score);
            ShowLeaderboard();
        }

        public void AddPosition(string name, int score, bool player)
        {
            LeaderboardPosition position;

            if (name == "")
            {
                position = new LeaderboardPosition("Unknown player", score, player);
            }
            else
            {
                position = new LeaderboardPosition(name, score, player);
            }
            
            positionsList.Add(position);
        }

        public void ShowLeaderboard()
        {
            for(int i = 0; i < positionsList.Count; i++)
            {
                var position = positionsList[i];
                
                //First
                if(i == 0)
                {
                    var item = Instantiate(uiItemPrefab, uiItemsList.transform);
                    string order = (i+1).ToString()+".";
                    string name = order + position.Name;
                    string score = position.Score.ToString();
                    item.SetColor(uiItemColorActive);
                    item.SetText(name, score);
                }
                //Last
                else if(i == positionsList.Count-1)
                {
                    var space = Instantiate(uiItemPrefab, uiItemsList.transform);
                    space.SetText("........", "");
                    //
                    var item  = Instantiate(uiItemPrefab, uiItemsList.transform);
                    string order = (5000-position.Score*1.5f).ToString()+".";
                    string name  = order + position.Name;
                    string score = position.Score.ToString();
                    item.SetText(name, score);
                }
                //Default
                else
                {
                    var item = Instantiate(uiItemPrefab, uiItemsList.transform);
                    string order = (i+1).ToString() + ".";
                    string name = order + position.Name;
                    string score = position.Score.ToString();
                    item.SetText(name, score);
                }
            }
        }
    }
}