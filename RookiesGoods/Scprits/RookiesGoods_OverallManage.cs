using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using LitJson;
using UnityEngine;
[Serializable]
public class RookiesGoods_OverallManage 
{
    private Dictionary<int, RookiesGoods_GoodsBase> PrototypeGoodsMenu { get;  set; }

    private List<int> DurabilityID { get; set; }

    private List<int> CompositeID { get; set; }

    private Dictionary<int, Sprite> SpriteDic { get; set; }

    public static RookiesGoods_OverallManage GoodsManage { get { return Instance ?? (Instance = new RookiesGoods_OverallManage()); } }

    private static  RookiesGoods_OverallManage Instance { get;  set; }

    private Dictionary<int, RookiesGoods_PlayerData> PlayersData { get; set; }

    public bool IsSavingAfterQuit { get; set;}

    // Start is called before the first frame update
    public RookiesGoods_OverallManage()
    {
        IsSavingAfterQuit = false;
        PrototypeGoodsMenu = new Dictionary<int, RookiesGoods_GoodsBase>();
        DurabilityID = new List<int>();
        SpriteDic = new Dictionary<int, Sprite>();
        PlayersData = new Dictionary<int, RookiesGoods_PlayerData>();
        CompositeID = new List<int>();
        InitData();
    }

    public void RegisterPlayer(RookiesGoods_PlayerData player)
    {
        if (PlayersData.ContainsKey(player.PlayerId))
            throw new ArgumentException("以存在角色ID");
        PlayersData.Add(player.PlayerId, player);
    }

    public List<int> GetCompositeID()
    {
        return new List<int>(CompositeID);
    }

    public int RegisterDurabilityGoods( int goodsId)
    {
        int specialID=0;

        if(DurabilityID.Count==0)
            DurabilityID.Add(1);
        else
            DurabilityID.Add(DurabilityID[DurabilityID.Count - 1] + 1);
        specialID = DurabilityID[DurabilityID.Count - 1];
        return specialID;
    }

    public void LogOffDurabilityGoods(int specialID)
    {
        for (int i = 0; i < DurabilityID.Count; i++)
        {
            if (DurabilityID[i] == specialID)
                DurabilityID.Remove(DurabilityID[i]);
        }
    }

    public static T DeepCopy<T>(T obj)
    {
        object retval;
        using (MemoryStream ms = new MemoryStream())
        {
            BinaryFormatter bf = new BinaryFormatter();
            //序列化成流
            bf.Serialize(ms, obj);
            ms.Seek(0, SeekOrigin.Begin);
            //反序列化成对象
            retval = bf.Deserialize(ms);
            ms.Close();
        }
        return (T)retval;
    }

    public RookiesGoods_GoodsBase GetGoods(int id)
    {
        RookiesGoods_GoodsBase goods;
        PrototypeGoodsMenu.TryGetValue(id,out goods);
        if (goods.Type.Equals("RookiesGoods_SuitBase"))
            if (((RookiesGoods_SuitBase)goods).Durability != 0)
                return DeepCopy(goods);
        return goods;
    }

    public Sprite Try2GetSprite(int id)
    {
        Sprite sprite = null;
        SpriteDic.TryGetValue(id, out sprite);
        return sprite;
    }

    public RookiesGoods_PlayerData Try2GetPlayer(int id)
    {
        RookiesGoods_PlayerData player =null;
        PlayersData.TryGetValue(id, out player);
        return player;
    }

    public void InitData()
    {

        XmlDocument xml = new XmlDocument();
        xml.Load(Application.dataPath + "/RookiesGoods/Config/RookiesGoods_Config.xml");
        XmlNodeList xmlNodeList = xml.SelectSingleNode("RookiesGoods_Config/JsonPath").ChildNodes;
        string path=xmlNodeList[0].InnerText;
        if (File.Exists(Application.dataPath + "/Resources/"+ path+".json"))
        {
            string classType, itemType, name, intro, effect,sprite;
            int id, maxNum, durability,length;
            int[] arrayValue;
            List<int> listValue=new List<int>();
            TextAsset textAsset = Resources.Load(path) as TextAsset;
            JsonData jsonData = JsonMapper.ToObject(textAsset.text);
            foreach (JsonData data in jsonData)
            {
                JsonData stringValue = data["classType"];
                JsonData intValue;
                JsonData arrayValues;
                JsonData propertyValue;
                classType = stringValue.ToString();
                intValue = data["id"];
                id = int.Parse(intValue.ToString());
                intValue = data["maxNum"];
                maxNum = int.Parse(intValue.ToString());
                stringValue = data["itemType"];
                itemType = stringValue.ToString();
                stringValue = data["name"];
                name = stringValue.ToString();
                stringValue = data["intro"];
                intro = stringValue.ToString();
                stringValue = data["effect"];
                effect = stringValue.ToString();
                stringValue = data["spritePath"];
                sprite = stringValue.ToString();
                intValue = data["durability"];
                durability = int.Parse(intValue.ToString());
                arrayValues = data["compositeArray"];
				if(sprite!="")
                  SpriteDic.Add(id,Resources.Load<Sprite>(sprite));
                switch (classType)
                {
                    case "RookiesGoods_SuitBase":
                        RookiesGoods_SuitBase suit;
                        suit = new RookiesGoods_SuitBase(id, name, itemType, intro, effect, maxNum);
                        suit.SetType(classType);
                        length = arrayValues.Count;
                        arrayValue = new int[length];
                        for (int i = 0; i < length; i++)
                            arrayValue[i] = int.Parse(arrayValues[i].ToString());
                        if (length > 0)
                        {
                            CompositeID.Add(id);
                            suit.UpdateComposite(arrayValue);
                        }
                        suit.Durability = durability;
                        propertyValue = data["specialProperty"];
                        foreach (string key in propertyValue.Keys)
                        {
                            JsonData temp=propertyValue[key];
                            switch (temp.GetJsonType())
                            {
                                case JsonType.None:
                                    break;
                                case JsonType.Object:
                                    suit.AddProperty(key, temp);
                                    break;
                                case JsonType.Array:
                                    length = temp.Count;
                                    listValue.Clear();
                                    for (int i = 0; i < length; i++)
                                        listValue.Add(int.Parse(temp[i].ToString()));
                                    if (length > 0)
                                        suit.AddProperty(key, listValue);
                                        break;
                                case JsonType.String:
                                    suit.AddProperty(key, temp.ToString());                                   
                                    break;
                                case JsonType.Int:
                                    suit.AddProperty(key, int.Parse(temp.ToString()));
                                    break;
                                case JsonType.Long:
                                    suit.AddProperty(key, long.Parse(temp.ToString()));
                                    break;
                                case JsonType.Double:
                                    suit.AddProperty(key, double.Parse(temp.ToString()));                                   
                                    break;
                                case JsonType.Boolean:
                                    suit.AddProperty(key, bool.Parse(temp.ToString()));
                                    break;
                                default:
                                    break;
                            }
                        }
                        PrototypeGoodsMenu.Add(id, suit);
                        break;
                    case "RookiesGoods_Composite":
                        RookiesGoods_Composite goods;                     
                        goods = new RookiesGoods_Composite(id, name, itemType, intro, effect, maxNum);
                        goods.SetType(classType);
                        length = arrayValues.Count;
                        arrayValue = new int[length];
                        for (int i = 0; i < length; i++)
                            arrayValue[i] = int.Parse(arrayValues[i].ToString());
                        if (length > 0)
                        {
                            CompositeID.Add(id);
                            goods.UpdateComposite(arrayValue);
                        }
                        propertyValue = data["specialProperty"];
                        foreach (string key in propertyValue.Keys)
                        {
                            JsonData temp = propertyValue[key];
                            switch (temp.GetJsonType())
                            {
                                case JsonType.None:
                                    break;
                                case JsonType.Object:
                                    goods.AddProperty(key, temp);
                                    break;
                                case JsonType.Array:
                                    length = temp.Count;
                                    listValue.Clear();
                                    for (int i = 0; i < length; i++)
                                        listValue.Add(int.Parse(temp[i].ToString()));
                                    if (length > 0)
                                        goods.AddProperty(key, listValue);
                                    break;
                                case JsonType.String:
                                    goods.AddProperty(key, temp.ToString());
                                    break;
                                case JsonType.Int:
                                    goods.AddProperty(key, int.Parse(temp.ToString()));
                                    break;
                                case JsonType.Long:
                                    goods.AddProperty(key, long.Parse(temp.ToString()));
                                    break;
                                case JsonType.Double:
                                    goods.AddProperty(key, double.Parse(temp.ToString()));
                                    break;
                                case JsonType.Boolean:
                                    goods.AddProperty(key, bool.Parse(temp.ToString()));
                                    break;
                                default:
                                    break;
                            }
                        }
                        PrototypeGoodsMenu.Add(id, goods);
                        break;
                    case "RookiesGoods_Consumable":
                        RookiesGoods_Consumable consumable;
                        consumable = new RookiesGoods_Consumable(id, name, itemType, intro, effect, maxNum);
                        consumable.SetType(classType);
                        length = arrayValues.Count;
                        arrayValue = new int[length];
                        for (int i = 0; i < length; i++)
                            arrayValue[i] = int.Parse(arrayValues[i].ToString());
                        if (length > 0)
                        {
                            CompositeID.Add(id);
                            consumable.UpdateComposite(arrayValue);
                        }
                        propertyValue = data["specialProperty"];
                        foreach (string key in propertyValue.Keys)
                        {
                            JsonData temp = propertyValue[key];
                            switch (temp.GetJsonType())
                            {
                                case JsonType.None:
                                    break;
                                case JsonType.Object:
                                    consumable.AddProperty(key, temp);
                                    break;
                                case JsonType.Array:
                                    length = temp.Count;
                                    listValue.Clear();
                                    for (int i = 0; i < length; i++)
                                        listValue.Add(int.Parse(temp[i].ToString()));
                                    if (length > 0)
                                        consumable.AddProperty(key, listValue);
                                    break;
                                case JsonType.String:
                                    consumable.AddProperty(key, temp.ToString());
                                    break;
                                case JsonType.Int:
                                    consumable.AddProperty(key, int.Parse(temp.ToString()));
                                    break;
                                case JsonType.Long:
                                    consumable.AddProperty(key, long.Parse(temp.ToString()));
                                    break;
                                case JsonType.Double:
                                    consumable.AddProperty(key, double.Parse(temp.ToString()));
                                    break;
                                case JsonType.Boolean:
                                    consumable.AddProperty(key, bool.Parse(temp.ToString()));
                                    break;
                                default:
                                    break;
                            }
                        }
                        PrototypeGoodsMenu.Add(id, consumable);
                        break;
                    default:
                        throw new ArgumentException(string.Format("id {0}的框架类型错误",id));
                }
            }
        }
        else
        {
            throw new ArgumentException("丢失物品配置文件");
        }
    }

    /// <summary>
    /// 全部物品数据存档
    /// </summary>
    public void SaveAll()
    {
        foreach (RookiesGoods_PlayerData temp in PlayersData.Values)
            temp.Save();
    }
}
