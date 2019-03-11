using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum CurGoodsType:int
{
    SuitBase,
    Composite,
    Consumable,
    Null
}

public class TipPanel : MonoBehaviour
{
    private Text intro;
    private Text effect;
    private Text theName;
    private Text dur;
    private Button sueOrWear;
    private Button delDurability;
    private Text useOrWear;
    private CanvasGroup canvasGroup;
    RookiesGoods_GoodsBase CurGoods { get; set; }
    CurGoodsType goodsType=CurGoodsType.Null;
    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            switch (child.gameObject.name)
            {
                case "intro":
                    intro = child.GetComponent<Text>();
                    break;
                case "effect":
                    effect = child.GetComponent<Text>();
                    break;
                case "dur":
                    dur = child.GetComponent<Text>();
                    break;
                case "name":
                    theName = child.GetComponent<Text>();
                    break;
                case "Button":
                    sueOrWear= child.GetComponent<Button>();
                    useOrWear = child.GetComponentInChildren<Text>();
                    break;
                case "DelDurability":
                    delDurability = child.GetComponent<Button>();
                    break;
            }
        }
        delDurability.gameObject.SetActive(false);
        sueOrWear.gameObject.SetActive(false);
    }
    public void UpdataData2Suit(RookiesGoods_SuitBase target)
    {
        if (target == null)
            return;
        intro.text = "介绍:"+target.Intro;
        effect.text = "作用:"+target.Effect;
        theName.text = "名字:"+target.Name;
        dur.text = "耐久度：" + target.Durability;
        if (target.WearerID == 0)
            useOrWear.text = "装备";
        else
            useOrWear.text = "卸下";
        transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        sueOrWear.gameObject.SetActive(true);
        goodsType = CurGoodsType.SuitBase;
        CurGoods = target;
        delDurability.gameObject.SetActive(true);
    }

    public void UpdataData2composite(RookiesGoods_Composite target)
    {
        if (target == null)
            return;
        intro.text = "介绍:" + target.Intro;
        effect.text = "作用:" + target.Effect;
        theName.text = "名字:" + target.Name;
        dur.text = "" ;
        useOrWear.text = "";
        transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        sueOrWear.gameObject.SetActive(false);
        delDurability.gameObject.SetActive(false);
        goodsType = CurGoodsType.Composite;
        CurGoods = target;
    }

    public void UpdataData2Consumable(RookiesGoods_Consumable target)
    {
        if (target == null)
            return;
        intro.text = "介绍:" + target.Intro;
        effect.text = "作用:" + target.Effect;
        theName.text = "名字:" + target.Name;
        dur.text = "";
        useOrWear.text = "使用";
        transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        sueOrWear.gameObject.SetActive(true);
        delDurability.gameObject.SetActive(false);
        goodsType = CurGoodsType.Consumable;
        CurGoods = target;
    }
    public void DelDurability()
    {
        RookiesGoods_SuitBase target = ((RookiesGoods_SuitBase)CurGoods);
        target.BeUsed();
        intro.text = "介绍:" + target.Intro;
        effect.text = "作用:" + target.Effect;
        theName.text = "名字:" + target.Name;
        dur.text = "耐久度：" + target.Durability;
        if (target.WearerID == 0)
            useOrWear.text = "装备";
        else
            useOrWear.text = "卸下";
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        sueOrWear.gameObject.SetActive(true);
        goodsType = CurGoodsType.SuitBase;
        CurGoods = target;
        delDurability.gameObject.SetActive(true);
    }
    public void Use()
    {
        switch (goodsType)
        {
            case CurGoodsType.SuitBase:
                if((((RookiesGoods_SuitBase) CurGoods).WearerID)==0)
                    ((RookiesGoods_SuitBase)CurGoods).SuitUp(RookiesGoods_OverallManage.GoodsManage.Try2GetPlayer(1));
                else
                    ((RookiesGoods_SuitBase)CurGoods).TakeOff();
                break;
            case CurGoodsType.Composite:
                break;
            case CurGoodsType.Consumable:
                ((RookiesGoods_Consumable)CurGoods).Used(RookiesGoods_OverallManage.GoodsManage.Try2GetPlayer(1));
                break;
            case CurGoodsType.Null:

                break;
            default:

                break;
        }
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.currentSelectedGameObject||(EventSystem.current.currentSelectedGameObject&& EventSystem.current.currentSelectedGameObject.name=="TipPanel"))
            {
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
    }
}
