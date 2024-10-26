using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Game.Common.Scripts.Services;
using Game.Common.Scripts.DI.Huds;

namespace Game.Common.Scripts.UI.Items
{
    public class BaseItem : MonoBehaviour
    {
        //======INSPECTOR
        
        [SerializeField] protected TMP_Text uiItemTitle;
        [SerializeField] protected TMP_Text uiItemPrice;
        [SerializeField] protected Image    uiItemImage;
        //
        [SerializeField] private GameObject tabAvailable;
        [SerializeField] private GameObject tabUnselected;
        [SerializeField] private GameObject tabSelected;
        [SerializeField] private Button     buttonBuy;
        [SerializeField] private Button     buttonSelect;

        //======FIELDS

        protected BaseHud                  sceneHud;
        protected AccountService           sceneAccount;
        protected AccountService.ItemState itemState;
        protected ShopService.BaseData     itemData;

        //======PROPERTIES

        public ShopService.BaseData Data { get { return itemData; } }

        //======ITEM

        public void Init(BaseHud hud, AccountService account, ShopService.BaseData data)
        {
            sceneHud           = hud;
            sceneAccount       = account;
            itemData           = data;            
            uiItemTitle.text   = data.Id;
            uiItemPrice.text   = data.Price.ToString();
            uiItemImage.sprite = data.Sprite;
            buttonBuy.onClick.AddListener(sceneHud.OnButtonTap);
            buttonSelect.onClick.AddListener(sceneHud.OnButtonTap);
        }

        //======TABS

        public void SwitchTab(AccountService.ItemState state)
        {
            itemState = state;
            //
            tabAvailable.SetActive(false);
            tabUnselected.SetActive(false);
            tabSelected.SetActive(false);
            //
            switch(state)
            {
                case AccountService.ItemState.Available:
                {
                    tabAvailable.SetActive(true);

                    //Price
                    if(sceneAccount.Data.Balance < itemData.Price)
                    {
                        buttonBuy.interactable = false;
                    }

                    break;
                }
                case AccountService.ItemState.Unselected:
                {
                    tabUnselected.SetActive(true);
                    break;
                }
                case AccountService.ItemState.Selected:
                {
                    tabSelected.SetActive(true);
                    break;
                }
            }
        }
    }
}

