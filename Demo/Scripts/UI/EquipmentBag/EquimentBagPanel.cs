using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquimentBagPanel : MonoBehaviour
{
    private UISuitBaseGrid[] childs;
    private void Start()
    {
        childs = GetComponentsInChildren<UISuitBaseGrid>();
        UpdataBagUI();
        UIDelegate.Delegate.UpdataBagUI += UpdataBagUI;
    }
    void UpdataBagUI()
    {
        for (int i = 0; i < childs.Length; i++)
            childs[i].InitItem(null);
        List<RookiesGoods_Grid> items = RookiesGoods_OverallManage.GoodsManage.Try2GetPlayer(1).TryGetBag("EquipmentBag").Try2GetGoodsMessage();
        if (items != null)
            for (int i = 0; i < items.Count; i++)
            {
                childs[i].InitItem(items[i]);
            }
    }
}
