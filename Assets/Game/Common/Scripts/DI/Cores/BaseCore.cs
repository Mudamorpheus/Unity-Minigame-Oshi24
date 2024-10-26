using UnityEngine;

using Zenject;

using Game.Common.Scripts.DI.Huds;
using Game.Common.Scripts.DI.Engines;
using Game.Common.Scripts.Services;
using DG.Tweening;
using Game.Common.Scripts.Systems;

namespace Game.Common.Scripts.DI.Cores
{
    public class BaseCore : MonoBehaviour
    {
        //======INJECTS

        [Inject] protected BaseHud        sceneHud;
        [Inject] protected BaseEngine     sceneEngine;
        [Inject] protected AccountService sceneAccount;
        [Inject] protected ShopService    sceneShop;

        //======MONOBEHAVIOUR

        public virtual void Start()
        {
            sceneHud.Init();
            sceneEngine.Init();
        }

        public void OnDestroy()
        {
            DOTween.Clear();
        }

        public void OnApplicationQuit()
        {
            DOTween.Clear();
        }
    }
}