using System.Collections.Generic;
using UnityEngine;

namespace Game.Common.Scripts.Services
{
    public class ShopService
    {
        //======SUBCLASSES

        [System.Serializable]
        public class BaseData
        {
            public string Id;
            public int Price;
            public Sprite Sprite;
        }

        [System.Serializable]
        public class SkinData : BaseData
        {
            public RuntimeAnimatorController SkinAnimator;

            public SkinData(string id, int price, Sprite sprite)
            {
                Id     = id;
                Price  = price;
                Sprite = sprite;
            }
        }

        [System.Serializable]
        public class StickData : BaseData
        {
            public Sprite StickSprite;

            public StickData(string id, int price, Sprite sprite)
            {
                Id     = id;
                Price  = price;
                Sprite = sprite;
            }
        }

        //======FIELDS

        [SerializeField] private List<SkinData>  shopSkins;
        [SerializeField] private List<StickData> shopSticks;

        //======PROPERTIES

        public List<SkinData>  Skins  { get { return shopSkins; } }
        public List<StickData> Sticks { get { return shopSticks; } }

        //======SHOP

        public void Init(List<SkinData> skins, List<StickData> sticks)
        {
            shopSkins = skins;
            shopSticks = sticks;
        }
    }
}
