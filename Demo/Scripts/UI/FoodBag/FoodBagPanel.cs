using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBagPanel : MonoBehaviour
{
    private UIConsumableGrid[] childs;
    private void Start()
    {
        childs = GetComponentsInChildren<UIConsumableGrid>();
        UpdataBagUI();
        UIDelegate.Delegate.UpdataBagUI += UpdataBagUI;
    }
    void UpdataBagUI()
    {
        for (int i = 0; i < childs.Length; i++)
            childs[i].InitItem(null);
        List<RookiesGoods_Grid> items = RookiesGoods_OverallManage.GoodsManage.Try2GetPlayer(1).TryGetBag("FoodBag").Try2GetGoodsMessage();
        if (items != null)
            for (int i = 0; i < items.Count; i++)
            {
                childs[i].InitItem(items[i]);
            }
    }
}
