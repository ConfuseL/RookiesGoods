using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class StorageConfigWindow : EditorWindow
{
    //用户输入是否正确->是否可以储存
    bool canSave=false;
    string strTemp;
    static string DELETESIGN = "deletesign";
    Vector2 scrollPos;
    GUIStyle label = new GUIStyle();

    //存储从xml读出来的容器配置
    List<Storage> StorageMenu=new List<Storage>();

    //存储从xml读出来的存储类型配置
    //原始数据，用于在容器配置中选择
    List<string> RriginalTypeList = new List<string>();

    //备份数据，用于让用户在存储类型配置中更改，各部会影响下方的容器配置
    List<string> ModifiableTypeList = new List<string>();

    //用来显示下拉内容的存储类型数组
    string[] typeArray;

    //容器类 不使用结构体的原因是因为List<结构体>会返回值而不是引用
    class Storage
    {
       public string name;
        public string saveType;
        public int volume;
      public  Storage(string n,string s, int v)
        {
            name = n;
            saveType = s;
            volume = v;
        }
    }
    StorageConfigWindow()
    {
        titleContent = new GUIContent("容器配置窗口");
    }
    private void OnEnable()
    {
        minSize = new Vector2(300, 300);
        label.fontSize = 20;
        Load();
    }
    private void OnGUI()
    {
        canSave = true;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width),GUILayout.Height(position.height));
        GUILayout.Label(" 存储种类配置", label);
        if(ModifiableTypeList.Count>0)
            EditorGUILayout.TextField("类型", ModifiableTypeList[0]);
            for (int i = 1; i < ModifiableTypeList.Count; i++)
            {
                if (!ModifiableTypeList[i].Equals(DELETESIGN))
                {
                    strTemp = EditorGUILayout.TextField("类型", ModifiableTypeList[i]);
                    ModifiableTypeList[i] = strTemp;
                    if (GUILayout.Button("删除"))
                    {
                        if (i > ModifiableTypeList.Count)
                            ModifiableTypeList.Remove(ModifiableTypeList[i]);
                        else
                            ModifiableTypeList[i] = DELETESIGN;
                    }
                }
            }

        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("添加存储类型"))
        {
            ModifiableTypeList.Add("");
        }
        if (GUILayout.Button("保存存储配置"))
        {
            SaveType();
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.HelpBox("存储配置保存之后，容器配置中的存储类型才会出现新选项喔", MessageType.Info);
        GUILayout.Space(20);
        GUILayout.Label(" 容器配置", label);
        for (int i=0; i< StorageMenu.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("容器名字");
            StorageMenu[i].name = EditorGUILayout.TextField(StorageMenu[i].name);
            for(int j=0;j< StorageMenu.Count;j++)
            {
                if (j == i)
                    continue;
                if(StorageMenu[i].name.Equals(StorageMenu[j].name))
                {
                    EditorGUILayout.HelpBox("不允许出现相同的容器名", MessageType.Error);
                    canSave = false;
                }
            }
            GUILayout.Label("存储类型");
            int index = EditorGUILayout.Popup(Array.IndexOf(typeArray, StorageMenu[i].saveType), typeArray);
            StorageMenu[i].saveType = typeArray[index];
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("容量");
            StorageMenu[i].volume = EditorGUILayout.IntField(StorageMenu[i].volume);
            if (GUILayout.Button("删除容器"))
            {
                StorageMenu.Remove(StorageMenu[i--]);
            }
            GUILayout.EndHorizontal();
            if (i>=0&& StorageMenu[i].name.Equals(""))
            {
                EditorGUILayout.HelpBox("名字不能为空", MessageType.Error);
                canSave = false;
            }
            else if (i > 0 && StorageMenu[i].saveType.Equals(""))
            {
                EditorGUILayout.HelpBox("存储类型不能为空", MessageType.Error);
                canSave = false;
            }
            else if (i > 0 && StorageMenu[i].volume < 1)
            {
                EditorGUILayout.HelpBox("容量必须大于1", MessageType.Error);
                canSave = false;
            }
            else
                canSave = true;
        }
        GUILayout.Space(5);
        if (GUILayout.Button("添加容器"))
        {
            StorageMenu.Add(new Storage("", "all", 1));
        }
        GUILayout.Space(5);
        if (canSave&&GUILayout.Button("保存容器数据"))
        {
                SaveContainer();
        }
        GUILayout.Space(5);
        if (GUILayout.Button("重新加载容器数据"))
        {
            Load();
        }
        GUILayout.Label("By ConfuseL");
        EditorGUILayout.EndScrollView();
    }
    void Load()
    {
        StorageMenu.Clear();
        RriginalTypeList.Clear();
        ModifiableTypeList.Clear();
        XmlDocument xml = new XmlDocument();
        xml.Load(Application.dataPath + "/RookiesGoods/Config/RookiesGoods_Config.xml");
        if (xml.SelectSingleNode("RookiesGoods_Config/BagMenu") != null)
        {
            XmlNodeList xmlNodeList = xml.SelectSingleNode("RookiesGoods_Config/BagMenu").ChildNodes;
            if (xmlNodeList != null)
                foreach (XmlNode item in xmlNodeList)
                {
                    string volume = (item.SelectSingleNode("Volume").InnerText);
                    string type = (item.SelectSingleNode("Type").InnerText);
                    StorageMenu.Add(new Storage(item.Name, type, int.Parse(volume)));
                }
        }
        if (xml.SelectSingleNode("RookiesGoods_Config/SaveTypes") != null)
        {
            XmlNodeList xmlNodeList = xml.SelectSingleNode("RookiesGoods_Config/SaveTypes").ChildNodes;
            if (xmlNodeList != null)
            {
                foreach (XmlElement item in xmlNodeList)
                {
                    string type = item.GetAttribute("type");
                    RriginalTypeList.Add(type);
                    ModifiableTypeList.Add(type);
                }
            }
        }
        else
        {
            RriginalTypeList.Add("all");
            ModifiableTypeList.Add("all");
        }
        typeArray = RriginalTypeList.ToArray();

    }
    void SaveType()
    {
        XmlDocument xml = new XmlDocument();
        xml.Load(Application.dataPath + "/RookiesGoods/Config/RookiesGoods_Config.xml");
        XmlNode root = xml.SelectSingleNode("RookiesGoods_Config");
        XmlNode xmlNode = xml.SelectSingleNode("RookiesGoods_Config/SaveTypes");
        MyDistinct();
        if (xmlNode!=null)
        {
            root.RemoveChild(xmlNode);
            XmlNodeList xmlNodeList = xml.SelectSingleNode("RookiesGoods_Config/BagMenu").ChildNodes;
            if (xmlNodeList != null)
            {
                int i;
                for (i = 0; i < RriginalTypeList.Count; i++)
                {
                    //如果用户把原始的存储类型删除，让该类型的物品和容器的存储类型变为默认的all
                    if (ModifiableTypeList[i].Equals(DELETESIGN))
                    {
                        foreach (XmlNode item in xmlNodeList)
                        {
                            string type = (item.SelectSingleNode("Type").InnerText);
                            if(type.Equals(RriginalTypeList[i].ToLower()))
                            {
                                item.SelectSingleNode("Type").InnerText = "all";
                                //把全局物品管理器里的对应种类的物品的存储种类更改为空，因为该种类被删除了
                                RookiesGoods_OverallManage.GoodsManage.ChangeSaveType(RriginalTypeList[i].ToLower(),"empty");
                            }
                        }
                    }
                    //如果原始的和更改后的不一样，同样对该类型为物品和容器的存储类型变为改变后的类型
                    else if (!ModifiableTypeList[i].Equals(RriginalTypeList[i]))
                    {
                        foreach (XmlNode item in xmlNodeList)
                        {
                            string type = (item.SelectSingleNode("Type").InnerText);
                            if (type.Equals(RriginalTypeList[i].ToLower()))
                            {
                                item.SelectSingleNode("Type").InnerText = ModifiableTypeList[i].ToLower();
                                //把全局物品管理器里的对应种类的物品的存储种类更改为新的种类，因为该种类被替换了
                                RookiesGoods_OverallManage.GoodsManage.ChangeSaveType(RriginalTypeList[i].ToLower(), ModifiableTypeList[i].ToLower());
                            }
                        }
                    }
                }
                for (i = ModifiableTypeList.Count-1; i >=0; i--)
                {
                    if(ModifiableTypeList[i].Equals(DELETESIGN))
                     ModifiableTypeList.Remove(ModifiableTypeList[i]);
                }
            }

        }
        XmlElement typeNode = xml.CreateElement("SaveTypes");
        root.AppendChild(typeNode);
        for (int i = 0; i < ModifiableTypeList.Count; i++)
        {
            XmlElement type = xml.CreateElement("Type");
            type.SetAttribute("id", (i).ToString());
            type.SetAttribute("type", ModifiableTypeList[i].ToLower());
            typeNode.AppendChild(type);
        }
        xml.Save(Application.dataPath + "/RookiesGoods/Config/RookiesGoods_Config.xml");
        //刷新json文件在Unity上的缓存 否则会一直是没存储前的json文件
        AssetDatabase.Refresh();
        Load();
        if (RookiesGoods_OverallManage.GoodsManage.UpdateSaveChange != null)
            RookiesGoods_OverallManage.GoodsManage.UpdateSaveChange();
    }
    void SaveContainer()
    {
        XmlDocument xml = new XmlDocument();
        xml.Load(Application.dataPath + "/RookiesGoods/Config/RookiesGoods_Config.xml");
        XmlNode root = xml.SelectSingleNode("RookiesGoods_Config");
        XmlNode bagMenu =xml.SelectSingleNode("RookiesGoods_Config/BagMenu");
        if (bagMenu != null)
        {
            root.RemoveChild(bagMenu);
        }
        XmlElement typeNode = xml.CreateElement("BagMenu");
        root.AppendChild(typeNode);
        for (int i = 0; i < StorageMenu.Count; i++)
        {
            XmlElement container = xml.CreateElement(StorageMenu[i].name);
            XmlElement temp = xml.CreateElement("Volume");
            temp.InnerText = StorageMenu[i].volume.ToString();
            container.AppendChild(temp);
            temp = xml.CreateElement("Type");
            temp.InnerText = StorageMenu[i].saveType.ToLower();
            container.AppendChild(temp);
            typeNode.AppendChild(container);
        }
        xml.Save(Application.dataPath + "/RookiesGoods/Config/RookiesGoods_Config.xml");
        //刷新json文件在Unity上的缓存 否则会一直是没存储前的json文件
        AssetDatabase.Refresh();
    }

    //类型list去重
    void MyDistinct()
    {
        for(int i=0;i<ModifiableTypeList.Count;i++)
        {
            //从后往前删
            for(int j= ModifiableTypeList.Count-1;j>i;j--)
            {
                //如果j不是原始的且被标记为删除的类型，并且i和j相等 ，删除j
                if (!ModifiableTypeList[j].Equals(DELETESIGN) && ModifiableTypeList[j].Equals(ModifiableTypeList[i]))
                    ModifiableTypeList.Remove(ModifiableTypeList[j]);
            }
        }
    }
}
