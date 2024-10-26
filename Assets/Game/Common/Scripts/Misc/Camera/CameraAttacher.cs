using UnityEngine;

namespace Game.Common.Scripts.Misc.Camera
{
    public class CameraAttacher : MonoBehaviour
    {
        //======INSPECTOR

        [SerializeField] private GameObject attachCamera;
        [SerializeField] private GameObject attachTarget;
        [SerializeField] private bool       attachX;
        [SerializeField] private bool       attachY;

        //======FIELDS

        private float diffX;
        private float diffY;

        //======MONOBEHAVIOUR

        private void Awake()
        {
            diffX = attachCamera.transform.position.x - attachTarget.transform.position.x;
            diffY = attachCamera.transform.position.y - attachTarget.transform.position.y;
        }

        private void FixedUpdate()
        {
            if(attachX || attachY)
            {
                var position = new Vector3(attachCamera.transform.position.x, attachCamera.transform.position.y, attachCamera.transform.position.z);
                if (attachX) { position.x = diffX + attachTarget.transform.position.x; }
                if (attachY) { position.y = diffY + attachTarget.transform.position.y; }
                attachCamera.transform.position = position;
            }
        }
    }
}