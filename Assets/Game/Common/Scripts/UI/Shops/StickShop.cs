using System.Collections.Generic;

using Game.Common.Scripts.UI.Shops;
using Game.Common.Scripts.Services;
using Game.Common.Scripts.UI.Items;

using static Game.Common.Scripts.Services.AccountService;

public class StickShop : BaseShop
{
    //======FIELDS

    private List<StickItem> stickItems;

    //======MONOBEHAVIOUR

    public override void Awake()
    {
        base.Awake();

        stickItems = new List<StickItem>();

        foreach (var shopData in sceneShop.Sticks)
        {
            var accountData = sceneAccount.Sticks.Find(x => x.Id == shopData.Id);
            if (accountData != null)
            {
                AddStick(shopData, accountData.State);
            }
            else
            {
                AddStick(shopData, ItemState.Available);
            }
        }
    }

    //======SKIN

    public void AddStick(ShopService.StickData data, ItemState state)
    {
        var item = Instantiate(uiItemPrefab, uiItemsList.transform);
        var comp = item.GetComponent<StickItem>();
        if (comp)
        {
            comp.Init(sceneHud, sceneAccount, data);
            comp.SwitchTab(state);
            stickItems.Add(comp);
        }
    }

    public void UpdateAccountSticks()
    {
        foreach (var shopData in sceneShop.Sticks)
        {
            var accountData = sceneAccount.Sticks.Find(x => x.Id == shopData.Id);
            var itemData = stickItems.Find(x => x.Data.Id == shopData.Id);

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
