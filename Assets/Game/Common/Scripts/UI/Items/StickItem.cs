using Game.Common.Scripts.DI.Huds;

namespace Game.Common.Scripts.UI.Items
{
    public class StickItem : BaseItem
    {
        //======SKIN

        public void Buy()
        {
            ((MenuHud)sceneHud).BuyStick(itemData.Id, itemData.Price);
        }

        public void Select()
        {
            ((MenuHud)sceneHud).SelectStick(itemData.Id);
        }
    }
}

