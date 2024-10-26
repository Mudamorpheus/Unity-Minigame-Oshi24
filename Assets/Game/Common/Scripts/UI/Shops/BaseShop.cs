using UnityEngine;

using Zenject;

using Game.Common.Scripts.Services;
using Game.Common.Scripts.DI.Huds;
using UnityEngine.UI;
using Game.Common.Scripts.UI.Misc;

namespace Game.Common.Scripts.UI.Shops
{
    public class BaseShop : MonoBehaviour
    {
        //======INSPECTOR

        [SerializeField] protected GameObject      uiItemsList;
        [SerializeField] protected GameObject      uiItemPrefab;        
        [SerializeField] protected GridLayoutGroup uiGrid;        

        //======INJECTS

        [Inject] protected BaseHud        sceneHud;
        [Inject] protected AccountService sceneAccount;
        [Inject] protected ShopService    sceneShop;

        //=======MONOBEHAVIOUR

        public virtual void Awake()
        {
            float screenX = Screen.width;
            float screenY = Screen.height;
            float scalerX = sceneHud.Scaler.referenceResolution.x;
            float scalerY = sceneHud.Scaler.referenceResolution.y;
            float cellX   = uiGrid.cellSize.x;
            float cellY   = sceneHud.Canvas.GetComponent<RectTransform>().rect.height * 0.3f;
            //float cellY = uiGrid.cellSize.y * Mathf.Min(screenY/scalerY, scalerY/screenY);
            uiGrid.cellSize = new Vector2(cellX, cellY);
        }
    }
}