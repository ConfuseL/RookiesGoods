using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RookiesGoodsMenu : ScriptableWizard
{
    [MenuItem("RookiesGoods/GoodsConfig")]
    static void GoodsConfig()
    {
        GetWindow(typeof(RookiesGoodsConfigWindow));
    
    }

    [MenuItem("RookiesGoods/StorageConfig")]
    static void StorageConfig()
    {
        GetWindow(typeof(StorageConfigWindow));
    }
    [MenuItem("RookiesGoods/VisitGitHub")]
    static void GoToGitHub()
    {
        System.Diagnostics.Process.Start("explorer.exe", "https://github.com/ConfuseL/RookiesGoods");
    }
}
