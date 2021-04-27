using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterAction {
    REST,
    ATTACK,
    SKILL
}

public class Monster
{
    public string name;
    public int level;
    public int maxHP;
    public int currentHP;
    public int attackDamage;
    public int stunDuration;
    public bool stunned;
    public MonsterAction[] actionPattern;
    private int actionPatternIndex;

    public Monster(string name, int level, int maxHP, int attackDamage, int stunDuration, MonsterAction[] actionPattern)
    {
        this.name = name;
        this.level = level;
        this.maxHP = maxHP;
        this.currentHP = maxHP;
        this.attackDamage = attackDamage;
        this.stunDuration = stunDuration;
        this.actionPattern = actionPattern;
        this.stunned = false;
    }

    public void IncreaseHP(int value)
    {
        this.currentHP += value;
        if (this.currentHP > this.maxHP)
        {
            this.currentHP = this.maxHP;
        }
    }

    public void DecreaseHP(int value)
    {
        this.currentHP -= value;
    }

    public MonsterAction GetTurnAction()
    {
        MonsterAction actionTaken = MonsterAction.REST;
        if(!stunned)
        {
            actionTaken = actionPattern[actionPatternIndex];
        }
        actionPatternIndex = (actionPatternIndex + 1) % actionPattern.Length;
        return actionTaken;
    }

    public void BecomeStunned()
    {
        this.stunned = true;
    }

    public void ClearEffects()
    {
        // will initialize all the buff/debuff here since we will count that in our DCCS every turn
        this.stunned = false;
    }
}
