using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private int cardID;
    [SerializeField] private Enemy targetEnemy;
    [SerializeField] private Tempchef curChef;
    private Card currentCard;
    private int enemyPatternIndex;
    private int cardIndex;
    public Text cardIDText;
    public Text cardBodyText;
    public Text handText;
    public int startingHandSize;
    public List<int> hand;

    void Start()
    {
        handsInitialize();
        cardID = hand[0];
        currentCard = CardDatabase.Instance.GetCardByID(cardID);
        Debug.Log("Card " + currentCard.name + " loaded.");
    }

    void Update()
    {
        cardID = hand[cardIndex];
        currentCard = CardDatabase.Instance.GetCardByID(cardID);
        cardIDText.text = "Card ID: " + cardID + ", Cost: " + currentCard.cost;
        cardBodyText.text = currentCard.cardText;
        handText.text = IntListToString(hand);
    }

    private void handsInitialize()
    {
        hand = new List<int>();
        for(int i = 0; i < startingHandSize; i++) {
            //randomly draw cards
            hand.Add(UnityEngine.Random.Range(0, 30));
        }
    }

    public void PlayCard()
    {
        Debug.Log("Scanning " + currentCard.name);
        //only if remaining mana is affordable
        if(CanPayCost()) {
            TargettedDamageHandler(targetEnemy);
            HealHandler();
            BlockHandler();
            AOEDamageHandler();
            DrawHandler();
            DiscardHandler();
            hand.RemoveAt(cardIndex);
            // need to do range things, will fix later
            cardIndex = 0;
        } else {
             Debug.Log("Not enough GM.");
        }
    }

    private bool CanPayCost()
    {
        return PlayerStats.Instance.TrySpendMana(currentCard.cost);
    }

    private bool TargettedDamageHandler(Enemy targetEnemy)
    {
        if(currentCard.singleDamage > 0)
        {
            Debug.Log("Dealing " + currentCard.singleDamage + " damage to " + targetEnemy.enmName);
            targetEnemy.demoMonster.DecreaseHP(currentCard.singleDamage);
            return true;
        }
        return false;
    }

    private bool AOEDamageHandler()
    {
        // placeholder before we have a multi-enemies battle system
        if(currentCard.aoeDamage > 0)
        {
            Debug.Log("Dealing " + currentCard.aoeDamage + " damage to all");
            targetEnemy.demoMonster.DecreaseHP(currentCard.aoeDamage);
            return true;
        }
        return false;
    }

    private bool HealHandler()
    {
        if(currentCard.heal > 0)
        {
            Debug.Log("Healing the chef by " + currentCard.heal);
            PlayerStats.Instance.ApplyHealing(currentCard.heal);
            return true;
        }
        return false;
    }

    private bool BlockHandler()
    {
        if(currentCard.blockPercent >0)
        {
            if(currentCard.cardType == CardType.IMMEDIATE )
            {
                Debug.Log("current turn, Blocking vlaue =  " + currentCard.blockPercent);
                curChef.blocking((int)(currentCard.blockPercent* 100),1);
                return true;
            }
            else
            {
                Debug.Log( currentCard.turnsInPlay + "turn, Blocking vlaue =  " + currentCard.blockPercent);
                curChef.blocking((int)(currentCard.blockPercent* 100),currentCard.turnsInPlay);
                return true;
            }
        }
        return false;
    }

    private bool DrawHandler()
    {
        if(currentCard.draw > 0)
        {
            for(int i = 0; i <currentCard.draw; i++)
            {
                hand.Add(UnityEngine.Random.Range(0, 30));
            }
            return true;
        }
        return false;
    }

    //randomly discard some number of cards 
    private bool DiscardHandler()
    {
         if(currentCard.discard > 0)
         {
            for(int i = 0; i <currentCard.discard; i++)
            {
                if(hand.Count > 0)
                {
                    hand.RemoveAt(UnityEngine.Random.Range(0, hand.Count));
                }
            }
            return true;
        }
        return false;
    }

    public void DoEnemyTurn()
    {
        int curAction = targetEnemy.demoMonster.actionpattern[enemyPatternIndex];
        Debug.Log("Enemy turn start : " + curAction);
        switch(curAction)
        {
            case 0:
                break;
            case 1:
                HandleEnemyAttack();
                break;
            case 2:
                HandleEnemySpecialSkill();
                break;
            default:
                Debug.Log("Invalid behavior in monster: " + targetEnemy.demoMonster.name);
                break;
        }

        enemyPatternIndex += 1;
        if(enemyPatternIndex == targetEnemy.demoMonster.actionpattern.Length)
        {
            enemyPatternIndex = 0;
        }
        curChef.buffupdate();
    }

    private void HandleEnemyAttack()
    {
        Debug.Log("og damgade:"+targetEnemy.demoMonster.basicAtt );
        int damagedealed = targetEnemy.demoMonster.basicAtt * (100 - curChef.buff[1,0])/100;
        Debug.Log("percentage:"+ curChef.buff[1,0] );
        PlayerStats.Instance.ApplyDamage(damagedealed);
    }

    private void HandleEnemySpecialSkill()
    {
        // turns plus 1 because the counter will decrease once enemy finish its move
        curChef.getStunned(targetEnemy.demoMonster.skilleffect +1);
        Debug.Log(targetEnemy.demoMonster.name + " spelled its skill." +targetEnemy.demoMonster.skilleffect);
    }

    public void IncrementCardNumber()
    {
        cardIndex++;
        if(cardIndex == hand.Count)
        {
            cardIndex = 0;
        }
        cardID = hand[cardIndex]; 
        currentCard = CardDatabase.Instance.GetCardByID(cardID);
    }

    public void DecrementCardNumber()
    {
        cardIndex--;
        if(cardIndex == -1)
        {
            cardIndex = hand.Count - 1;
        }
        cardID = hand[cardIndex]; 
        currentCard = CardDatabase.Instance.GetCardByID(cardID);
    }

    string IntListToString(List<int> list)
    {
        string str="Current hand: [";
        foreach(int n in list)
        {
            str += n + " ,";
        }
        str += "]";
        return str;
    }
}

