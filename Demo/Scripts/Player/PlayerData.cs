using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 这是用来记录角色属性的脚本，开发者自定义即可
/// </summary>
public class PlayerData : MonoBehaviour
{
    /// <summary>
    /// 生命值
    /// </summary>
   public int Hp { get; private set; }
    /// <summary>
    /// 魔法值
    /// </summary>
    public int Mp { get; private set; }
    /// <summary>
    /// 攻击力
    /// </summary>
    public int Atk { get; private set; }
    /// <summary>
    /// 防御力
    /// </summary>
    public int Def { get; private set; }

    /// <summary>
    /// 恢复生命值
    /// </summary>
    /// <param name="hp">恢复值</param>
    public void AddHp(int hp)
    {
        Hp += hp;
    }
    /// <summary>
    /// 减少生命值
    /// </summary>
    /// <param name="hp">减少值</param>
    public void DelHp(int hp)
    {
        Hp -= hp;
        if (Hp <= 0)
            Debug.Log("死亡");
    }
    /// <summary>
    /// 恢复法力值
    /// </summary>
    /// <param name="mp">法力值</param>
    public void AddMp(int mp)
    {
        Mp += mp;
    }
    /// <summary>
    /// 减少法力值
    /// </summary>
    /// <param name="mp">法力值</param>
    public void DelMp(int mp)
    {
        if (mp>Mp)
        {
            Debug.Log("mp不足");
            return;
        }
        Mp -= mp;
    }
    /// <summary>
    /// 增加攻击力
    /// </summary>
    /// <param name="atk">新增攻击力</param>
    public void AddAtk(int atk)
    {
        Atk += atk;
    }
    /// <summary>
    /// 减少攻击力
    /// </summary>
    /// <param name="atk">减少的攻击力</param>
    public void DelAtk(int atk)
    {
        Atk -= atk;
    }
    /// <summary>
    /// 增加防御力
    /// </summary>
    /// <param name="def">增加的防御力</param>
    public void AddDef(int def)
    {
        Def += def;
    }
    /// <summary>
    /// 减少防御力
    /// </summary>
    /// <param name="def">减少的防御力</param>
    public void DelDef(int def)
    {
        Def -= def;
    }

    private void Awake()
    {
        Hp = 100;
        Mp = 70;
        Atk = 5;
        Def = 0;
    }
}
