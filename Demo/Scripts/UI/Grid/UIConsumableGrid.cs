using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIConsumableGrid : MonoBehaviour
{
    private RookiesGoods_Consumable item;
    private Text num;
    private Image sprite;
    private Button button;
    private Sprite Empty;
    private void Awake()
    {
        button = GetComponent<Button>();
        num = GetComponentInChildren<Text>();
        sprite = GetComponent<Image>();
        button.onClick.AddListener(delegate { GameObject.Find("TipPanel").GetComponent<TipPanel>().UpdataData2Consumable(item); });
        Empty = sprite.sprite;
    }
    public void InitItem(RookiesGoods_Grid grid)
    {
        if (grid == null)
        {
            item = null;
            num.text = "";
            sprite.sprite = Empty;
            return;
        }
        item = (RookiesGoods_Consumable)grid.Item;
        num.text = grid.Num.ToString();
        sprite.sprite = RookiesGoods_OverallManage.GoodsManage.Try2GetSprite(item.Id);
    }
}
