using System;
[Serializable]
public class RookiesGoods_SuitBase : RookiesGoods_Composite,DurabilityByCount
{
    public int WearerID { get; private set; }
    public int Durability { get; set ; }

    public RookiesGoods_SuitBase(int id, string name, string type, string intro, string effect, int maxNum) : base(id, name, type, intro, effect, maxNum)
    {
        WearerID = 0;
    }

    /// <summary>
    /// 装备给某角色
    /// </summary>
    /// <param name="player">角色</param>
    /// <returns>装备是否成功</returns>
    public bool SuitUp(RookiesGoods_PlayerData player)
    {
        if (player == null)
            return false;
        WearerID = player.PlayerId;
        if (Durability > 0)
        {
            if (SpecialId == 0)
            {
                SetSpecialID(RookiesGoods_OverallManage.GoodsManage.RegisterDurabilityGoods(Id));
            }
        }
        player.OnSuitUpSomething(this);
        return true;
    }
    /// <summary>
    /// 从角色身上去除
    /// </summary>
    /// <returns>如果当前没人装备 返回false</returns>
    public bool TakeOff(bool isOver=false)
    {
        RookiesGoods_PlayerData player=RookiesGoods_OverallManage.GoodsManage.Try2GetPlayer(WearerID);
        if (player == null)
            return false;

        player.OnTakeOffSuccessful(this, isOver);
        player = null;
        WearerID = 0;
        return true;
    }


    public void BeUsed()
    {
        if (Durability >= 0)
        {
            Durability--;
        }

        if (Durability == 0)
        {
            int lastWearID = WearerID;
            TakeOff(true);
            FinishedUse(lastWearID);
        }
    }

    public void FinishedUse(int lastWearID)
    {
        RookiesGoods_PlayerData player = RookiesGoods_OverallManage.GoodsManage.Try2GetPlayer(lastWearID);
        if (player != null)
            player.AutoDelByBag(this,1, SpecialId);
    }
}
