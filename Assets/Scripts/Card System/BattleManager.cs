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

    public int cardID          { get; }
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
    [SerializeField] private GameObject boardManagerObject;
    private IUIManager boardManager;

    // TEMPORARY: Just for testing before UI is implemented, should be removed
    [SerializeField] private Enemy targetEnemy;

    private int enemyPatternIndex;

    public Text handText;
    public Image monsterSwitcherImage;

    //ported from original tempchef.cs
    public Button chefMove;
    public Text chefStatus;
    public int[,] chefBuff;

    void Start()
    {
        // get a reference to the boardManager (we have to do it this way, since
        // interfaces can't be serialized and set in-inspector)
        boardManager = boardManagerObject.GetComponent<IUIManager>();
        // doing this here and in OnEnable(), because OnEnable() runs before
        // Start(), so boardManager will be null at the time; if for some
        // reason, however BattleManager is disabled, then re-enabled, we'll
        // want it in OnEnable() as well
        if(boardManager != null)
        {
            boardManager.CardPlayedEvent += PlayCard;
            boardManager.PlayerTurnEndedEvent += HandlePlayerTurnEnded;
        }

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

        //current buff list [0] = stunned or not 
        // [1] = current block rate in %, the actual damage = damage gonna receive * (1 - block rate)
        // the second value means the duration, for example buff{{1,2 }, {0,0}} means the stun will last for 2 turns
        chefBuff = new int[2, 2] { { 0, 0 }, { 0, 0 } };
    }

    void OnEnable()
    {
        if(boardManager != null)
        {
            boardManager.CardPlayedEvent += PlayCard;
            boardManager.PlayerTurnEndedEvent += HandlePlayerTurnEnded;
        }
    }

    void OnDisable()
    {
        if(boardManager != null)
        {
            boardManager.CardPlayedEvent -= PlayCard;
            boardManager.PlayerTurnEndedEvent -= HandlePlayerTurnEnded;
        }
    }

    void Update()
    {
        showChefStunned();
        chefStatus.text = "Current status: Stunned :" + chefBuff[0, 0] + " Turns remaining:" + chefBuff[0, 1] + "\r\n" + "Blocking rate:" + chefBuff[1, 0] + "%" + " Turns remaining: " + chefBuff[1, 1];
    }

    private void ShuffleDeck()
    {
        // shuffle with the Fisher-Yates algorithm
        // Diyuan: This algo is so cool

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
            boardManager.DrawCard(topCard);
        }
    }

    private void DiscardCardAtRandom()
    {
        int discardIndex = UnityEngine.Random.Range(0, hand.Count);
        int discardedCard = hand[discardIndex];
        hand.RemoveAt(discardIndex);
        discardPile.Add(discardedCard);
        boardManager.RemoveCardFromHand(discardedCard, true);
    }

    private bool CanPlayCard(int cardID)
    {
        Card cardToPlay = CardDatabase.Instance.GetCardByID(cardID);
        bool isDCCard =
                cardToPlay.cardType == CardType.DELAYED ||
                cardToPlay.cardType == CardType.CONTINUOUS;
        if((isDCCard && (dccs.Count < 5)) || !isDCCard)
        {
            // have to be sure to only call TrySpendMana() if the Delayed or
            // Continuous card actually has a space in the DCCS, since the
            // method has a side effect of spending the mana
            return PlayerStats.Instance.TrySpendMana(cardToPlay.cost);
        }
        else
        {
            // this block will only be entered if the card was Delayed or
            // Continuous, but there wasn't space for it in the DCCS
            return false;
        }
    }

    // NOTE: This method used to be public and called directly by a button
    private void PlayCard(int cardID)
    {
        Debug.Log("REEE");
        // only if remaining mana is affordable and space exists for Delayed or
        // Continuous cards
        if(CanPlayCard(cardID))
        {
            // if they could play it, CanPlayCard() spent the mana, so update
            // the UI
            boardManager.UpdatePlayerMana(
                    PlayerStats.Instance.GetMaxGlobalMana(),
                    PlayerStats.Instance.GetGlobalMana()
            );

            hand.Remove(cardID);
            boardManager.RemoveCardFromHand(cardID, false);

            Card cardToPlay = CardDatabase.Instance.GetCardByID(cardID);

            switch(cardToPlay.cardType)
            {
                case CardType.IMMEDIATE:
                    ResolveCardEffects(cardToPlay);
                    discardPile.Add(cardID);
                    break;
                
                case CardType.DELAYED:
                case CardType.CONTINUOUS:
                    // same behavior for both card types
                    Debug.Assert(dccs.Count < Constants.DCCS_SIZE);
                    dccs.Add(new DCCard(cardID, cardToPlay.turnsInPlay));
                    boardManager.PutCardInDCCS(cardID);
                    break;
                
                case CardType.BASIC:
                default:
                    Debug.Assert(false, "Unknown card type (or Basic) trying to be played!");
                    break;
            }
        }
        else
        {
            Debug.Log("Couldn't play the card for some reason.");
        }
    }

    private void ResolveCardEffects(Card card)
    {
        TargettedDamageHandler(card, targetEnemy);
        AOEDamageHandler(card);
        StunHandler(card);
        HealHandler(card);
        ManaRegenHandler(card);
        BlockHandler(card);
        // discard before drawing, in case of cards that say discard X, then draw Y
        DiscardHandler(card);
        DrawHandler(card);
    }

    private bool TargettedDamageHandler(Card card, Enemy targetEnemy)
    {
        if(card.singleDamage > 0)
        {
            Debug.Log("Dealing " + card.singleDamage + " damage to " + targetEnemy.enmName);
            targetEnemy.monsterList[0].DecreaseHP(card.singleDamage);
            if(targetEnemy.monsterList[0].currentHp <= 0)
            {
                boardManager.WinGame();
            }
            return true;
        }
        return false;
    }

    private bool AOEDamageHandler(Card card)
    {
        // placeholder before we have a multi-enemies battle system
        if(card.aoeDamage > 0)
        {
            Debug.Log("Dealing " + card.aoeDamage + " damage to all");
            targetEnemy.monsterList[0].DecreaseHP(card.aoeDamage);
            if(targetEnemy.monsterList[0].currentHp <= 0)
            {
                boardManager.WinGame();
            }
            return true;
        }
        return false;
    }

    private bool StunHandler(Card card)
    {
        if(card.stuns)
        {
            Debug.Log("Stunning the enemy");
            targetEnemy.monsterList[0].getStunned();
            return true;
        }
        return false;
    }

    private bool HealHandler(Card card)
    {
        if(card.heal > 0)
        {
            Debug.Log("Healing the chef by " + card.heal);
            PlayerStats.Instance.ApplyHealing(card.heal);
            boardManager.UpdatePlayerHealth(
                    PlayerStats.Instance.GetMaxHealth(),
                    PlayerStats.Instance.GetHealth()
            );
            return true;
        }
        return false;
    }

    private bool ManaRegenHandler(Card card) 
    {
        if (card.manaRegen > 0) 
        {
            Debug.Log("Regenerating  " + card.manaRegen + " global mana");
            PlayerStats.Instance.AddGlobalMana(card.manaRegen);
            boardManager.UpdatePlayerMana(
                    PlayerStats.Instance.GetMaxGlobalMana(),
                    PlayerStats.Instance.GetGlobalMana()
            );
            return true;
        }
        return false;
    }

    private bool BlockHandler(Card card)
    {
        if(card.blockPercent > 0)
        {
            if(card.cardType == CardType.IMMEDIATE )
            {
                Debug.Log("current turn, Blocking vlaue =  " + card.blockPercent);
                chefBlocking((int)(card.blockPercent * 100),1);
                return true;
            }
            else
            {
                Debug.Log(card.turnsInPlay + "turn, Blocking vlaue =  " + card.blockPercent);
                chefBlocking((int)(card.blockPercent * 100), card.turnsInPlay);
                return true;
            }
        }
        return false;
    }

    private bool DrawHandler(Card card)
    {
        if(card.draw > 0)
        {
            for(int i = 0; i < card.draw; i++)
            {
                DrawCard();
            }
            return true;
        }
        return false;
    }

    // randomly discard some number of cards 
    private bool DiscardHandler(Card card)
    {
        if(card.discard > 0)
        {
            for(int i = 0; i < card.discard; i++)
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

    // NOTE: This method used to be public and called directly by a button
    private void StartPlayerTurn()
    {
        // handle cards in the DCCS
        List<int> cardsToRemove = new List<int>();

        for(int i = 0; i < dccs.Count; i++)
        {
            bool cooldownOver = dccs[i].DecrementCounter();
            Card dccsCard = CardDatabase.Instance.GetCardByID(dccs[i].cardID);
            if(dccsCard.cardType == CardType.DELAYED)
            {
                if(cooldownOver)
                {
                    ResolveCardEffects(dccsCard);
                }
            }
            else if(dccsCard.cardType == CardType.CONTINUOUS)
            {
                ResolveCardEffects(dccsCard);
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
            boardManager.RemoveCardFromDCCS(removedCard);
        }

        //reset the debuff of the enemies (currently, the stunning effect), I tried many places and found out placing this line here will work.
        targetEnemy.monsterList[0].clearEffect();

        // reactivate the player's basic abilities for the turn, in case they
        // used them last turn
        boardManager.ActivateBasicAbilities();

        // the player draws a card every turn (except, technically, the first,
        // since nothing will call StartPlayerTurn() at that point)
        DrawCard();
    }

    private void HandlePlayerTurnEnded()
    {
        DoEnemyTurn();
        StartPlayerTurn();
    }
    
    private void MakeTransparent()
    {
        monsterSwitcherImage.GetComponent<Image>().color = new Color(255,255,255,0);
    }

    // NOTE: This method used to be public and called directly by a button
    private void DoEnemyTurn()
    {
        monsterSwitcherImage.GetComponent<Image>().color = new Color(255,255,255,255);
        Invoke("MakeTransparent", 1);

        if (!targetEnemy.monsterList[0].stunned)
        {
            int curAction = targetEnemy.monsterList[0].actionpattern[enemyPatternIndex];
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
                    Debug.Log("Invalid behavior in monster: " + targetEnemy.monsterList[0].name);
                    break;
            }

            enemyPatternIndex += 1;
            if(enemyPatternIndex == targetEnemy.monsterList[0].actionpattern.Length)
            {
                enemyPatternIndex = 0;
            }
        } 
        buffupdate();

    }

    private void HandleEnemyAttack()
    {
        Debug.Log("og damgade:"+targetEnemy.monsterList[0].basicAtt );
        int damagedealed = targetEnemy.monsterList[0].basicAtt * (100 - chefBuff[1,0])/100;
        Debug.Log("percentage:"+ chefBuff[1,0] );
        PlayerStats.Instance.ApplyDamage(damagedealed);
        boardManager.UpdatePlayerHealth(
                PlayerStats.Instance.GetMaxHealth(),
                PlayerStats.Instance.GetHealth()
        );
        if(PlayerStats.Instance.GetHealthAsPercentage() <= 0.0f)
        {
            boardManager.LoseGame();
        }
    }

    private void HandleEnemySpecialSkill()
    {
        // turns plus 1 because the counter will decrease once enemy finish its move
        chefGetStunned(targetEnemy.monsterList[0].skilleffect +1);
        Debug.Log(targetEnemy.monsterList[0].name + " spelled its skill." +targetEnemy.monsterList[0].skilleffect);
    }

    private void BasicAbility(int id)
    {
        if(CanPlayCard(id))
        {
            // if they could play it, CanPlayCard() spent the mana, so update
            // the UI
            boardManager.UpdatePlayerMana(
                    PlayerStats.Instance.GetMaxGlobalMana(),
                    PlayerStats.Instance.GetGlobalMana()
            );

            ResolveCardEffects(CardDatabase.Instance.GetCardByID(id));
            boardManager.DeactivateBasicAbilities();
        }
    }

    // NOTE: These methods are currently called directly by buttons on screen,
    //       they should probably be replaced by BasicAbility being called in
    //       response to some IUIManager event (though that may be overkill, who
    //       knows)
    public void BasicAttack()
    {
        BasicAbility(1);
    }

    public void BasicBlock()
    {
        BasicAbility(2);
    }

    public void BasicRecover()
    {
        BasicAbility(3);
    }

    // tempchef
    // easy to read since we don't have much status now
    private void chefGetStunned(int turns)
    {
        chefBuff[0, 0] = 1;
        chefBuff[0, 1] = turns;
    }

    private void chefBlocking(int blockrate, int turns)
    {
        chefBuff[1, 0] = blockrate;
        chefBuff[1, 1] = turns;
    }

    // to update as turns pass, once the remaining turns for one status become 0, set the value of that status to 0 to neutralize the buff.
    private void buffupdate()
    {
        // go through all the status we have, and decrease the turns number
        for (int i = 0; i < chefBuff.GetLength(0); i++)
        {
            chefBuff[i, 1] -= 1;
            if (chefBuff[i, 1] <= 0)
            {
                chefBuff[i, 1] = 0;
                chefBuff[i, 0] = 0;
            }
        }
    }

    private void showChefStunned()
    {
        if(chefBuff[0, 0] != 0)
        {
            chefMove.interactable = false;
        }
        else
        {
            chefMove.interactable = true;
        }
    }

}

