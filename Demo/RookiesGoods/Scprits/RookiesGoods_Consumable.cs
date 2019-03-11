using System;

[Serializable]
public class RookiesGoods_Consumable : RookiesGoods_Composite
{
    public RookiesGoods_Consumable(int id, string name, string type, string intro, string effect, int maxNum) : base(id, name, type, intro, effect, maxNum)
    {

    }

    private string FromBag { get; set; }

    public void Used(RookiesGoods_PlayerData player,string bagName="")
    {
        FromBag = bagName;
        player.OnUsedSomething(this);
    }

    public void BeUsed(RookiesGoods_PlayerData player)
    {
        if (FromBag == "" || player.TryGetBag(FromBag) == null)
            player.AutoDelByBag(this);
        else
            player.TryGetBag(FromBag).Delete(Id);
    }
}
