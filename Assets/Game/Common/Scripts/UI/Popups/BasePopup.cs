using UnityEngine;
using UnityEngine.UI;

using Game.Common.Scripts.DI.Huds;
using Game.Common.Scripts.DI.Engines;
using Game.Common.Scripts.Systems;

namespace Game.Common.Scripts.UI.Popups
{
    public class BasePopup : MonoBehaviour
    {
        //======INSPECTOR

        [SerializeField] private Button[] uiButtons;

        //======FIELDS

        protected BaseHud    sceneHud;
        protected BaseEngine sceneEngine;        

        //======PROPERTIES

        public BaseHud    Hud    { set { sceneHud = value; } }
        public BaseEngine Engine { set { sceneEngine = value; } }

        //======MONOBEHAVIOUR

        public void OnDestroy()
        {
            foreach (var button in uiButtons)
            {
                button.onClick.RemoveAllListeners();
            }
        }

        //======POPUP

        public virtual void Init()
        {
            //Taps
            foreach (var button in uiButtons)
            {
                button.onClick.AddListener(OnButtonTap);
            }
        }

        public virtual void Open()
        {
            gameObject.SetActive(true);
            sceneHud.SetButtonsInteractable(false);
        }

        public virtual void Close()
        {
            Destroy(gameObject);
            sceneHud.SetButtonsInteractable(true);
        }

        //======HUD

        public void SwitchScene(string scene)
        {
            PlayEngine engine = ((PlayEngine)sceneEngine);
            engine.Defeat(false);
            engine.DestroyPlayer();            
            sceneHud.SwitchScene(scene);
        }

        //======EVENTS

        public void OnButtonTap()
        {
            SoundSystem.Instance.PlaySound("tap");
        }
    }
}
