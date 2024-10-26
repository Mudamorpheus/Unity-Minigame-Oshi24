using UnityEngine;

using Zenject;
using TMPro;

using Game.Common.Scripts.DI.Engines;
using Game.Common.Scripts.Services;
using Game.Common.Scripts.Systems;
using Game.Common.Scripts.UI.Misc;
using Game.Common.Scripts.DI.Static;

namespace Game.Common.Scripts.DI.Huds
{
    public class MenuHud : BaseHud
    {
        //======INSPECTOR

        [SerializeField] private SkinShop         uiSkinShop;
        [SerializeField] private StickShop        uiStickShop;
        [SerializeField] private TMP_Text[]       uiBalanceTexts;
        [SerializeField] private ButtonSwitcher[] uiMusicSwitchers;
        [SerializeField] private ButtonSwitcher[] uiSoundSwitchers;
        [SerializeField] private TMP_InputField[] uiNameInputs;

        //======INJECTS

        [Inject] private new MenuEngine sceneEngine;

        //======MENU

        public override void Init()
        {
            base.Init();

            UpdateBalanceTexts();
            UpdateMusicSwitchers();
            UpdateSoundSwitchers();
            UpdateNameInputs();
        }

        //======COMPS

        public void UpdateShopItems()
        {
            uiSkinShop.UpdateAccountSkins();
            uiStickShop.UpdateAccountSticks();
        }

        public void UpdateBalanceTexts()
        {
            foreach (var text in uiBalanceTexts)
            {
                text.text = sceneAccount.Data.Balance.ToString();
            }
        }

        public void UpdateMusicSwitchers()
        {
            foreach(var switcher in uiMusicSwitchers)
            {
                switcher.Switch(sceneAccount.Data.Music);
            }
        }

        public void UpdateSoundSwitchers()
        {
            foreach (var switcher in uiSoundSwitchers)
            {
                switcher.Switch(sceneAccount.Data.Sound);
            }
        }

        public void UpdateNameInputs()
        {
            foreach(var input in uiNameInputs)
            {
                input.keyboardType = TouchScreenKeyboardType.Default;                
                input.text = sceneAccount.Data.Name;                
            }
        }

        //======ACTIONS

        public void BuySkin(string id, int price)
        {
            sceneAccount.Data.Shop.AddSkin(id, AccountService.ItemState.Unselected);
            sceneAccount.Data.Balance -= price;
                        
            UpdateBalanceTexts();
            UpdateShopItems();
            sceneAccount.Save();
        }

        public void BuyStick(string id, int price)
        {
            sceneAccount.Data.Shop.AddStick(id, AccountService.ItemState.Unselected);
            sceneAccount.Data.Balance -= price;

            UpdateBalanceTexts();
            UpdateShopItems();
            sceneAccount.Save();
        }

        public void SelectSkin(string id)
        {
            foreach(var data in sceneAccount.Data.Shop.Skins)
            {
                if(data.Id == id)
                {
                    data.State = AccountService.ItemState.Selected;
                }
                else if(data.State == AccountService.ItemState.Selected)
                {
                    data.State = AccountService.ItemState.Unselected;
                }
            }
            UpdateShopItems();
            sceneAccount.Save();
        }

        public void SelectStick(string id)
        {
            foreach (var data in sceneAccount.Data.Shop.Sticks)
            {
                if (data.Id == id)
                {
                    data.State = AccountService.ItemState.Selected;
                }
                else if (data.State == AccountService.ItemState.Selected)
                {
                    data.State = AccountService.ItemState.Unselected;
                }
            }
            UpdateShopItems();
            sceneAccount.Save();
        }

        public void SwitchMusic()
        {
            sceneAccount.Data.Music = !sceneAccount.Data.Music;
            UpdateMusicSwitchers();            
            MusicSystem.Instance.SwitchMusic(sceneAccount.Data.Music);
            sceneAccount.Save();
        }

        public void SwitchSound()
        {
            sceneAccount.Data.Sound = !sceneAccount.Data.Sound;
            UpdateSoundSwitchers();            
            SoundSystem.Instance.SwitchSound(sceneAccount.Data.Sound);
            sceneAccount.Save();
        }

        public void SetName(TMP_InputField input)
        {
            if(input.text.Length < StaticData.PlayerNameMin || input.text.Length > StaticData.PlayerNameMax)
            {
                input.text = sceneAccount.Data.Name;
            }
            else
            {
                sceneAccount.Data.Name = input.text;
                sceneAccount.Save();
            }
            
        }
    }
}

