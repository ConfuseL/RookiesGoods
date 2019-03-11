using System.Collections.Generic;

public class RookiesGoods_Bag 
{
    public int Vloume { get; private set; }
    public int EmptyGrids { get; private set; }
   // public string SaveType { get; private set; }
    public Dictionary<int, List<RookiesGoods_Grid>> Grids { get; private set; }
    List<RookiesGoods_Grid> pool;

    public RookiesGoods_Bag(int vloume)
    {
        EmptyGrids=Vloume = vloume;
        Grids = new Dictionary<int, List<RookiesGoods_Grid>>();
        pool = new List<RookiesGoods_Grid>();
    }
    /// <summary>
    /// 更新这个背包的容量
    /// </summary>
    /// <param name="newVloume"></param>
    /// <returns>如果更新后容量缩小，且产生物品超出的数量，则不会改变且返回false，否则改变并true</returns>
    public bool UpdateVloume(int newVloume)
    {
        if (newVloume < Vloume - EmptyGrids)
        {
            return false;
        }
        Vloume = newVloume;
        return true;
    }

    /// <summary>
    /// 向这个背包里添加物品
    /// </summary>
    /// <param name="target">目标物品</param>
    /// <param name="num">数量</param>
    /// <returns></returns>
    public int Add(RookiesGoods_GoodsBase target,int num=1)
    {
        int more=0;
        if (Grids.ContainsKey(target.Id))
        {
            List<RookiesGoods_Grid> gridList;
            Grids.TryGetValue(target.Id, out gridList);
            gridList.Sort(new GridComparer());
            more = gridList[0].AddNum(num);
            while (more > 0 && EmptyGrids > 0)
            {
                RookiesGoods_Grid grid = new RookiesGoods_Grid();
                grid.setGood(target);
                more = grid.AddNum(more);
                gridList.Add(grid);
                EmptyGrids--;
            }
        }
        else if (EmptyGrids>0)
        {
            more = num;
            List<RookiesGoods_Grid> gridList = new List<RookiesGoods_Grid>();
            while (more > 0 && EmptyGrids > 0)
            {
                RookiesGoods_Grid grid = new RookiesGoods_Grid();
                grid.setGood(target);
                more = grid.AddNum(more);
                gridList.Add(grid);
                EmptyGrids--;
            }
            Grids.Add(target.Id, gridList);
        }
        return more;
    }

    /// <summary>
    /// 删除背包物品
    /// </summary>
    /// <param name="id">物品id</param>
    /// <param name="num">数量，默认为1</param>
    /// <param name="specialId">指定的特殊id，默认为0，用于指定有耐久度的某物品</param>
    /// <returns>返回是否删除成功</returns>
    public bool Delete(int id, int num = 1, int specialId = 0)
    {
        int more = 0;
        if (Grids.ContainsKey(id))
        {

            if (specialId != 0)
            {
                List<RookiesGoods_Grid> gridList;
                Grids.TryGetValue(id, out gridList);
                foreach (RookiesGoods_Grid grid in gridList)
                {
                    if (grid.Item.SpecialId == specialId)
                    {
                        gridList.Remove(grid);
                        EmptyGrids++;
                        if (gridList.Count == 0)
                            Grids.Remove(id);
                        return true;
                    }
                }
                return false;
            }
            else
            {
                List<RookiesGoods_Grid> gridList;
                Grids.TryGetValue(id, out gridList);
                if(RookiesGoods_OverallManage.GoodsManage.GetGoods(id).SpecialId!=0)
                    gridList.Sort(new GridComparerByDurability());
                else
                    gridList.Sort(new GridComparer());
                more = gridList[0].DelNum(num);
                if (more >= 0)
                {
                    pool.Add(gridList[0]);
                    gridList.RemoveAt(0);
                    EmptyGrids++;
                }
                while (more > 0 && gridList.Count > 0)
                {
                    more = gridList[0].DelNum(num);
                    if (more > 0)
                    {
                        pool.Add(gridList[0]);
                        gridList.RemoveAt(0);
                        EmptyGrids++;
                    }
                }
                return true;
            }
        }
        else
            return false;
    }

    /// <summary>
    /// 背包是否拥有某id物品
    /// </summary>
    /// <param name="id">物品id</param>
    /// <returns>是否拥有</returns>
    public bool IsThere(int id)
    {
        return Grids.ContainsKey(id);
    }

    public int FindOwnNumber(int targetId)
    {
        int res = 0;
        List<RookiesGoods_Grid> grids;
        Grids.TryGetValue(targetId, out grids);
        if (grids != null)
        {
            for (int i = 0; i < grids.Count; i++)
                res += grids[i].Num;
        }
        return res;
    }

    public List<RookiesGoods_Grid> Try2GetGoodsMessage()
    {
        List<RookiesGoods_Grid> grids=new List<RookiesGoods_Grid>();

        foreach (List<RookiesGoods_Grid> _Grids in Grids.Values)
        {
            for (int i = 0; i < _Grids.Count; i++)
            {
                grids.Add(_Grids[i]);
            }
        }

        return grids;
    }
}
