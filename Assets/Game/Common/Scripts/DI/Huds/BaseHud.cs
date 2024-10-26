using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Zenject;

using Game.Common.Scripts.DI.Engines;
using Game.Common.Scripts.UI.Popups;
using Game.Common.Scripts.Services;
using Game.Common.Scripts.Systems;

namespace Game.Common.Scripts.DI.Huds
{
    public class BaseHud : MonoBehaviour
    {
        //======INSPECTOR

        [SerializeField] private Button[]     uiButtons;
        [SerializeField] private GameObject[] uiViews;
        [SerializeField] private GameObject   uiView;

        //======INJECTS

        [Inject] protected BaseEngine          sceneEngine;
        [Inject] protected AccountService      sceneAccount;       
        [Inject] protected PausePopup.Factory  uiPausePopupFactory;
        [Inject] protected DefeatPopup.Factory uiDefeatPopupFactory;
        [Inject] protected Camera              uiCamera;
        [Inject] protected Canvas              uiCanvas;
        [Inject] protected CanvasScaler        uiScaler;        

        //======PROPERTIES

        public AccountService Account { get { return sceneAccount; } }
        public Camera         Camera { get { return uiCamera; } }
        public Canvas         Canvas { get { return uiCanvas; } }
        public CanvasScaler   Scaler { get { return uiScaler; } }

        //======MONOBEHAVIOUR

        public void OnDestroy()
        {
            foreach (var button in uiButtons)
            {
                button.onClick.RemoveAllListeners();
            }
        }

        //======HUD

        public virtual void Init()
        {
            //Views
            if (uiView)
            {
                SwitchView(uiView);
            }

            //Taps
            foreach(var button in uiButtons)
            {
                button.onClick.AddListener(OnButtonTap);
            }
        }

        public virtual void Disable()
        {
            SetButtonsInteractable(false);
        }

        public virtual void Enable()
        {
            SetButtonsInteractable(true);
        }

        public void SetButtonsInteractable(bool state)
        {
            foreach (var button in uiButtons)
            {
                if (button) { button.interactable = state; }
            }
        }

        //======VIEWS

        public void ShowView(GameObject view)
        {
            view.transform.position = uiCanvas.transform.position;
            view.SetActive(true);
        }

        public void HideView(GameObject view)
        {
            view.SetActive(false);
        }

        public void HideViews()
        {
            foreach (var view in uiViews)
            {
                HideView(view);
            }
        }

        public void SwitchView(GameObject view)
        {
            if (uiViews.Contains(view))
            {
                HideViews();
                ShowView(view);
            }
        }

        public void SwitchScene(string scene)
        {
            SceneManager.LoadScene(scene);
        }

        //======POPUPS

        public void ShowPausePopup()
        {
            uiPausePopupFactory.Create(this, sceneEngine);
        }

        public void ShowDefeatPopup()
        {
            SoundSystem.Instance.PlaySound("game over");
            uiDefeatPopupFactory.Create(this, sceneEngine);
        }

        //======EVENTS

        public void OnButtonTap()
        {
            SoundSystem.Instance.PlaySound("tap");
        }
    }
}

