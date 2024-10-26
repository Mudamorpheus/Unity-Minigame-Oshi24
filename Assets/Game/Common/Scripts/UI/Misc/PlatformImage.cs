using UnityEngine;
using UnityEngine.UI;

namespace Game.Common.Scripts.UI.Misc
{
    public class PlatformImage : MonoBehaviour
    {
        //======INSPECTOR

        [SerializeField] private Image  uiPlatformImage;
        [SerializeField] private Sprite uiDefaultSprite;
        [SerializeField] private Sprite uiAndroidSprite;
        [SerializeField] private Sprite uiIosSprite;

        //======MONOBEHAVIOUR

        private void Awake()
        {
            Switch();
        }

        private void OnValidate()
        {
            Switch();
        }

        //======PLATFORM

        public void Switch()
        {
            #if UNITY_ANDROID
                uiPlatformImage.sprite = uiAndroidSprite;
            #elif UNITY_IOS
                uiPlatformImage.sprite = uiIosSprite;
            #else
                uiPlatformImage.sprite = uiDefaultSprite;
            #endif
        }
    }
}
