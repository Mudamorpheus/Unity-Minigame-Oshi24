using UnityEngine;

using DG.Tweening;

using Game.Common.Scripts.Systems;
using Game.Common.Scripts.DI.Static;
using Game.Common.Scripts.DI.Huds;

namespace Game.Common.Scripts.DI.Cores
{
    public class StickEntity : BaseEntity
    {
        //======INSPECTOR

        [SerializeField] private float stickScaleMax = 5f;

        //======FIELDS

        private BaseHud sceneHud;

        //======PROPERTIES

        public float ScaleMax { get { return stickScaleMax; } set { stickScaleMax = value; } }

        //======STICK        

        public void Init(BaseHud hud)
        {
            sceneHud = hud;
            stickScaleMax *= hud.Camera.orthographicSize/8f;
        }

        public void Resize()
        {
            gameObject.transform.DOScaleY(stickScaleMax, StaticData.StickGrowDuration).SetEase(Ease.Linear);
        }

        public void Fall()
        {
            Vector3 rotation = new Vector3(0, 0, -90);
            gameObject.transform.
                DORotate(rotation, StaticData.StickFallDuration).
                SetEase(Ease.Linear).
                OnComplete(delegate { SoundSystem.Instance.PlaySound("stick"); });
        }

        public void Break()
        {
            gameObject.transform.DOKill();
        }        
    }
}