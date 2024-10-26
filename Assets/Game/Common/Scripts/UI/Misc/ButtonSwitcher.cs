using UnityEngine;
using UnityEngine.UI;

namespace Game.Common.Scripts.UI.Misc
{
    [ExecuteInEditMode]
    public class ButtonSwitcher : MonoBehaviour
	{
        //======INSPECTOR

        [SerializeField] private Button uiButton;
		[SerializeField] private Image  uiImage;
        [SerializeField] private Sprite uiSpriteOn;
        [SerializeField] private Sprite uiSpriteOff;
        [SerializeField] private bool   uiSwitchState;

        //======PROPERTIES

        public bool State { get { return uiSwitchState; } }

        //======MONOBEHAVIOUR

        private void Awake()
        {
			Switch(uiSwitchState);
        }

        private void OnValidate()
        {
            Switch(uiSwitchState);
        }

        //======BUTTONSWITCHER

        public void Switch(bool state)
		{
            uiSwitchState = state;
            if (state)
			{
				uiImage.sprite = uiSpriteOn;
			}
			else
			{
                uiImage.sprite = uiSpriteOff;                
            }
		}

        public void Switch()
        {
            Switch(!uiSwitchState);
        }
    }	
}
