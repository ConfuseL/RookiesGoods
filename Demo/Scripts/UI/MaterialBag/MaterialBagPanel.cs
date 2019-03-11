using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialBagPanel : MonoBehaviour
{
    private UICompositeGrid[] childs;
    private void Start()
    {
        childs = GetComponentsInChildren<UICompositeGrid>();
        UpdataBagUI();
        UIDelegate.Delegate.UpdataBagUI += UpdataBagUI;
    }
    void UpdataBagUI()
    {
        for (int i = 0; i < childs.Length; i++)
            childs[i].InitItemByGrid(null);
        List<RookiesGoods_Grid> items= RookiesGoods_OverallManage.GoodsManage.Try2GetPlayer(1).TryGetBag("MaterialBag").Try2GetGoodsMessage();
        if(items!=null)
        for (int i = 0; i < items.Count; i++)
        {
            childs[i].InitItemByGrid(items[i]);
        }
    }

}
