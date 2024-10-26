using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Common.Scripts.UI.Items
{
    public class ScoreItem : MonoBehaviour
    {
        //======INSPECTOR

        [SerializeField] private TMP_Text uiNameText;
        [SerializeField] private TMP_Text uiScoreText;

        //======SCORE

        public void SetText(string name, string score)
        {
            uiNameText.text  = name;
            uiScoreText.text = score;
        }

        public void SetColor(Color color)
        {
            uiNameText.color  = color;
            uiScoreText.color = color;
        }
    }
}

