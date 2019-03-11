using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 需要开发者自己实现的管理抽象函数
/// </summary>
public class PlayerGoodsManager : RookiesGoods_PlayerData
{
    /// <summary>
    /// 一个管理类应该只对应一个玩家属性脚本
    /// </summary>
    private PlayerData playerData;
    private PlayerPropertyPanel propertyPanel;

    //用于显示是否合成成功
    private Text isSuccessful;
    private void Awake()
    {
        //初始化角色，并定制角色id
        Init(1);
        //测试：为该角色背包添加这些物品
        AutoAdd2Bag(RookiesGoods_OverallManage.GoodsManage.GetGoods(2), 12);
       AutoAdd2Bag(RookiesGoods_OverallManage.GoodsManage.GetGoods(6), 1);
        AutoAdd2Bag(RookiesGoods_OverallManage.GoodsManage.GetGoods(3), 2);
        AutoAdd2Bag(RookiesGoods_OverallManage.GoodsManage.GetGoods(4), 2);
    }

    private void Start()
    {
        //加载玩家属性脚本
        playerData = GameObject.Find("Player").GetComponent<PlayerData>();
        propertyPanel= GameObject.Find("PlayerProperty").GetComponent<PlayerPropertyPanel>();
        isSuccessful= GameObject.Find("AreYouOK").GetComponent<Text>();
        isSuccessful.text = "";
        RookiesGoods_OverallManage.GoodsManage.GetGoods(1);
        //更新UI面板的属性值
       propertyPanel.UpdataUIData(playerData.Hp, playerData.Mp, playerData.Atk, playerData.Def);
    }

   /// <summary>
   /// 合成失败的调用
   /// </summary>
   /// <param name="composite">合成目标</param>
    public override void OnComposeFailed(RookiesGoods_Composite composite,string reson)
    {
        isSuccessful.text = reson;
    }

    /// <summary>
    /// 合成成功时调用
    /// </summary>
    /// <param name="composite">合成目标</param>
    public override void OnComposeSuccessful(RookiesGoods_Composite composite)
    {
        isSuccessful.text = "合成成功";
        //自动添加背包
        AutoAdd2Bag(composite);
    }
    /// <summary>
    /// 装备某物时调用
    /// </summary>
    /// <param name="suit">装备目标</param>
    public override void OnSuitUpSomething(RookiesGoods_SuitBase suit)
    {
        //如果目标物体的游戏类型属于equipment，并且手持栏还有空余位置
        if (suit.ItemType == "equipment" && TryGetBag("HandBag").EmptyGrids > 0)
        {
            //添加至手持栏
            AutoDelByBag(suit, 1, suit.SpecialId);
            TryGetBag("HandBag").Add(suit);
            //获取json制定的自定义属性，给角色增益
            if(suit.TryGetProperty("atk")!=null)
            playerData.AddAtk(int.Parse(suit.TryGetProperty("atk").ToString()));
            if (suit.TryGetProperty("def") != null)
                playerData.AddDef(int.Parse(suit.TryGetProperty("def").ToString()));
            //更新UI面板的属性值
            propertyPanel.UpdataUIData(playerData.Hp, playerData.Mp, playerData.Atk, playerData.Def);
            //更新容器面板
            UIDelegate.Delegate.UpdataBagUI();

        }

    }
    /// <summary>
    /// 卸载某物时调用
    /// </summary>
    /// <param name="suit">装备目标</param>
    public override void OnTakeOffSuccessful(RookiesGoods_SuitBase suit,bool isOver)
    {
        //如果目标物体的游戏类型属于equipment，并且容纳装备的背包还有空余位置
        if (suit.ItemType == "equipment" && TryGetBag("EquipmentBag").EmptyGrids > 0)
        {
            //从手持HandBag删除，转入equipmentBag
            TryGetBag("HandBag").Delete(suit.Id,1,suit.SpecialId);
            //如果不是因为耐久度使用完毕和自动卸装，就将它放入容纳装备的背包中
            if (!isOver)
                TryGetBag("EquipmentBag").Add(suit);
            //获取json制定的自定义属性，给角色减益
            if (suit.TryGetProperty("atk") != null)
                playerData.DelAtk(int.Parse(suit.TryGetProperty("atk").ToString()));
            if (suit.TryGetProperty("def") != null)
                playerData.DelDef(int.Parse(suit.TryGetProperty("def").ToString()));
            //更新UI面板的属性值
            propertyPanel.UpdataUIData(playerData.Hp, playerData.Mp, playerData.Atk, playerData.Def);
            //更新容器面板
            UIDelegate.Delegate.UpdataBagUI();
        }
    }
    /// <summary>
    /// 使用某物品时调用
    /// </summary>
    /// <param name="consumable">目标消耗品</param>
    public override void OnUsedSomething(RookiesGoods_Consumable consumable)
    {

        //如果消耗品的游戏种类是食物 
        if (consumable.ItemType == "food" )
        {
            //获取json制定的自定义属性，给角色增益
            if (consumable.TryGetProperty("hp") != null)
                playerData.AddHp(int.Parse(consumable.TryGetProperty("hp").ToString()));
            if (consumable.TryGetProperty("mp") != null)
                playerData.AddMp(int.Parse(consumable.TryGetProperty("mp").ToString()));
            //标记使用过物品，框架会从容器移除它
            consumable.BeUsed(this);
            //更新UI面板的属性值
            propertyPanel.UpdataUIData(playerData.Hp, playerData.Mp, playerData.Atk, playerData.Def);
            //更新容器面板
            UIDelegate.Delegate.UpdataBagUI();
        }
    }
}
