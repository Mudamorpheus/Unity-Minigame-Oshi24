using Game.Common.Scripts.DI.Huds;

namespace Game.Common.Scripts.UI.Items
{
    public class SkinItem : BaseItem
    {
        //======SKIN

        public void Buy()
        {
            ((MenuHud)sceneHud).BuySkin(itemData.Id, itemData.Price);
        }

        public void Select()
        {
            ((MenuHud)sceneHud).SelectSkin(itemData.Id);
        }
    }
}

