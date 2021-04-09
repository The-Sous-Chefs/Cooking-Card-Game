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
        blockHandler();
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
        curChef.buffupdate();
        
    }

    private void enemyAttack()
    {
         Debug.Log("og damgade:"+targetEnemy.demoMonster.basicAtt );
        int damagedealed = targetEnemy.demoMonster.basicAtt * (100 - curChef.buff[1,0])/100;
         Debug.Log("percentage:"+ curChef.buff[1,0] );
        PlayerStats.Instance.ApplyDamage(damagedealed);
    }

    private void enemySpellSkill()
    {
        // turns plus 1 because the counter will decrease once enemy finish its move
        curChef.getStunned(targetEnemy.demoMonster.skilleffect +1);
        Debug.Log(targetEnemy.demoMonster.name + " spelled its skill." +targetEnemy.demoMonster.skilleffect);
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
            PlayerStats.Instance.ApplyHealing(currentCard.heal);
            return true;
        }
        return false;
    }

    private bool costHandler()
    {
        if (currentCard.cost > 0)
        {
            Debug.Log("Cost " + currentCard.cost + " GM");
            PlayerStats.Instance.TrySpendMana(currentCard.cost);
            return true;
        }
        return false;
    }

    private bool blockHandler() {
        if(currentCard.blockPercent >0) {
            if (currentCard.cardType == CardType.IMMEDIATE ) {
                Debug.Log("current turn, Blocking vlaue =  " + currentCard.blockPercent);
                curChef.blocking((int)(currentCard.blockPercent* 100),1);
                return true;
            } else {
                Debug.Log( currentCard.turnsInPlay + "turn, Blocking vlaue =  " + currentCard.blockPercent);
                curChef.blocking((int)(currentCard.blockPercent* 100),currentCard.turnsInPlay);
                return true;
            }
        }
        return false;
    }
}
