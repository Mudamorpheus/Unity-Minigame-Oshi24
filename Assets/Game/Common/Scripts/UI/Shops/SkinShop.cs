using System.Collections.Generic;

using Game.Common.Scripts.Services;
using Game.Common.Scripts.UI.Shops;
using Game.Common.Scripts.UI.Items;

using static Game.Common.Scripts.Services.AccountService;

public class SkinShop : BaseShop
{
    //======FIELDS

    private List<SkinItem> skinItems;

    //======MONOBEHAVIOUR

    public override void Awake()
    {
        base.Awake();

        skinItems = new List<SkinItem>();
        
        foreach (var shopData in sceneShop.Skins)
        {
            var accountData = sceneAccount.Skins.Find(x => x.Id == shopData.Id);
            if(accountData != null)
            {
                AddSkin(shopData, accountData.State);
            }            
            else
            {
                AddSkin(shopData, ItemState.Available);
            }
        }
    }

    //======SKIN

    public void AddSkin(ShopService.SkinData data, ItemState state)
    {
        var item = Instantiate(uiItemPrefab, uiItemsList.transform);
        var comp = item.GetComponent<SkinItem>();
        if(comp)
        {
            comp.Init(sceneHud, sceneAccount, data);
            comp.SwitchTab(state);
            skinItems.Add(comp);
        }
    }

    public void UpdateAccountSkins()
    {
        foreach(var shopData in sceneShop.Skins)
        {
            var accountData = sceneAccount.Skins.Find(x => x.Id == shopData.Id);
            var itemData    = skinItems.Find(x => x.Data.Id == shopData.Id);

            if (accountData != null)
            {
                itemData.SwitchTab(accountData.State);
            }
            else
            {
                itemData.SwitchTab(ItemState.Available);
            }
        }    
    }
}
