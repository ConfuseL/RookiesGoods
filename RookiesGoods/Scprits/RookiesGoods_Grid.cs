using System.Collections.Generic;

public class RookiesGoods_Grid 
{
    //public int EspecialId { get ; private set; }
    public int Num { get; private set; }
    public RookiesGoods_GoodsBase Item { get; private set; }

    public RookiesGoods_Grid()
    {
        Num = 0;
    }

    public void setGood(RookiesGoods_GoodsBase target)
    {
        Item = target;
    }

    public int AddNum(int target)
    {
        int more = 0;

        if (Item == null)
            more = -1;
        Num += target;
        more = Num - Item.MaxNum;
        if (more > 0)
            Num -= more;
        else
            more = 0;
        return more;
    }
    public int DelNum(int target)
    {
        int more = -1;
        Num -= target;
        if(Num<=0)
        {
            more = Num * -1;
            Item = null;
            Num = 0;
        }
        return more;
    }
}

public class GridComparer : IComparer<RookiesGoods_Grid>
{
    public int Compare(RookiesGoods_Grid x, RookiesGoods_Grid y)
    {
        if (x.Num < y.Num)
            return -1;
        else
            return 1;
    }
}

public class GridComparerByDurability : IComparer<RookiesGoods_Grid>
{
    public int Compare(RookiesGoods_Grid x, RookiesGoods_Grid y)
    {
        if (((RookiesGoods_SuitBase)x.Item).Durability < ((RookiesGoods_SuitBase)y.Item).Durability)
            return -1;
        else
            return 1;
    }
}