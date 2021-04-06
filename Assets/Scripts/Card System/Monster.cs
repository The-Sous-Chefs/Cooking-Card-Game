using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Monster{
    public string name;
    public int level;
    public int initialHp;
    public int currentHp;
    public int basicAtt;
    public bool stuns;
    public int[] actionpattern;

    public Monster(string name, int level, int initialHp, int basicAtt, bool stuns)
    {
        this.name = name;
        this.level = level;
        this.initialHp = initialHp;
        this.basicAtt = basicAtt;
        this.stuns = stuns;
        this.currentHp = initialHp;
        //default action pattern, 1 means attack, 2 means skill, 0 means rest
        this.actionpattern = new int[4] { 1, 1, 1 ,2 };
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