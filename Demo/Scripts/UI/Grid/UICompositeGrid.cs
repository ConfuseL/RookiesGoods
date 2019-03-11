using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICompositeGrid : MonoBehaviour
{
    private RookiesGoods_Composite item;
    private Text num;
    private Image sprite;
    private Button button;
    private Sprite Empty;
    private void Awake()
    {
        button = GetComponent<Button>();
        num = GetComponentInChildren<Text>();
        sprite = GetComponent<Image>();
        button.onClick.AddListener(delegate { GameObject.Find("TipPanel").GetComponent<TipPanel>().UpdataData2composite(item); });
        Empty = sprite.sprite;
    }
    public void InitItemByGrid(RookiesGoods_Grid grid)
    {
        if (grid == null)
        {
            item = null;
            num.text = "";
            sprite.sprite = Empty;
            return;
        }
        item = (RookiesGoods_Composite)grid.Item;
        num.text = grid.Num.ToString();
        sprite.sprite = RookiesGoods_OverallManage.GoodsManage.Try2GetSprite(item.Id);
    }
    public void InitItem(RookiesGoods_Composite target)
    {
        item = target;
        sprite.sprite = RookiesGoods_OverallManage.GoodsManage.Try2GetSprite(item.Id);
    }
}
