using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPropertyPanel : MonoBehaviour
{
    private Text hp;
    private Text mp;
    private Text atk;
    private Text def;
    private void Start()
    {
        foreach(Transform child in GetComponentsInChildren<Transform>())
        {
            switch(child.gameObject.name)
            {
                case "hp":
                    hp = child.GetComponent<Text>();
                    break;
                case "mp":
                    mp = child.GetComponent<Text>();
                    break;
                case "atk":
                    atk = child.GetComponent<Text>();
                    break;
                case "def":
                    def = child.GetComponent<Text>();
                    break;
            }
        }
    }
    public void UpdataUIData(int h,int m, int a, int d)
    {
        hp.text = "hp: " + h;
        mp.text = "mp: " + m;
        atk.text = "atk: " + a;
        def.text = "def: " + d;
    }
}
