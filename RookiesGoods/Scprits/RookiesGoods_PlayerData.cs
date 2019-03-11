using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Text.RegularExpressions;
using System;
public abstract class RookiesGoods_PlayerData :MonoBehaviour
{
    /// <summary>
    /// 角色id
    /// </summary>
    public int PlayerId { get ; private set ; }

    /// <summary>
    /// 角色储存物品的容器 可用于背包、仓库
    /// </summary>
    public Dictionary<string, RookiesGoods_Bag> Bags { get; private set; }

    /// <summary>
    /// 背包目录
    /// </summary>
    private Dictionary<string, List<string>> BagsMenu { get ;  set ; }

    /// <summary>
    /// 存储物品对应的最大收纳量
    /// </summary>
    public Dictionary<int, int> GoodsMaxVolume { get; private set; }


    /// <summary>
    /// 合成成功的回调
    /// </summary>
    /// /// <param name="composite">合成物的信息</param>
    public abstract void OnComposeSuccessful(RookiesGoods_Composite composite);


    /// <summary>
    /// 合成失败的回调
    /// </summary>
    /// /// <param name="composite">合成物的信息</param>
    public abstract void OnComposeFailed(RookiesGoods_Composite composite, string reason);

    /// <summary>
    /// 使用成功的回调
    /// </summary>
    /// /// <param name="composite">合成物的信息</param>
    public abstract void OnUsedSomething(RookiesGoods_Consumable  consumable);

    /// <summary>
    /// 装备成功的回调
    /// </summary>
    /// <param name="suit">装备的信息</param>
    public abstract void OnSuitUpSomething(RookiesGoods_SuitBase suit);



    /// <summary>
    /// 解除装备的回调
    /// </summary>
    /// <param name="suit">装备的信息</param>
    public abstract void OnTakeOffSuccessful(RookiesGoods_SuitBase suit,bool isOver);


    public void SetID(int playerId)
    {
        PlayerId = playerId;
    }

    /// <summary>
    /// 自动添加物品，根据类型自动添加对应背包。优先添加于储存类型为ALL的背包，若其不存在或者空间不足后添加于对应储存类型的背包。
    /// 如果不想这么做，你可以利用
    /// </summary>
    /// <param name="goods">物品实体</param>
    /// <param name="num">物品数量</param>
    /// <returns>返回添加之后多余的数量 ，如果返回值与添加数量一致，说明背包控件已满</returns>
    public int AutoAdd2Bag(RookiesGoods_GoodsBase goods, int num=1)
    {
        int more= num;
        if (BagsMenu.ContainsKey("ALL"))
        {
            //RookiesGoods_Bag temp;
            //Bags.TryGetValue(BagsMenu["ALL"], out temp);
            //if (temp != null)
            //{
            //    more=temp.Add(goods, num);
            //}

            List<string> bags= BagsMenu["ALL"];
            for (int i = 0; i < bags.Count; i++)
            {
                RookiesGoods_Bag temp;
                Bags.TryGetValue(bags[i], out temp);
                if (temp != null && more > 0)
                {
                    more = temp.Add(goods, num);
                }
            }
        }
        if (more > 0)
        {
            if (BagsMenu.ContainsKey(goods.ItemType.ToUpper()))
            {
                //RookiesGoods_Bag temp;
                //Bags.TryGetValue(BagsMenu[goods.ItemType.ToUpper()], out temp);
                //if (temp != null)
                //{
                //    more = temp.Add(goods, num);
                //}
                List<string> bags = BagsMenu[goods.ItemType.ToUpper()];
                for (int i = 0; i < bags.Count; i++)
                {
                    RookiesGoods_Bag temp;
                    Bags.TryGetValue(bags[i], out temp);
                    if (temp != null&&more>0)
                    {
                        more = temp.Add(goods, num);
                    }
                }
            }
        }
        return more;
    }


    /// <summary>
    /// 在全部背包中检索并删除物品
    /// </summary>
    /// <param name="goods">物品实体</param>
    /// <param name="num">物品数量</param>
    /// <returns>返回删除是否成功 失败的原因：背包里没有该物品</returns>
    public bool AutoDelByBag(RookiesGoods_GoodsBase goods, int num = 1,int specialID=0)
    {
        bool flag = false;
        if (BagsMenu.ContainsKey("ALL"))
        {
            //RookiesGoods_Bag temp;
            //Bags.TryGetValue(BagsMenu["ALL"], out temp);
            //if (temp != null)
            //{
            //    flag = temp.Delete(goods.Id, num, specialID);
            //}
            List<string> bags = BagsMenu["ALL"];
            for (int i = 0; i < bags.Count; i++)
            {
                RookiesGoods_Bag temp;
                Bags.TryGetValue(bags[i], out temp);
                if (temp != null)
                {
                    flag = temp.Delete(goods.Id, num, specialID);
                }
            }
        }
        else if(flag==false)
        {
            if (BagsMenu.ContainsKey(goods.ItemType.ToUpper()))
            {
                //RookiesGoods_Bag temp;
                //Bags.TryGetValue(BagsMenu[goods.ItemType.ToUpper()], out temp);
                //if (temp != null)
                //{
                //    flag = temp.Delete(goods.Id, num, specialID);
                //}
                List<string> bags = BagsMenu[goods.ItemType.ToUpper()];
                for (int i = 0; i < bags.Count; i++)
                {
                    RookiesGoods_Bag temp;
                    Bags.TryGetValue(bags[i], out temp);
                    if (temp != null)
                    {
                        flag = temp.Delete(goods.Id, num, specialID);
                        if (flag)
                            return true;
                    }
                }
            }

        }
        return flag;
    }

    

    public void Init(int playerID)
    {
        PlayerId = playerID;
        //TODO 读取角色文件，发现是否有存档，有就读取 否则
        LoadXML();
        RookiesGoods_OverallManage.GoodsManage.RegisterPlayer(this);
    }

    private void LoadXML()
    {
        BagsMenu = new Dictionary<string, List<string>>();
        Bags = new Dictionary<string, RookiesGoods_Bag>();
        XmlDocument xml = new XmlDocument();
        xml.Load(Application.dataPath + "/RookiesGoods/Config/RookiesGoods_Config.xml");
        XmlNodeList xmlNodeList = xml.SelectSingleNode("RookiesGoods_Config/BagMenu").ChildNodes;
        foreach (XmlNode item in xmlNodeList)
        {
           string volume=(item.SelectSingleNode("Volume").InnerText);
           string type = (item.SelectSingleNode("Type").InnerText);
           Regex regExp = new Regex(@"^[0-9]+$");
            if (!regExp.IsMatch(volume))
            {
                throw new ArgumentException("RookiesGoods_Config.xml中的"+item.Name+"背包容量应为一个全数字单位");
            }
            if (BagsMenu.ContainsKey(type.ToUpper()))
            {
                List<string> bags;
                BagsMenu.TryGetValue(type.ToUpper(), out bags);
                bags.Add(item.Name.ToUpper());
            }
            else
            {
                List<string> bags = new List<string>();
                bags.Add(item.Name.ToUpper());
                BagsMenu.Add(type.ToUpper(), bags);
            }
            Bags.Add(item.Name.ToUpper(), new RookiesGoods_Bag(int.Parse(volume)));
        }
    }

    public RookiesGoods_Bag TryGetBag(string bagName)
    {
        RookiesGoods_Bag bag= null;
        Bags.TryGetValue(bagName.ToUpper(), out bag);
        return bag;
    }

    /// <summary>
    /// 检查时候可以合成某物品
    /// </summary>
    /// <param name="target">物品id</param>
    /// <param name="player"> 想要合成该物品的玩家的RookiesGoods_PlayerData实体  </param>
    /// <param name="bagNames">角色要检查的背包名</param>
    /// <returns></returns>
    public bool CheckSynthesize(int targetID, params string[] bagNames)
    {
        RookiesGoods_Composite target =(RookiesGoods_Composite) RookiesGoods_OverallManage.GoodsManage.GetGoods(targetID);
        if (target == null)
        {
            OnComposeFailed(target, "不存在目标物品");
            return false;
        }
        Dictionary<int, int> tempCounter = new Dictionary<int, int>(target.CompositeTable);
        int[] keys = new int[tempCounter.Count];
        int i = 0;
        foreach (int id in tempCounter.Keys)
        {
            keys[i++] = id;
        }
        if (bagNames.Length == 0)
        {
            for (i = 0; i < tempCounter.Count; i++)
            {
                foreach (RookiesGoods_Bag bag in Bags.Values)
                {
                    tempCounter[keys[i]] -= bag.FindOwnNumber(keys[i]);
                    if (tempCounter[keys[i]] <= 0)
                        break;
                }
                if (tempCounter[keys[i]] > 0)
                {
                    OnComposeFailed(target, "材料不足");
                    return false;
                }
            }
            return true;
        }
        else
        {
            RookiesGoods_Bag bag;
            List<RookiesGoods_Bag> searchList = new List<RookiesGoods_Bag>();
            for (i = 0; i < bagNames.Length; i++)
            {
                bag = TryGetBag(bagNames[i]);
                if (bag == null)
                    throw new ArgumentException("参数" + bagNames[i] + "错误，找不到角色对应背包");
                searchList.Add(bag);
            }
             for (i = 0; i < tempCounter.Count; i++)
            {
                foreach (RookiesGoods_Bag temp in Bags.Values)
                {
                    tempCounter[keys[i]] -= temp.FindOwnNumber(keys[i]);
                    if (tempCounter[keys[i]] <= 0)
                        break;
                }
                    if (tempCounter[keys[i]] > 0)
                    {
                        OnComposeFailed(target, "材料不足");
                        return false;
                    }
            }
            return true;
        }
    }


    /// <summary>
    /// 合成物品
    /// </summary>
    /// <param name="targetID"></param>
    /// <param name="bagNames"></param>
    /// <returns></returns>
    public bool AutoTrying2Synthesize(int targetID, params string[] bagNames)
    {

        if (!CheckSynthesize(targetID, bagNames))
            return false;
        RookiesGoods_Composite target = (RookiesGoods_Composite)RookiesGoods_OverallManage.GoodsManage.GetGoods(targetID);
        Dictionary<int, int> tempCounter = new Dictionary<int, int>(target.CompositeTable);
        int[] keys = new int[tempCounter.Count];
        int i = 0;
        foreach (int id in tempCounter.Keys)
        {
            keys[i++] = id;
        }
        if (bagNames.Length == 0)
        {

            for (i = 0; i < tempCounter.Count; i++)
            {
                foreach (RookiesGoods_Bag bag in Bags.Values)
                {
                    if (!bag.IsThere(keys[i]))
                        continue;
                    if (tempCounter[keys[i]] >= bag.FindOwnNumber(keys[i]))
                    {
                        bag.Delete(keys[i], bag.FindOwnNumber(keys[i]));
                        tempCounter[keys[i]] -= bag.FindOwnNumber(keys[i]);
                        if (tempCounter[keys[i]] == 0)
                            break;
                    }
                    else
                    {
                        bag.Delete(keys[i], tempCounter[keys[i]]);
                        break;
                    }
                }
            }
            OnComposeSuccessful(target);
            return true;
        }
        else
        {
            RookiesGoods_Bag bag;
            List<RookiesGoods_Bag> searchList = new List<RookiesGoods_Bag>();
            for (i = 0; i < bagNames.Length; i++)
            {
                bag = TryGetBag(bagNames[i]);
                if (bag == null)
                    throw new ArgumentException("参数" + bagNames[i] + "错误，找不到角色对应背包");
                searchList.Add(bag);
            }
            foreach (int id in tempCounter.Keys)
            {
                for (i = 0; i < tempCounter.Count; i++)
                {
                    foreach (RookiesGoods_Bag temp in Bags.Values)
                    {
                        if (tempCounter[keys[i]] >= temp.FindOwnNumber(keys[i]))
                        {
                            temp.Delete(keys[i], temp.FindOwnNumber(keys[i]));
                            tempCounter[keys[i]] -= temp.FindOwnNumber(keys[i]);
                            if (tempCounter[keys[i]] == 0)
                                break;
                        }
                        else
                        {
                            temp.Delete(keys[i], temp.FindOwnNumber(keys[i]) - tempCounter[keys[i]]);
                            break;
                        }
                    }
                }
            }
            OnComposeSuccessful(target);
            return true;
        }
    }
}
