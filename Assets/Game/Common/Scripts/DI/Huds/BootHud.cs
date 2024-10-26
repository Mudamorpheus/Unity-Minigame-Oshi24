using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

using Game.Common.Scripts.DI.Static;

namespace Game.Common.Scripts.DI.Huds
{
    public class BootHud : BaseHud
    {
        //======INSPECTOR

        [SerializeField] private Image uiAndroidSplash;
        [SerializeField] private Image uiIosSplash;

        //======BOOT

        public override void Init()
        {
            base.Init();

            SelectPlatformSplash();
        }

        public void SelectPlatformSplash()
        {
#if UNITY_ANDROID
            ShowSplash(uiAndroidSplash);
#elif UNITY_IOS
            ShowSplash(uiIosSplash);
#else
            EndSplash();
#endif
        }

        public void ShowSplash(Image splash)
        {
            splash.gameObject.SetActive(true);
            Sequence sequence = DOTween.Sequence();
            sequence.Append(splash.DOFade(1, StaticData.SplashDuration));
            sequence.Append(splash.DOFade(0, StaticData.SplashDuration));
            sequence.OnComplete(delegate { EndSplash(); });
        }

        public void EndSplash()
        {
            SwitchScene("Menu");
        }
    }
}

