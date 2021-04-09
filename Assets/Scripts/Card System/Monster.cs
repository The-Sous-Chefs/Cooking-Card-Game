using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Monster{
    public string name;
    public int level;
    public int initialHp;
    public int currentHp;
    public int basicAtt;
    public int skilleffect;
    public int[] actionpattern;

    public Monster(string name, int level, int initialHp, int basicAtt, int skilleffect, int[] action)
    {
  
        this.name = name;
        this.level = level;
        this.initialHp = initialHp;
        this.basicAtt = basicAtt;
        this.skilleffect = skilleffect;;
        this.currentHp = initialHp;
        //default action pattern, 1 means attack, 2 means skill, 0 means rest
        this.actionpattern = action;
    }

    public void hpDecrease(int value)
    {
        this.currentHp -= value;
    }

    public void hpIncrease(int value)
    {
        this.currentHp += value;
        if(this.currentHp > this.initialHp)
        {
            this.currentHp = this.initialHp;
        }
    }
        
}