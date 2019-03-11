using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComposeTablePanel : MonoBehaviour
{
    private UICompositeGrid childs;
    private Text needList;
    RookiesGoods_Composite CurGoods;
    Dropdown CanCompositeList;
    private void Start()
    {
        CanCompositeList= transform.Find("CanCompositeList").GetComponent<Dropdown>();
        needList = transform.Find("NeedList").GetComponent<Text>();
        childs = GetComponentInChildren<UICompositeGrid>();
        List<int> temp = RookiesGoods_OverallManage.GoodsManage.GetCompositeID();
        for (int i= 0; i < temp.Count; i++)
        {
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = temp[i].ToString();
            CanCompositeList.options.Add(data);
        }
        CurGoods = (RookiesGoods_Composite)RookiesGoods_OverallManage.GoodsManage.GetGoods(temp[0]);
        childs.InitItem(CurGoods);

        if (CurGoods != null)
        {
            needList.text = "需要";
            foreach (KeyValuePair<int, int> kvp in CurGoods.CompositeTable)
            {
                needList.text += RookiesGoods_OverallManage.GoodsManage.GetGoods(kvp.Key).Name + " " + kvp.Value + "个、";
            }
        }
        CanCompositeList.captionText.text = temp[0].ToString();
    }
    public void DoIt()
    {
        RookiesGoods_OverallManage.GoodsManage.Try2GetPlayer(1).AutoTrying2Synthesize(CurGoods.Id);
        UIDelegate.Delegate.UpdataBagUI();
    }
    public void Drop_select()
    {
        CurGoods = (RookiesGoods_Composite)RookiesGoods_OverallManage.GoodsManage.GetGoods(int.Parse(CanCompositeList.captionText.text));
        childs.InitItem(CurGoods);

        if (CurGoods != null)
        {
            needList.text = "需要";
            foreach (KeyValuePair<int, int> kvp in CurGoods.CompositeTable)
            {
                needList.text += RookiesGoods_OverallManage.GoodsManage.GetGoods(kvp.Key).Name + " " + kvp.Value + "个、";
            }
        }
    }
}
