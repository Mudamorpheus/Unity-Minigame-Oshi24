using UnityEngine;

namespace Game.Common.Scripts.UI.Misc
{
    public class ScreenScaler : MonoBehaviour
    {
        //======SCREENSCALER

        [SerializeField] private Camera sceneCamera;
        [SerializeField] private float sceneWidth = 20;

        private void Start()
        {
            float unitsPerPixel = sceneWidth / Screen.width;
            float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
            sceneCamera.orthographicSize = desiredHalfHeight;
        }
    }
}

