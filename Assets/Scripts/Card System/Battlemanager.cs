using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Battlemanager : MonoBehaviour
{
    [SerializeField] private int cardID;
    [SerializeField] private Enemy targetEnemy;
    [SerializeField] private Tempchef curChef;
    private Card currentCard;
    private int enemypatternIndex;
    public Text cardid;
    public Text cardtx;

    void Start()
    {
        currentCard = CardDatabase.Instance.GetCardByID(cardID);
        Debug.Log("Card " + currentCard.name + " loaded.");
    }

    public void scancard()
    {
        Debug.Log("Scanning " + currentCard.name);
        singleDamageHandler(targetEnemy);
        healHandler();





        costHandler();
       
    }

    public void enemyturn()
    {
        int curAction = targetEnemy.demoMonster.actionpattern[enemypatternIndex];
        Debug.Log("Enemy turn start : " + curAction);
        switch (curAction)
        {
            case 0:
                break;
            case 1:
                enemyAttack();
                break;
            case 2:
                enemySpellSkill();
                break;
            default:
                Debug.Log("Invalid behavior in monster: " + targetEnemy.demoMonster.name);
                break;


        }

        enemypatternIndex += 1;
        if (enemypatternIndex == targetEnemy.demoMonster.actionpattern.Length)
        {
            enemypatternIndex = 0;
        }
    }

    private void enemyAttack()
    {
        curChef.hpDecrease(targetEnemy.demoMonster.basicAtt);
    }

    private void enemySpellSkill()
    {
        Debug.Log(targetEnemy.demoMonster.name + " spelled its skill.");
    }

    void Update()
    {
        cardid.text = "Card ID: " + cardID;
        cardtx.text = currentCard.cardText;
    }

    public void addCardNum()
    {
        cardID++;
        currentCard = CardDatabase.Instance.GetCardByID(cardID);
    }

    public void decCardNum()
    {
        cardID--;
        currentCard = CardDatabase.Instance.GetCardByID(cardID);
    }

    // 3/30 and 4/6 demos only: currently just defaultly take effect to the one
    // enemy
    // Need to implement target later
    private bool singleDamageHandler(Enemy targetEnemy)
    {
        if (currentCard.singleDamage > 0)
        {
            Debug.Log("Dealing " + currentCard.singleDamage + " damage to " + targetEnemy.enmName);
            targetEnemy.demoMonster.hpDecrease(currentCard.singleDamage);
            return true;
        }
        return false;
    }

    private bool healHandler()
    {
        if (currentCard.heal > 0)
        {
            Debug.Log("Healing the chef by " + currentCard.heal);
            curChef.hpIncrease(currentCard.heal);
            return true;
        }
        return false;
    }

    private bool costHandler()
    {
        if (currentCard.cost > 0)
        {
            Debug.Log("Cost " + currentCard.cost + " GM");
            curChef.gmDec(currentCard.cost);
            return true;
        }
        return false;
    }
}
