using System;
using System.Collections.Generic;
[Serializable]
public class RookiesGoods_GoodsBase
{
    /// <summary>
    /// 唯一标识符
    /// </summary>
    public int Id { private set; get; }

    /// <summary>
    /// 特殊标识符
    /// </summary>
    public int SpecialId { private set; get; }

    /// <summary>
    /// 物品名字
    /// </summary>
    public string Name { private set; get; }
    /// <summary>
    /// 物品存储类型
    /// </summary>
    public string SaveType { private set; get; }
    /// <summary>
    /// 物品对象类型
    /// </summary>
    public string Type { private set; get; }
    /// <summary>
    /// 物品介绍
    /// </summary>
    public string Intro { private set; get; }
    /// <summary>
    /// 物品作用说明 可有可无 主要给物品设计者备注，也可以展示给玩家
    /// </summary>
    public string Effect { private set; get; }

    /// <summary>
    /// 图片路径
    /// </summary>
    public string Sprite { private set; get; }

    /// <summary>
    /// 动态属性
    /// </summary>
    public Dictionary<string, object> Property { get; private set; }

    /// <summary>
    /// 在角色的物品容器中，单格储存的最大量
    /// </summary>
    public int MaxNum { private set; get; }

    public void SetType(string type)
    {
        Type = type;
    }

    public void SetSpritePath(string path)
    {
        Sprite = path;
    }

    public RookiesGoods_GoodsBase(int id, string name, string type, string intro, string effect, int maxNum)
    {
        Id = id;
        Name = name;
        SaveType = type;
        Intro = intro;
        Effect = effect;
        MaxNum = maxNum;
        Sprite = "";
    }


    public void ReSet(int id, string name, string type, string intro, string effect, int maxNum)
    {
        Id = id;
        Name = name;
        SaveType = type;
        Intro = intro;
        Effect = effect;
        MaxNum = maxNum;
    }

    public object TryGetProperty(string propertyName)
    {
        object res = null;
       Property.TryGetValue(propertyName, out  res);
        return res;
    }

    public void AddProperty(string propertyName, object target)
    {
        if (Property == null)
            Property = new Dictionary<string, object>();
        if (target == null)
            throw new NullReferenceException("属性不能为空！");
        if (Property.ContainsKey(propertyName))
            throw new NullReferenceException("属性已经存在！");
        Property.Add(propertyName, target);
    }

    public void SetSpecialID(int SpecialId)
    {
        this.SpecialId = SpecialId;
    }

    public void ChangeSaveType(string target)
    {
        SaveType = target;
    }
}

/// <summary>
/// 可合成物品接口
/// </summary>
public interface Composite
{
    /// <summary>
    /// 合成表
    /// </summary>
    Dictionary<int, int> CompositeTable { get; set; }

    void UpdateComposite(int[] compositeTable);
}

/// <summary>
/// 次数型耐久度物品接口
/// </summary>
public interface DurabilityByCount
{
    int Durability { get; set; }

    void BeUsed();

    void FinishedUse(int lastWearID);
}

/// <summary>
/// 时效型耐久度物品接口
/// </summary>
public interface DurabilityByTimes
{
    float Durability { get; set; }

    void BeUsed(float timer);

    void FinishedUse();
}