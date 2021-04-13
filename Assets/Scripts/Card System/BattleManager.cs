using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System;
using UnityEngine.UI;

public class DCCard
{
    //-----------------
    // member variables
    //-----------------

    public int cardID          { get;}
    public int cooldownCounter { get; private set; }

    public DCCard(int cardID, int cooldownCounter)
    {
        Debug.Assert(cooldownCounter > 0);
        this.cardID = cardID;
        this.cooldownCounter = cooldownCounter;
    }

    public bool DecrementCounter()
    {
        cooldownCounter--;
        return cooldownCounter == 0;
    }
}

public class BattleManager : MonoBehaviour
{
    public List<int> deck;
    public List<int> discardPile;
    public List<int> hand;
    public List<DCCard> dccs;

    // TEMPORARY: Just for testing before UI is implemented, should be removed
    [SerializeField] private int cardID;
    [SerializeField] private Enemy targetEnemy;
    [SerializeField] private Tempchef curChef;
    private Card currentCard;
    private int enemyPatternIndex;
    private int cardIndex;

    public Text cardIDText;
    public Text cardBodyText;
    public Text handText;
    public Text handListText;
    public Text deckSizeText;
    public Text discardPileSizeText;
    public Text dccsContentsText;
    void Start()
    {
        // initialize the deck, discardPile, hand, and DCCS
        deck = PlayerStats.Instance.GetCollectedCardIDs();
        ShuffleDeck();
        discardPile = new List<int>();
        hand = new List<int>();
        dccs = new List<DCCard>();

        // draw the player's opening hand
        for(int i = 0; i < Constants.STARTING_HAND_SIZE; i++)
        {
            DrawCard();
        }
        // debugging card: put the card would like to test here
        // hand.Add(0);
        cardID = hand[0];
        currentCard = CardDatabase.Instance.GetCardByID(cardID);
        Debug.Log("Card " + currentCard.name + " loaded.");
    }

    void Update()
    {
        if (hand.Count != 0) {
            cardID = hand[cardIndex];
        } else {
            cardID = 0;
        }
        currentCard = CardDatabase.Instance.GetCardByID(cardID);
        cardIDText.text = "Card ID: " + cardID + ", Name: " + currentCard.name + ", Cost: " + currentCard.cost;
        cardBodyText.text = currentCard.cardText;
        handText.text = IntListToString(hand);

        string handList = "";
        foreach(int id in hand)
        {
            handList += CardDatabase.Instance.GetCardByID(id).ToString() + "\n\n";
        }
        handListText.text = handList;
        deckSizeText.text = deck.Count.ToString();
        discardPileSizeText.text = discardPile.Count.ToString();

        string dccsList = "";
        foreach(DCCard card in dccs)
        {
            dccsList += CardDatabase.Instance.GetCardByID(card.cardID).ToString() +
                    "\nThe Above Card's Cooldown is: " + card.cooldownCounter + "\n\n";
        }
        dccsContentsText.text = dccsList;
    }

    private void ShuffleDeck()
    {
        // shuffle with the Fisher-Yates algorithm
        // Diyuan: This algo is so cool

        // Removing the basic cards from our deck, it is kinda stupid but effective way
        deck.Remove(1);
        deck.Remove(2);
        deck.Remove(3);

        int indexA = deck.Count;
        while(indexA > 1)
        {
            indexA--;
            int indexB = UnityEngine.Random.Range(0, indexA + 1);
            int temp = deck[indexB];
            deck[indexB] = deck[indexA];
            deck[indexA] = temp;
        }
    }

    private void DrawCard()
    {
        if(deck.Count == 0)
        {
            // shuffle the discard pile into the deck
            deck = new List<int>(discardPile);
            ShuffleDeck();
            discardPile = new List<int>();
        }

        if(deck.Count != 0)
        {
            // if the discard pile was empty, it's still possible that the deck
            // is empty at this point, so the if statement is needed
            int lastIndex = deck.Count - 1;
            int topCard = deck[lastIndex];
            deck.RemoveAt(lastIndex);
            hand.Add(topCard);
        }
    }

    private void DiscardCardAtRandom()
    {
        int discardIndex = UnityEngine.Random.Range(0, hand.Count);
        int discardedCard = hand[discardIndex];
        hand.RemoveAt(discardIndex);
        discardPile.Add(discardedCard);
    }

    private bool CanPlayCard()
    {
        bool isDCCard =
                currentCard.cardType == CardType.DELAYED ||
                currentCard.cardType == CardType.CONTINUOUS;
        if((isDCCard && (dccs.Count < 5)) || !isDCCard)
        {
            // have to be sure to only call TrySpendMana() if the Delayed or
            // Continuous card actually has a space in the DCCS, since the
            // method has a side effect of spending the mana
            return PlayerStats.Instance.TrySpendMana(currentCard.cost);
        }
        else
        {
            // this block will only be entered if the card was Delayed or
            // Continuous, but there wasn't space for it in the DCCS
            return false;
        }
    }

    public void PlayCard()
    {
        Debug.Log("Scanning " + currentCard.name);
        // only if remaining mana is affordable and space exists for Delayed or
        // Continuous cards
        if(CanPlayCard())
        {
            int playedCard = hand[cardIndex];
            hand.RemoveAt(cardIndex);

            switch(currentCard.cardType)
            {
                case CardType.IMMEDIATE:
                    ResolveCardEffects();
                    discardPile.Add(playedCard);
                    break;
                
                case CardType.DELAYED:
                case CardType.CONTINUOUS:
                    // same behavior for both card types
                    Debug.Assert(dccs.Count < Constants.DCCS_SIZE);
                    dccs.Add(new DCCard(playedCard, currentCard.turnsInPlay));
                    break;
                
                case CardType.BASIC:
                default:
                    Debug.Assert(false, "Unknown card type trying to be played!");
                    break;
            }

            // need to do range things, will fix later
            cardIndex = 0;
        }
        else
        {
            Debug.Log("Couldn't play the card for some reason.");
        }
    }

    public void ResolveCardEffects()
    {
        TargettedDamageHandler(targetEnemy);
        AOEDamageHandler();
        HealHandler();
        BlockHandler();
        stunHandler();
        ManaRegenHandler(); 
        // discard before drawing, in case of cards that say discard X, then
        // draw Y
        DiscardHandler();
        DrawHandler();
    }


    private bool ManaRegenHandler() 
    {
        if (currentCard.manaRegen > 0) 
        {
            Debug.Log("Regenerating  " + currentCard.manaRegen + " global mana ");
            PlayerStats.Instance.AddGlobalMana(currentCard.manaRegen);
            return true;
        }
        return false;
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

    private bool stunHandler(){
        if(currentCard.stuns)
        {
            Debug.Log("Stunning the enemy");
            targetEnemy.demoMonster.getStunned();
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
            for(int i = 0; i < currentCard.draw; i++)
            {
                DrawCard();
            }
            return true;
        }
        return false;
    }

    // randomly discard some number of cards 
    private bool DiscardHandler()
    {
        if(currentCard.discard > 0)
        {
            for(int i = 0; i < currentCard.discard; i++)
            {
                if(hand.Count > 0)
                {
                    DiscardCardAtRandom();
                }
            }
            return true;
        }
        return false;
    }

    public void StartPlayerTurn()
    {
        // handle cards in the DCCS
        List<int> cardsToRemove = new List<int>();

        //reset the debuff of the enemies (currently, the stunning effect), I tried many places and found out placing this line here will work.
        targetEnemy.demoMonster.clearEffect();


        for(int i = 0; i < dccs.Count; i++)
        {
            bool cooldownOver = dccs[i].DecrementCounter();
            currentCard = CardDatabase.Instance.GetCardByID(dccs[i].cardID);
            if(currentCard.cardType == CardType.DELAYED)
            {
                if(cooldownOver)
                {
                    ResolveCardEffects();
                }
            }
            else if(currentCard.cardType == CardType.CONTINUOUS)
            {
                ResolveCardEffects();
            }

            if(cooldownOver)
            {
                cardsToRemove.Add(i);
            }
        }
        // iterate backwards across cardsToRemove, so the indices can be
        // guaranteed to be correct
        for(int i = cardsToRemove.Count - 1; i >= 0; i--)
        {
            int removedCard = dccs[cardsToRemove[i]].cardID;
            dccs.RemoveAt(cardsToRemove[i]);
            discardPile.Add(removedCard);
        }
    

        // the player draws a card every turn (except, technically, the first,
        // since nothing will call StartPlayerTurn() at that point)
        DrawCard();
    }

    public void DoEnemyTurn()
    {
        if (!targetEnemy.demoMonster.stunned) {
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

    private void BasicAbility(int id){
        Card temp = currentCard;
        currentCard = CardDatabase.Instance.GetCardByID(id);
        if(CanPlayCard()) {
            ResolveCardEffects();
        }
        currentCard = temp;
    }
    public void BasicAttack() {
        BasicAbility(1);
    }

    public void BasicBlock() {
        BasicAbility(2);
    }
    public void BasicRecover() {
        BasicAbility(3);
    }


    public void IncrementCardNumber()
    {
        cardIndex++;
        if(cardIndex == hand.Count)
        {
            cardIndex = 0;
        }
        // To get rid of OutOfRange
        if(hand.Count != 0) {
            cardID = hand[cardIndex]; 
        } else {
            cardID = 0;
        }
        currentCard = CardDatabase.Instance.GetCardByID(cardID);
    }

    public void DecrementCardNumber()
    {
        cardIndex--;
        if(cardIndex == -1)
        {
            cardIndex = hand.Count - 1;
        }
        // To get rid of OutOfRange
        if(hand.Count != 0) {
            cardID = hand[cardIndex]; 
        } else {
            cardID = 0;
        }
        currentCard = CardDatabase.Instance.GetCardByID(cardID);
    }

    string IntListToString(List<int> list)
    {
        string str="Current Hand: [";
        foreach(int n in list)
        {
            str += n + ", ";
        }
        str += "]";
        return str;
    }
}

