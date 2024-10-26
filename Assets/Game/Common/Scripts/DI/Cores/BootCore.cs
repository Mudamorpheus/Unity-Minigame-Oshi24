using System.Collections.Generic;

using UnityEngine;

using Game.Common.Scripts.Services;
using Game.Common.Scripts.Systems;

namespace Game.Common.Scripts.DI.Cores
{
    public class BootCore : BaseCore
    {
        //======INSPECTOR

        [SerializeField] private List<ShopService.SkinData>  shopSkins;
        [SerializeField] private List<ShopService.StickData> shopSticks;

        //======BOOT

        public override void Start()
        {
            base.Start();

            //Account
            sceneAccount.Init();
            sceneAccount.Load();
            sceneAccount.Save();

            //Shop
            sceneShop.Init(shopSkins, shopSticks);

            //Bg
            MusicSystem.Instance.Init("bg");

            //Audio
            SoundSystem.Instance.SwitchSound(sceneAccount.Data.Sound);
            MusicSystem.Instance.SwitchMusic(sceneAccount.Data.Music);
        }


    }
}
