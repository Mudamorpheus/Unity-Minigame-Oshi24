using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using Zenject;

using Game.Common.Scripts.DI.Cores;
using Game.Common.Scripts.DI.Engines;
using Game.Common.Scripts.DI.Static;

namespace Game.Common.Scripts.Games.Controllers
{
    public class TapController : MonoBehaviour
    {
        //======INJECTS

        [Inject] private PlayEngine sceneEngine;

        //======FIELDS

        private bool  tapState = false;
        private float tapTimer;

        //======MONOBEHAVIOUR

        private void Awake()
        {
            tapTimer = StaticData.PlayerTapCooldown;
        }

        private void Update()
        {
            //Cooldown
            if(tapTimer < StaticData.PlayerTapCooldown)
            {
                tapTimer += Time.deltaTime;
                return;
            }

            //Tap
            if (Input.touchCount > 0 && Input.touchCount <= 1)
            {
                foreach (Touch touch in Input.touches)
                {
                    //Tap begin
                    if (!tapState && touch.phase == TouchPhase.Began && !IsPointerOverUIObject(touch))
                    {
                        tapState = true;
                        OnTapBegin();
                    }

                    //Tap end
                    if (tapState && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
					{
                        tapTimer = 0f;
                        tapState = false;                        
                        OnTapEnd();
                    }
                }
            }
        }

        //======UI

        private bool IsPointerOverUIObject(Touch t)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(t.position.x, t.position.y);

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        //======TAP

        [SerializeField]
        private UnityEvent<Object> tapEventBegin;
        private void OnTapBegin()
        {
            tapEventBegin?.Invoke(this);
        }

        [SerializeField]
        private UnityEvent<Object> tapEventEnd;
        private void OnTapEnd()
        {
            tapEventEnd?.Invoke(this);
        }
    }
}