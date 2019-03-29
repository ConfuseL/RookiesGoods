using System;
using System.Collections.Generic;
[Serializable]
public class RookiesGoods_Composite : RookiesGoods_GoodsBase, Composite
{
    public RookiesGoods_Composite(int id, string name, string type, string intro, string effect ,int maxNum) : base(id, name, type, intro, effect, maxNum)
    {

    }

    public Dictionary<int, int> CompositeTable { get ; set; }

    public void UpdateComposite(int[] compositeTable)
    {
        if (CompositeTable == null)
            CompositeTable = new Dictionary<int, int>();
        else
            CompositeTable.Clear();
        for (int i = 0; i < compositeTable.Length; i += 2)
            CompositeTable.Add(compositeTable[i], compositeTable[i + 1]);
    }

}
