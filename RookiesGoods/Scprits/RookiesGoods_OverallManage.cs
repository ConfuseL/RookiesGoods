using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using LitJson;
using UnityEditor;
using UnityEngine;
[Serializable]
public class RookiesGoods_OverallManage 
{
    //物品目录
    private Dictionary<int, RookiesGoods_GoodsBase> PrototypeGoodsMenu { get;  set; }
    //为每一个拥有耐久度的物品添加一个映射，保证独一无二
    private List<int> DurabilityID { get; set; }
    //可合成物品的id列表
    private List<int> CompositeID { get; set; }
    //图片存储
    private Dictionary<int, Sprite> SpriteDic { get; set; }
    //单例
    public static RookiesGoods_OverallManage GoodsManage { get { return Instance ?? (Instance = new RookiesGoods_OverallManage()); } }

    private static  RookiesGoods_OverallManage Instance { get;  set; }
    //多角色管理器
    private Dictionary<int, RookiesGoods_PlayerData> PlayersData { get; set; }
    //是否自动在游戏退出时保存角色数据
    public bool IsSavingAfterQuit { get; set;}

    public RookiesGoods_OverallManage()
    {
        InitData();
    }
    /// <summary>
    /// 注册角色
    /// </summary>
    /// <param name="player">角色物品管理器</param>
    public void RegisterPlayer(RookiesGoods_PlayerData player)
    {
        if (PlayersData.ContainsKey(player.PlayerId))
            throw new ArgumentException("以存在角色ID");
        PlayersData.Add(player.PlayerId, player);
    }
    /// <summary>
    /// 获得可合成物品的id 用于编辑器
    /// </summary>
    /// <returns></returns>
    public List<int> GetCompositeID()
    {
        return new List<int>(CompositeID);
    }
    /// <summary>
    /// 注册拥有有效耐久度物品
    /// </summary>
    /// <param name="goodsId">物品id</param>
    /// <returns></returns>
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
    /// <summary>
    /// 移除有效耐久度物品，即当耐久度归0的时候慧聪映射中移除
    /// </summary>
    /// <param name="specialID"></param>
    public void LogOffDurabilityGoods(int specialID)
    {
        for (int i = 0; i < DurabilityID.Count; i++)
        {
            if (DurabilityID[i] == specialID)
                DurabilityID.Remove(DurabilityID[i]);
        }
    }
    /// <summary>
    /// 序列化深拷贝
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns>返回返回深拷贝的对象</returns>
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

    /// <summary>
    /// 获得物品的引用，或者是深拷贝
    /// 当物品不需要被特殊识别的时候，返回引用，例如食物，在游戏中一种食物的效果都一样，只是每个角色存储了多少而已
    /// 否则返回深拷贝，例如耐久度的武器，每个武器的耐久度都不同，需要保证武器的独一无二
    /// </summary>
    /// <param name="id">物品id</param>
    /// <param name="fromEditor">是否从编辑器窗口调用</param>
    /// <returns>返回引用、深拷贝</returns>
    public RookiesGoods_GoodsBase GetGoods(int id,bool fromEditor=false)
    {
        RookiesGoods_GoodsBase goods;
        PrototypeGoodsMenu.TryGetValue(id,out goods);
        if (goods!=null&&goods.Type.Equals("RookiesGoods_SuitBase") && (!fromEditor))
            if (((RookiesGoods_SuitBase)goods).Durability != 0)
                return DeepCopy(goods);
        return goods;
    }
    /// <summary>
    /// 尝试获取精灵图
    /// </summary>
    /// <param name="id">物品id</param>
    /// <returns>精灵图/空</returns>
    public Sprite Try2GetSprite(int id)
    {
        Sprite sprite = null;
        SpriteDic.TryGetValue(id, out sprite);
        return sprite;
    }
    /// <summary>
    /// 尝试获得角色物品管理器
    /// </summary>
    /// <param name="id">角色id</param>
    /// <returns>角色物品管理器</returns>
    public RookiesGoods_PlayerData Try2GetPlayer(int id)
    {
        RookiesGoods_PlayerData player =null;
        PlayersData.TryGetValue(id, out player);
        return player;
    }
    /// <summary>
    /// 初始化数据，读取xml和json，解析所有数据。
    /// </summary>
    public void InitData()
    {
        IsSavingAfterQuit = false;
        PrototypeGoodsMenu = new Dictionary<int, RookiesGoods_GoodsBase>();
        DurabilityID = new List<int>();
        SpriteDic = new Dictionary<int, Sprite>();
        PlayersData = new Dictionary<int, RookiesGoods_PlayerData>();
        CompositeID = new List<int>();
        XmlDocument xml = new XmlDocument();
        xml.Load(Application.dataPath + "/RookiesGoods/Config/RookiesGoods_Config.xml");
        XmlNodeList xmlNodeList = xml.SelectSingleNode("RookiesGoods_Config/JsonPath").ChildNodes;
        string path=xmlNodeList[0].InnerText;
        if (File.Exists(Application.dataPath + "/Resources/"+ path+".json"))
        {
            string classType, itemType, name, intro, effect,sprite;
            int id, maxNum, durability,length;
            int[] arrayValue;
            List<int> intListValue=new List<int>();
            List<double> dbListValue = new List<double>();
            List<string> strListValue = new List<string>();
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
                                    intListValue.Clear();
                                    strListValue.Clear();
                                    dbListValue.Clear();
                                    if (length>0)
                                        switch (temp[0].GetJsonType())
                                        {
                                            case JsonType.Int:
                                                for (int i = 0; i < length; i++)
                                                    intListValue.Add(int.Parse(temp[i].ToString()));
                                                suit.AddProperty(key, intListValue);
                                                break;
                                            case JsonType.String:
                                                for (int i = 0; i < length; i++)
                                                    strListValue.Add(temp[i].ToString());
                                                suit.AddProperty(key, strListValue);
                                                break;
                                            case JsonType.Double:
                                                for (int i = 0; i < length; i++)
                                                    dbListValue.Add(double.Parse(temp[i].ToString()));
                                                suit.AddProperty(key, dbListValue);
                                                break;
                                            default:
                                                break;
                                        }
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
                                    intListValue.Clear();
                                    strListValue.Clear();
                                    dbListValue.Clear();
                                    if (length > 0)
                                        switch (temp[0].GetJsonType())
                                        {
                                            case JsonType.Int:
                                                for (int i = 0; i < length; i++)
                                                    intListValue.Add(int.Parse(temp[i].ToString()));
                                                goods.AddProperty(key, intListValue);
                                                break;
                                            case JsonType.String:
                                                for (int i = 0; i < length; i++)
                                                    strListValue.Add(temp[i].ToString());
                                                goods.AddProperty(key, strListValue);
                                                break;
                                            case JsonType.Double:
                                                for (int i = 0; i < length; i++)
                                                    dbListValue.Add(double.Parse(temp[i].ToString()));
                                                goods.AddProperty(key, dbListValue);
                                                break;
                                            default:
                                                break;
                                        }
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
                                    intListValue.Clear();
                                    strListValue.Clear();
                                    dbListValue.Clear();
                                    if (length > 0)
                                        switch (temp[0].GetJsonType())
                                        {
                                            case JsonType.Int:
                                                for (int i = 0; i < length; i++)
                                                    intListValue.Add(int.Parse(temp[i].ToString()));
                                                consumable.AddProperty(key, intListValue);
                                                break;
                                            case JsonType.String:
                                                if(IsInt(temp[0].ToString()))
                                                {
                                                    for (int i = 0; i < length; i++)
                                                        intListValue.Add(int.Parse(temp[i].ToString()));
                                                    consumable.AddProperty(key, intListValue);
                                                }
                                                else
                                                {
                                                    for (int i = 0; i < length; i++)
                                                        strListValue.Add(temp[i].ToString());
                                                    consumable.AddProperty(key, strListValue);
                                                }
   
                                                break;
                                            case JsonType.Double:
                                                for (int i = 0; i < length; i++)
                                                    dbListValue.Add(double.Parse(temp[i].ToString()));
                                                consumable.AddProperty(key, dbListValue);
                                                break;
                                            default:
                                                break;
                                        }
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
            Debug.LogError("物品配置文件为空，插件将在你第一次存储物品信息时，自动生成配置文件。或者不使用插件直接编写。");
           // throw new ArgumentException("丢失物品配置文件");
        }
    }

    /// <summary>
    /// 全部角色物品数据存档
    /// </summary>
    public void SaveAll()
    {
        foreach (RookiesGoods_PlayerData temp in PlayersData.Values)
            temp.Save();
    }
    /// <summary>
    /// 获得当前管理的所有物品id 用于编辑器窗口
    /// </summary>
    /// <returns>id列表</returns>
    public List<string> GetAllId()
    {
        List<string> ids = new List<string>(); 
        foreach(int id in PrototypeGoodsMenu.Keys)
        {
            ids.Add(id.ToString());
        }
        return ids;
    }
    /// <summary>
    /// 将物品添加于管理器 用于编辑器窗口
    /// </summary>
    /// <param name="data">物品</param>
    public void Add2GoodMenu(RookiesGoods_GoodsBase data)
    {
        if(PrototypeGoodsMenu.ContainsKey(data.Id))
            throw new ArgumentException(string.Format("id {0}重复添加", data.Id));
        PrototypeGoodsMenu.Add(data.Id, data);
    }
    /// <summary>
    /// 改变物品的存储类型 用于编辑器窗口
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    public void ChangeSaveType(string source,string target)
    {
        foreach(KeyValuePair<int,RookiesGoods_GoodsBase> keyValue in PrototypeGoodsMenu)
        {
            if(keyValue.Value.SaveType.Equals(source))
            {
                keyValue.Value.ChangeSaveType(target);
            }
        }
    }
    /// <summary>
    /// 移除物品 用于编辑器窗口
    /// </summary>
    /// <param name="id">目标id</param>
    public void RemoveGoods(int id)
    {
        if(PrototypeGoodsMenu.ContainsKey(id))
            {
            PrototypeGoodsMenu.Remove(id);
            Save2JSON();
        }
    }

    public bool ContainsGoodsKey(int id)
    {
        return PrototypeGoodsMenu.ContainsKey(id);
    }
    /// <summary>
    /// 存储JOSN文件 用于编辑器窗口
    /// </summary>
    public void Save2JSON()
    {
        if (PrototypeGoodsMenu.Count == 0)
            return;
        XmlDocument xml = new XmlDocument();
        Sprite sp;
        xml.Load(Application.dataPath + "/RookiesGoods/Config/RookiesGoods_Config.xml");
        XmlNodeList xmlNodeList = xml.SelectSingleNode("RookiesGoods_Config/JsonPath").ChildNodes;
        string path = xmlNodeList[0].InnerText;
        path = Application.dataPath + "/Resources/" + path + ".json";
        StringBuilder sb = new StringBuilder();
        JsonWriter writer = new JsonWriter(sb);
        writer.WriteArrayStart();
        foreach (KeyValuePair<int,RookiesGoods_GoodsBase> item in PrototypeGoodsMenu)
        {
            writer.WriteObjectStart();

            writer.WritePropertyName("id");
            writer.Write(item.Key);

            writer.WritePropertyName("classType");
            writer.Write(item.Value.Type);

            writer.WritePropertyName("itemType");
            writer.Write(item.Value.SaveType);

            writer.WritePropertyName("name");
            writer.Write(item.Value.Name);

            writer.WritePropertyName("maxNum");
            writer.Write(item.Value.MaxNum);

            writer.WritePropertyName("spritePath");
            sp = Try2GetSprite(item.Key);
            if (!item.Value.Sprite.Equals(""))
            {
                int index;
                for (index = item.Value.Sprite.Length - 1; index >= 0; index--)
                    if (item.Value.Sprite[index] == '.')
                    {
                        break;
                    }
                writer.Write(item.Value.Sprite.Substring(0, index).Remove(0, 17));
            }
            else if (item.Value.Sprite.Equals("") && sp != null)
            {
                item.Value.SetSpritePath(AssetDatabase.GetAssetPath(sp));
                int index;
                for (index = item.Value.Sprite.Length - 1; index >= 0; index--)
                    if (item.Value.Sprite[index] == '.')
                    {
                        break;
                    }
                writer.Write(item.Value.Sprite.Substring(0, index).Remove(0, 17));
            }
            else
                writer.Write("");

            writer.WritePropertyName("intro");
            writer.Write(item.Value.Intro);

            writer.WritePropertyName("effect");
            writer.Write(item.Value.Effect);

            writer.WritePropertyName("compositeArray");
            writer.WriteArrayStart();
            if(((RookiesGoods_Composite)item.Value).CompositeTable!=null)
            foreach(KeyValuePair<int,int> com in ((RookiesGoods_Composite)item.Value).CompositeTable)
            {
                writer.Write(com.Key);
                writer.Write(com.Value);
            }
            writer.WriteArrayEnd();

            writer.WritePropertyName("durability");
            if(item.Value.Type.Equals("RookiesGoods_SuitBase"))
                writer.Write(((RookiesGoods_SuitBase)item.Value).Durability);
            else
                writer.Write(0);

            writer.WritePropertyName("specialProperty");
            writer.WriteObjectStart();
            if (item.Value.Property != null)
                foreach (KeyValuePair<string, object> pro in item.Value.Property)
            {
                writer.WritePropertyName(pro.Key);
                if (pro.Value.GetType() == typeof(int))
                {
                    writer.Write(pro.Value.ToString());
                }
                else if (pro.Value.GetType() == typeof(List<int>))
                {
                    writer.WriteArrayStart();
                    for (int i = 0; i < ((List<int>)pro.Value).Count; i++)
                        writer.Write(((List<int>)pro.Value)[i]);
                    writer.WriteArrayEnd();
                }
                else if (pro.Value.GetType() == typeof(double))
                {
                    writer.Write(pro.Value.ToString());
                }
                else if (pro.Value.GetType() == typeof(List<double>))
                {
                    writer.WriteArrayStart();
                    for (int i = 0; i < ((List<double>)pro.Value).Count; i++)
                        writer.Write(((List<double>)pro.Value)[i]);
                    writer.WriteArrayEnd();
                }
                else if (pro.Value.GetType() == typeof(string))
                {
                    writer.Write(pro.Value.ToString());
                }
                else if (pro.Value.GetType() == typeof(List<string>))
                {
                    writer.WriteArrayStart();
                    for (int i = 0; i < ((List<string>)pro.Value).Count; i++)
                        writer.Write(((List<string>)pro.Value)[i]);
                    writer.WriteArrayEnd();
                }
            }

            writer.WriteObjectEnd();

            writer.WriteObjectEnd();
        }
        writer.WriteArrayEnd();
        StreamWriter sw = new StreamWriter(path, false, Encoding.GetEncoding("utf-8"));
        string json = Regex.Unescape(sb.ToString());
        sw.Write(json);
        sw.Close();
        sw.Dispose();
        //刷新json文件在Unity上的缓存 否则会一直是没存储前的json文件
        AssetDatabase.Refresh();
        //  EditorUtility.SetDirty(AssetDatabase.LoadAssetAtPath("Assets/Resources/"+xmlNodeList[0].InnerText+".json", typeof(object)));
    }
    /// <summary>
    /// 判断是否为整数，因为litjosn中判断整数和字符都是返回字符类型...所以还需要在判断一次
    /// </summary>
    /// <param name="value">判断的字符串</param>
    /// <returns>返回是否是整数</returns>
    public static bool IsInt(string value)
    {
        return Regex.IsMatch(value, @"^[+-]?\d*$");
    }
}
