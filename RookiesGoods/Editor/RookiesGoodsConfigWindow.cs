using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEditor;
using UnityEngine;


public class RookiesGoodsConfigWindow : EditorWindow
{
    //枚举id的下标
    int idIndex;
    //枚举存储种类的下标
    int saveTypeIndex;
    //物品最大容纳量
    int maxNum;
    //物品的耐久度
    int durability;
    //基础属性     
    string _name = "";
    string intro = "";
    string effect = "";
    string itemType = "";
    string spritePath = "";

    //id枚举数组
    string[] idOption;
    //存储种类枚举数组
    string[] saveTypeOption;
    //用户输入是否正确
    bool canSave=false;
    bool updateData = false;
    Vector2 scrollPos;
    Type type = Type.RookiesGoods_Composite;
    Sprite sprite;
    //当前显示的物品
    RookiesGoods_Composite data = null;
    //合成表
    List<MyPair<int,int>> ComposeTable= new List<MyPair<int, int>>();
    //用户自定义属性 int/int array类型
    List<MyPair<string, List<int>>> IntProperty = new List<MyPair<string, List<int>>>();
    //用户自定义属性 double/double array类型
    List<MyPair<string, List<double>>> DbProperty = new List<MyPair<string, List<double>>>();
    //用户自定义属性 string/string array类型
    List<MyPair<string, List<string>>> StrProperty = new List<MyPair<string, List<string>>>();
    //存储从全局管理器获取的ID列表
    List<string> IdList;

    GUIStyle label = new GUIStyle();

    Regex mRegular = new Regex(".*Assets/Resources/.*", RegexOptions.ExplicitCapture);

    class MyPair<T,Y>
    {
        public T first;
        public Y second;
       public MyPair(T a,Y b)
        {
            first = a;
            second = b;
        }
        public void SetFirst(T target)
        {
            first = target;
        }
        public void SetSecond(Y target)
        {
            second = target;
        }
    }


    enum Type
    {
        RookiesGoods_Composite,
        RookiesGoods_SuitBase,
        RookiesGoods_Consumable
    }

    RookiesGoodsConfigWindow()
    {
        titleContent = new GUIContent("物品配置窗口");
    }

    private void OnEnable()
    {
        IdList = RookiesGoods_OverallManage.GoodsManage.GetAllId();
        if (IdList.Count == 0)
        {
            Debug.Log("正在为你创建物品配置文件");
            data = new RookiesGoods_SuitBase(1, "", "empty", "", "", 1);
            data.SetType("RookiesGoods_Composite");
            RookiesGoods_OverallManage.GoodsManage.Add2GoodMenu(data);
            IdList.Add("1");
            idIndex = 0;
        }
        idOption = IdList.ToArray();
        idIndex = 0;
        minSize = new Vector2(400, 500);
        data = (RookiesGoods_Composite)RookiesGoods_OverallManage.GoodsManage.GetGoods(int.Parse(IdList[0]), true);
        InitMessage();
        InitComposeMessage();
        Load();
        label.fontSize = 20;
    }
    private void OnGUI()
    {
        if (Event.current.type == EventType.KeyDown || Event.current.type == EventType.KeyUp) return;
        canSave = true;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));
        GUILayout.BeginVertical();

        GUILayout.Label(" 物品基础属性", label);

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        idIndex = EditorGUILayout.Popup("物品id",idIndex, idOption);
        if (updateData||data == null||data.Id != int.Parse(idOption[idIndex]))
        {
            data = (RookiesGoods_Composite)RookiesGoods_OverallManage.GoodsManage.GetGoods(int.Parse(idOption[idIndex]),true);
            if (data == null)
            {
                EditorGUILayout.HelpBox("物品不存在", MessageType.Error);
            }
            else
            {
                InitMessage();
                InitComposeMessage();
            }
            updateData = false;
        }

        GUILayout.Space(5);
        GUILayout.EndHorizontal();

        maxNum = EditorGUILayout.IntField("最大容纳量", maxNum);
        if (maxNum < 0)
        {
            EditorGUILayout.HelpBox("容纳量不能为负数", MessageType.Error);
            canSave = false;
        }
        _name = EditorGUILayout.TextField("名字", _name);
        if (_name.Equals(""))
        {
            EditorGUILayout.HelpBox("名字不能为空", MessageType.Error);
            canSave = false;
        }
        type = (Type)EditorGUILayout.EnumPopup("框架对象类型", type);
        if (type == Type.RookiesGoods_SuitBase)
            durability = EditorGUILayout.IntField("耐久度，无限耐久置0", durability);
        saveTypeIndex = EditorGUILayout.Popup("容器存储种类",Array.IndexOf(saveTypeOption, itemType), saveTypeOption);
        itemType = saveTypeOption[saveTypeIndex];
        if (itemType.Equals("empty"))
        {
            EditorGUILayout.HelpBox("存储种类不能为空", MessageType.Error);
            canSave = false;
        }
        GUILayout.EndVertical();

        GUILayout.Space(60);
        sprite = EditorGUI.ObjectField(new Rect(6, 40, position.width - 6, 100), "", sprite, typeof(Sprite), true) as Sprite;
        GUILayout.Label(new GUIContent("精灵图", "请选择在Resource下的精灵图"), EditorStyles.boldLabel);
        GUILayout.EndHorizontal();
        GUILayout.Space(30);
        if (sprite == null || mRegular.IsMatch(AssetDatabase.GetAssetPath(sprite)))
        {
            spritePath = AssetDatabase.GetAssetPath(sprite);
        }
        else
        {
            EditorGUILayout.HelpBox("图片必须在Assets/Resources/下", MessageType.Error);
            canSave = false;
            spritePath = "";
        }
        GUILayout.Label("介绍", GUILayout.MaxWidth(80));
        intro = EditorGUILayout.TextArea(intro, GUILayout.MaxHeight(75));
        if (intro.Equals(""))
        {
            EditorGUILayout.HelpBox("介绍当前为空", MessageType.Warning);
        }
        GUILayout.Label("实际作用", GUILayout.MaxWidth(80));
        effect = EditorGUILayout.TextArea(effect, GUILayout.MaxHeight(75));
        if (effect.Equals(""))
        {
            EditorGUILayout.HelpBox("实际作用当前为空", MessageType.Warning);
        }
        GUILayout.Label("合成表", GUILayout.MaxWidth(80));

        for(int i= 0; i< ComposeTable.Count ; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("合成物id");
            if (RookiesGoods_OverallManage.GoodsManage.ContainsGoodsKey(ComposeTable[i].first))
            {
                int index = EditorGUILayout.Popup(Array.IndexOf(idOption, ComposeTable[i].first.ToString()), idOption);
                EditorGUILayout.TextField(RookiesGoods_OverallManage.GoodsManage.GetGoods(ComposeTable[i].first).Name);
                int num = EditorGUILayout.IntField("数量 :", ComposeTable[i].second);
                ComposeTable[i].SetFirst(int.Parse(idOption[index]));
                ComposeTable[i].SetSecond(num);
            }
            else
            {
                ComposeTable.Remove(ComposeTable[i--]);
                continue;
            }
            if (ComposeTable[i].second < 0)
            {
                EditorGUILayout.HelpBox("数量不能为负数", MessageType.Error);
                canSave=false;
            }
            if (GUILayout.Button("删除"))
            {
                ComposeTable.Remove(ComposeTable[i--]);
            }
            GUILayout.EndHorizontal();

        }
        if (IdList.Count > 1 && GUILayout.Button("添加一个合成物品"))
        {
            ComposeTable.Add(new MyPair<int,int>(1,0)); 
        }
        GUILayout.Space(5);
        GUILayout.Label(" 物品自定义属性", label);
        GUILayout.Space(5);
        GUILayout.Label("整形属性", EditorStyles.boldLabel);
        for (int i=0;i<IntProperty.Count;i++)
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            IntProperty[i].SetFirst(EditorGUILayout.TextField("属性名", IntProperty[i].first));
            for(int j = 0; j < IntProperty.Count; j++)
            {
                if (j == i)
                    continue;
                if(IntProperty[j].first.Equals(IntProperty[i].first))
                {
                    EditorGUILayout.HelpBox("属性名不能相同", MessageType.Error);
                    canSave = false;
                    break;
                }
            }
            if (GUILayout.Button("删除整个属性"))
            {
                IntProperty.Remove(IntProperty[i--]);
            }
            GUILayout.EndHorizontal();
            for (int j=0; i >= 0 && j <IntProperty[i].second.Count; j++)
            {
                EditorGUILayout.BeginHorizontal();
                IntProperty[i].second[j] = EditorGUILayout.IntField("数值 " + j,IntProperty[i].second[j], GUILayout.MaxWidth(position.width- 60));
                if(GUILayout.Button("删除"))
                {
                    IntProperty[i].second.Remove(IntProperty[i].second[j--]);
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("添加一个新值"))
            {
                IntProperty[i].second.Add(0);
            }
        }
        if (GUILayout.Button("添加一个新整形属性"))
        {
            IntProperty.Add(new MyPair<string, List<int>>("", new List<int>(0)));
        }
        GUILayout.Space(5);
        GUILayout.Label("字符属性", EditorStyles.boldLabel);
        for (int i = 0; i < StrProperty.Count; i++)
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            StrProperty[i].SetFirst(EditorGUILayout.TextField("属性名", StrProperty[i].first));
            for (int j = 0; j < StrProperty.Count; j++)
            {
                if (j == i)
                    continue;
                if (StrProperty[j].first.Equals(StrProperty[i].first))
                {
                    EditorGUILayout.HelpBox("属性名不能相同", MessageType.Error);
                    canSave = false;
                    break;
                }
            }
            if (GUILayout.Button("删除整个属性"))
            {
                StrProperty.Remove(StrProperty[i--]);
            }
            GUILayout.EndHorizontal();
            for (int j = 0; i >= 0&&j <StrProperty[i].second.Count; j++)
            {
                EditorGUILayout.BeginHorizontal();
                StrProperty[i].second[j] = EditorGUILayout.TextField("数值 " + j,StrProperty[i].second[j], GUILayout.MaxWidth(position.width - 100));
                if (GUILayout.Button("删除"))
                {
                    StrProperty[i].second.Remove(StrProperty[i].second[j--]);
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("添加一个新值"))
            {
                StrProperty[i].second.Add("");
            }
        }
        if (GUILayout.Button("添加一个新字符属性"))
        {
            StrProperty.Add(new MyPair<string, List<string>>("", new List<string>()));
        }
        GUILayout.Space(5);
        GUILayout.Label("浮点属性", EditorStyles.boldLabel);
        for (int i = 0; i < DbProperty.Count; i++)
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            DbProperty[i].SetFirst(EditorGUILayout.TextField("属性名", DbProperty[i].first));
            for (int j = 0; j < DbProperty.Count; j++)
            {
                if (j == i)
                    continue;
                if (DbProperty[j].first.Equals(DbProperty[i].first))
                {
                    EditorGUILayout.HelpBox("属性名不能相同", MessageType.Error);
                    canSave = false;
                    break;
                }
            }
            if (GUILayout.Button("删除整个属性"))
            {
                DbProperty.Remove(DbProperty[i--]);
            }
            GUILayout.EndHorizontal();
            for (int j = 0; i >= 0 && j < DbProperty[i].second.Count; j++)
            {
                EditorGUILayout.BeginHorizontal();
                DbProperty[i].second[j] = EditorGUILayout.DoubleField("数值 " + j,DbProperty[i].second[j], GUILayout.MaxWidth(position.width - 80));
                if (GUILayout.Button("删除"))
                {
                    DbProperty[i].second.Remove(DbProperty[i].second[j--]);
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("添加一个新值"))
            {
                DbProperty[i].second.Add(0);
            }
        }
        if (GUILayout.Button("添加一个新浮点属性"))
        {
            DbProperty.Add(new MyPair<string, List<double>>("", new List<double>()));
        }
        GUILayout.Space(30);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("添加一个新物品"))
        {
            idIndex= int.Parse(IdList[IdList.Count - 1])+1;
            IdList.Add(idIndex.ToString());
            idOption = IdList.ToArray();
            data = new RookiesGoods_SuitBase(idIndex, "", "empty", "", "", 1);
            idIndex = idOption.Length - 1;
            RookiesGoods_OverallManage.GoodsManage.Add2GoodMenu(data);
            data.SetType("RookiesGoods_Composite");
            updateData = true;
        }
        GUILayout.Space(5);
        if (GUILayout.Button("重新导入所有物品信息"))
        {
            RookiesGoods_OverallManage.GoodsManage.InitData();
            updateData = true;
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (IdList.Count>1&&GUILayout.Button("删除该物品"))
        {
            RookiesGoods_OverallManage.GoodsManage.RemoveGoods(data.Id);
            RookiesGoods_OverallManage.GoodsManage.InitData();
            IdList = RookiesGoods_OverallManage.GoodsManage.GetAllId();
            idOption = IdList.ToArray();
            idIndex = (idIndex==idOption.Length)? idIndex-1: idIndex;
            updateData = true;
        }
        if (!canSave)
            EditorGUILayout.HelpBox("信息不正确无法修改", MessageType.Error);
        else 
        {
            if (GUILayout.Button("保存该物品信息"))
            {
                Save();
                RookiesGoods_OverallManage.GoodsManage.InitData();
                updateData = true;
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.Label("By ConfuseL");

        EditorGUILayout.EndScrollView();
    }
    void LoadGoodsMessage(int id)
    {
        RookiesGoods_Composite good = (RookiesGoods_Composite)RookiesGoods_OverallManage.GoodsManage.GetGoods(id);
        maxNum = good.MaxNum;
        intro = good.Intro;
        effect = good.Effect;
        sprite = RookiesGoods_OverallManage.GoodsManage.Try2GetSprite(id);
    }

    void InitMessage()
    {

        IntProperty.Clear();
        StrProperty.Clear();
        DbProperty.Clear();
            maxNum = data.MaxNum;
            _name = data.Name;
            durability = 0;
            switch (data.Type)
            {
                case "RookiesGoods_SuitBase":
                    type = Type.RookiesGoods_SuitBase;
                    durability = ((RookiesGoods_SuitBase)data).Durability;
                    break;
                case "RookiesGoods_Composite":
                    type = Type.RookiesGoods_Composite;
                    break;
                case "RookiesGoods_Consumable":
                    type = Type.RookiesGoods_Consumable;
                    break;
            }
            itemType = data.SaveType;
            intro = data.Intro;
            effect = data.Effect;
            sprite = RookiesGoods_OverallManage.GoodsManage.Try2GetSprite(data.Id);
        //开始解析动态属性，因为目前仅支持int double string 类型的变量以及其数组形式的变量类型 所以判断6次
        if (data.Property != null)
           foreach(KeyValuePair<string,object> property in data.Property)
            {
            object objTemp;
            if (property.Value.GetType() == typeof(int))
            {
                List<int> temp = new List<int>();
                data.Property.TryGetValue(property.Key, out objTemp);
                temp.Add((int)objTemp);
                IntProperty.Add(new MyPair<string, List<int>>( property.Key, temp));
            }
            else if (property.Value.GetType() == typeof(List<int>))
            {
                data.Property.TryGetValue(property.Key, out objTemp);
                IntProperty.Add(new MyPair<string, List<int>>(property.Key, (List<int>)objTemp));
            }
            else if (property.Value.GetType() == typeof(double))
            {
                List<double> temp = new List<double>();
                data.Property.TryGetValue(property.Key, out objTemp);
                temp.Add((double)objTemp);
                DbProperty.Add(new MyPair<string, List<double>>(property.Key, temp));
            }
            else if (property.Value.GetType() == typeof(List<double>))
            {
                data.Property.TryGetValue(property.Key, out objTemp);
                DbProperty.Add(new MyPair<string, List<double>>(property.Key, (List<double>)objTemp));
            }
           else  if (property.Value.GetType() == typeof(string))
            {
                List<string> temp = new List<string>();
                data.Property.TryGetValue(property.Key, out objTemp);
                temp.Add((string)objTemp);
                StrProperty.Add(new MyPair<string, List<string>>(property.Key, temp));
            }
            else if (property.Value.GetType() == typeof(List<string>))
            {
                data.Property.TryGetValue(property.Key, out objTemp);
                StrProperty.Add(new MyPair<string, List<string>>(property.Key, (List<string>)objTemp));
            }
        }
        
    }
    void InitComposeMessage()
    {
        ComposeTable.Clear();
        if (data.CompositeTable == null)
            return;
        foreach (KeyValuePair<int,int> composition in data.CompositeTable)
        {
            MyPair<int,int> temp = new MyPair<int, int>(composition.Key,composition.Value);
            ComposeTable.Add(temp);
        }
    }

    void Load()
    {
        List<string> temp = new List<string>();
        XmlDocument xml = new XmlDocument();
        xml.Load(Application.dataPath + "/RookiesGoods/Config/RookiesGoods_Config.xml");
        XmlNode root = xml.SelectSingleNode("RookiesGoods_Config");
        XmlNodeList xmlNodeList = xml.SelectSingleNode("RookiesGoods_Config/SaveTypes").ChildNodes;
        if (xmlNodeList != null)
        {
           for(int i=0;i<xmlNodeList.Count;i++)
            {
                temp.Add(xmlNodeList[i].Attributes["type"].Value.Equals("all")?"empty": xmlNodeList[i].Attributes["type"].Value);
            }
            saveTypeOption = temp.ToArray();
        }
    }

    void Save()
    {
        //重写物体
        data.ReSet(data.Id, _name, itemType, intro, effect, maxNum);
        if(type==Type.RookiesGoods_SuitBase)
        {
            ((RookiesGoods_SuitBase)data).Durability = durability;
        }
        data.SetType(type.ToString());
        data.SetSpritePath(spritePath);
        //擦除原始自定义属性
        if (data.Property!=null)
        data.Property.Clear();
        //添加新属性
        int cnt;
        for(cnt = 0; cnt < IntProperty.Count; cnt++)
        {
            if(IntProperty[cnt].second.Count==1)
            {
                data.AddProperty(IntProperty[cnt].first, IntProperty[cnt].second[0]);
            }
            else
            {
                data.AddProperty(IntProperty[cnt].first, IntProperty[cnt].second);
            }
        }
        for (cnt = 0; cnt < DbProperty.Count; cnt++)
        {
            if (DbProperty[cnt].second.Count == 1)
            {
                data.AddProperty(DbProperty[cnt].first, DbProperty[cnt].second[0]);
            }
            else
            {
                data.AddProperty(DbProperty[cnt].first, DbProperty[cnt].second);
            }
        }
        for (cnt = 0; cnt < StrProperty.Count; cnt++)
        {
            if (StrProperty[cnt].second.Count == 1)
            {
                data.AddProperty(StrProperty[cnt].first, StrProperty[cnt].second[0]);
            }
            else
            {
                data.AddProperty(StrProperty[cnt].first, StrProperty[cnt].second);
            }
        }
        int[] compositionArray = new int[ComposeTable.Count * 2];
        for(cnt = 0;cnt< ComposeTable.Count;cnt++)
        {
            compositionArray[2*cnt] = ComposeTable[cnt].first;
            compositionArray[2*cnt+1] = ComposeTable[cnt].second;
        }
        data.UpdateComposite(compositionArray);
        RookiesGoods_OverallManage.GoodsManage.Save2JSON();
    }

}
