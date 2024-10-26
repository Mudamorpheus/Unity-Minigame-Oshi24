using UnityEngine;

using DG.Tweening;
using Zenject;

using Game.Common.Scripts.DI.Huds;
using Game.Common.Scripts.DI.Engines;
using Game.Common.Scripts.DI.Static;
using Game.Common.Scripts.Services;
using Game.Common.Scripts.Systems;

namespace Game.Common.Scripts.DI.Cores
{
    public class PlayerEntity : BaseEntity
    {
        //======INSPECTOR

        [SerializeField] private GameObject  stickSpawn;
        [SerializeField] private GameObject  stickPlatform;
        [SerializeField] private StickEntity stickPrefab;

        //======INJECTS

        [Inject] private PlayHud     sceneHud;
        [Inject] private PlayEngine  sceneEngine;
        [Inject] private ShopService sceneShop;

        //======FIELDS

        private StickEntity playerStickPrev;
        private StickEntity playerStickCurrent;
        private int         playerCollisions;
        private bool        playerFalling;
        private int         playerPassed;

        //======PROPERTIES

        public StickEntity Stick { get { return playerStickCurrent; } }

        //======MONOBEHAVIOUR

        public void Awake()
        {
            //Player skin
            var accountSkinData = sceneHud.Account.Data.Shop.Skins.Find(x => x.State == AccountService.ItemState.Selected);
            var shopSkinData = sceneShop.Skins.Find(x => x.Id == accountSkinData.Id);
            entityAnimator.runtimeAnimatorController = shopSkinData.SkinAnimator;
        }

        //======PLAYER

        public void MoveX(float x)
        {
            float end = gameObject.transform.position.x + x;
            PlaySound("run", true);
            entityAnimator.Play("Walk");
            gameObject.transform.
                DOMoveX(end, StaticData.PlayerMoveDuraiton).
                SetEase(Ease.Linear).
                OnComplete(delegate 
                { 
                    entityAnimator.Play("Idle");
                    StopSound();
                });            
        }

        public void MoveY(float y)
        {
            float end = gameObject.transform.position.y + y;
            gameObject.transform.
                DOMoveY(end, StaticData.PlayerMoveDuraiton).
                SetEase(Ease.Linear);
        }

        public void Break()
        {
            gameObject.transform.DOKill();
        }

        //

        public void SpawnStick()
        {
            //Stick creation
            playerStickPrev = playerStickCurrent;
            var stick = Instantiate(stickPrefab, stickSpawn.transform);         
            playerStickCurrent = stick.GetComponent<StickEntity>();
            playerStickCurrent.Init(sceneHud);

            //Stick skin
            var accountStickData = sceneHud.Account.Data.Shop.Sticks.Find(x => x.State == AccountService.ItemState.Selected);
            var shopStickData = sceneShop.Sticks.Find(x => x.Id == accountStickData.Id);
            playerStickCurrent.Sprite.sprite = shopStickData.StickSprite;
        }

        public void StartGrow()
        {
            if(playerStickCurrent)
            {
                playerStickCurrent.Resize();
            }
        }

        public void EndGrow()
        {
            if (playerStickCurrent)
            {
                playerStickCurrent.transform.parent = stickPlatform.transform;
                playerStickCurrent.Break();
                playerStickCurrent.Fall();
                MoveX(playerStickCurrent.Height*1.25f + Width*1f);
                SpawnStick();
            }
        }

        //======GAME

        public void Fall()
        {
            //State
            playerFalling = true;

            //UI            
            sceneHud.Disable();

            //Player animation
            Break();                                    
            gameObject.transform.
                DOMoveY(-20, StaticData.PlayerMoveDuraiton).
                SetEase(Ease.Linear).
                OnComplete(delegate 
                { 
                    Defeat(); 
                });

            //Stick animation
            if(playerStickPrev)
            {
                playerStickPrev.gameObject.transform.
                    DOMoveY(-20, StaticData.PlayerMoveDuraiton).
                    SetEase(Ease.Linear);
                playerStickPrev.gameObject.transform.
                    DORotate(new Vector3(0, 0, 180), StaticData.PlayerMoveDuraiton).
                    SetEase(Ease.Linear);
            }

            //Stick animation
            playerStickCurrent.gameObject.transform.
                DOMoveY(-20, StaticData.PlayerMoveDuraiton).
                SetEase(Ease.Linear);
            playerStickCurrent.gameObject.transform.
                DORotate(new Vector3(0, 0, -180), StaticData.PlayerMoveDuraiton).
                SetEase(Ease.Linear);            

            //Sounds
            StopSound();
            SoundSystem.Instance.PlaySound("fall");
        }

        public void Defeat()
        {
            sceneEngine.Defeat(true);
        }

        public void Pause()
        {
            entityAnimator.speed = 0;
        }

        public void Resume()
        {
            entityAnimator.speed = 1;
        }

        //======COLLISION        

        public void OnTriggerEnter2D(Collider2D collider)
        {
            //Counter
            playerCollisions++;
        }

        public void OnTriggerExit2D(Collider2D collider)
        {            
            //Counter
            playerCollisions--;

            //Conditions
            if(!playerFalling)
            {
                if (playerCollisions == 0)
                {
                    Fall();
                }
                else
                {
                    BaseEntity entity = collider.gameObject.GetComponent<BaseEntity>();
                    if (entity)
                    {
                        if (entity as CityEntity)
                        {
                            playerPassed++;
                        }
                        if (entity as StickEntity)
                        {
                            if (playerPassed > 0)
                            {
                                playerPassed = 0;
                                sceneEngine.Reward();
                            }
                        }
                    }
                }
            }
        }
    }
}