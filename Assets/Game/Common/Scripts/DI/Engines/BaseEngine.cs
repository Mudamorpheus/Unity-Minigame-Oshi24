using DG.Tweening;
using UnityEngine;

namespace Game.Common.Scripts.DI.Engines
{
    public class BaseEngine : MonoBehaviour
    {
        //======ENGINE

        public virtual void Init()
        {

        }

        public virtual void Pause()
        {
            DOTween.PauseAll();
        }

        public virtual void Resume()
        {
            DOTween.PlayAll();
        }
    }
}

