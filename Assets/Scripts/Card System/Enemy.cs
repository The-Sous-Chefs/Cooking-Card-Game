using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public List<Monster> monsterList;
    //public Monster demoMonster = new Monster("demoMonster",1,50,3, 1, new int[4] { 1, 1, 1, 2 });
    public Text hptxt;
    public Text enmName;
  
    // Start is called before the first frame update
    void Start()
    {
        monsterList = new List<Monster>();
        Monster demoMonster = new Monster("demoMonster", 1, 50, 3, 1, new int[4] { 1, 1, 1, 2 });
        monsterList.Add(demoMonster);
        enmName.text = monsterList[0].name;
    }

    // Update is called once per frame
    void Update()
    {
        hptxt.text = "Hp :" + monsterList[0].currentHp;
        if (monsterList[0].stunned) {
            GetComponent<Image>().color = Color.red;
        } else {
            GetComponent<Image>().color = Color.white;
        }
    }
}

public class Monster
{
    public string name;
    public int level;
    public int initialHp;
    public int currentHp;
    public int basicAtt;
    public int skilleffect;
    public int[] actionpattern;
    public bool stunned;

    public Monster(string name, int level, int initialHp, int basicAtt, int skilleffect, int[] action)
    {
        this.name = name;
        this.level = level;
        this.initialHp = initialHp;
        this.basicAtt = basicAtt;
        this.skilleffect = skilleffect; ;
        this.currentHp = initialHp;
        //default action pattern, 1 means attack, 2 means skill, 0 means rest
        this.actionpattern = action;
        this.stunned = false;
    }

    public void IncreaseHP(int value)
    {
        this.currentHp += value;
        if (this.currentHp > this.initialHp)
        {
            this.currentHp = this.initialHp;
        }
    }

    public void DecreaseHP(int value)
    {
        this.currentHp -= value;
    }

    public void getStunned()
    {
        stunned = true;
    }

    public void clearEffect()
    {
        //will initialize all the buff/debuff here since we will count that in our DCCS very turn
        stunned = false;

    }
}
