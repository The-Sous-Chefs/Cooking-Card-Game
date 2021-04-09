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
    private int cardindex;
    public Text cardid;
    public Text cardtx;
    public Text handstx;
    public int beginninghandsNum;
    public List<int> hands;

    void Start()
    {
        handsInitialize();
        cardID = hands[0];
        currentCard = CardDatabase.Instance.GetCardByID(cardID);
        Debug.Log("Card " + currentCard.name + " loaded.");
    }

    private void handsInitialize() {
        hands = new List<int>();
        for (int i = 0; i <beginninghandsNum; i++) {
            //randomly draw cards
            hands.Add(UnityEngine.Random.Range(0, 30));      
        }
    }

    public void playthiscard()
    {
        Debug.Log("Scanning " + currentCard.name);
        //only if remaining mana is affordable
        if (costHandler()) {
            singleDamageHandler(targetEnemy);
            healHandler();
            blockHandler();
            aoeDamageHandler();
            drawHandler();
            discardHandler();
            hands.RemoveAt(cardindex);
            // need to do range things, will fix later
            cardindex = 0;
        } else {
             Debug.Log("Not enough GM.");
        }
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

    string numListToString(List<int> list){
        string str="Current hands: [";
        foreach(int n in list) 
            str+=n+" ,";
            
        str += "]";
        return str;
    }

    void Update()
    {
        cardID = hands[cardindex];
        currentCard = CardDatabase.Instance.GetCardByID(cardID);
        cardid.text = "Card ID: " + cardID + ", Cost: " + currentCard.cost ;
        cardtx.text = currentCard.cardText;
        handstx.text = numListToString(hands);
    }

    public void addCardNum()
    {
        cardindex++;
        if (cardindex == hands.Count) {
            cardindex = 0;
        }
        cardID = hands[cardindex]; 
        currentCard = CardDatabase.Instance.GetCardByID(cardID);
    }

    public void decCardNum()
    {
        cardindex--;
        if (cardindex == -1) {
            cardindex = hands.Count - 1;
        }
        cardID = hands[cardindex]; 
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

    private bool aoeDamageHandler() {
        // placeholder before we have a multi-enemies battle system
        if (currentCard.aoeDamage > 0)
        {
            Debug.Log("Dealing " + currentCard.aoeDamage + " damage to all");
            targetEnemy.demoMonster.hpDecrease(currentCard.aoeDamage);
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
    
        return PlayerStats.Instance.TrySpendMana(currentCard.cost);
     
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

    private bool drawHandler() {
        if (currentCard.draw > 0) {
            for (int i = 0; i <currentCard.draw; i++) {
                hands.Add(UnityEngine.Random.Range(0, 30));
            }
            return true;
        }
        return false;
    }

    //randomly discard some number of cards 
    private bool discardHandler() {
         if (currentCard.discard > 0) {
            for (int i = 0; i <currentCard.discard; i++) {
                if (hands.Count > 0) {
                    hands.RemoveAt(UnityEngine.Random.Range(0, hands.Count));
                }
            }
            return true;
        }
        return false;
    }

}
