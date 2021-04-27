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
    // only serialized for debugging purposes
    [SerializeField] private List<int> deck;
    [SerializeField] private List<int> discardPile;
    [SerializeField] private List<int> hand;
    private Dictionary<int, DCCard> dccs;
    private bool basicAbilityUsed;
    private float blockPercent;
    private int stunnedTurns;
    [SerializeField] private GameObject boardManagerObject;
    private IUIManager boardManager;

    // TEMPORARY: Just for testing before UI is implemented, should be removed
    [SerializeField] private Enemy targetEnemy;

    private int enemyPatternIndex;

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
            boardManager.BasicAbilityUsedEvent += PlayCard;
            boardManager.PlayerTurnEndedEvent += HandleEndOfPlayerTurn;
        }

        // initialize the deck, discardPile, hand, and DCCS
        deck = PlayerStats.Instance.GetCollectedCardIDs();
        foreach(int card in deck)
        {
            boardManager.PutCardInDeck(card);
        }
        ShuffleDeck();
        discardPile = new List<int>();
        hand = new List<int>();
        dccs = new Dictionary<int, DCCard>();

        // draw the player's opening hand
        for(int i = 0; i < Constants.STARTING_HAND_SIZE; i++)
        {
            DrawCard();
        }
        // debugging card: put the card would like to test here
        // int testCardID = 1;
        // hand.Add(testCardID);
        // boardManager.PutCardInHand(testCardID)

        // initialize basicAbilityUsed to false, block percentage to 0%,
        // and stunned status to not stunned
        basicAbilityUsed = false;
        blockPercent = 0.0f;
        stunnedTurns = 0;

        // NOTE: If we want the player to draw a card on the first turn or just
        //       have the start of the first turn treated like any other turn,
        //       we can uncomment the line below (if we do, we can get rid of
        //       "blockPercent = 0.0f;" above)
        // StartPlayerTurn();
    }

    void OnEnable()
    {
        if(boardManager != null)
        {
            boardManager.CardPlayedEvent += PlayCard;
            boardManager.BasicAbilityUsedEvent += PlayCard;
            boardManager.PlayerTurnEndedEvent += HandleEndOfPlayerTurn;
        }
    }

    void OnDisable()
    {
        if(boardManager != null)
        {
            boardManager.CardPlayedEvent -= PlayCard;
            boardManager.BasicAbilityUsedEvent -= PlayCard;
            boardManager.PlayerTurnEndedEvent -= HandleEndOfPlayerTurn;
        }
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
            discardPile = new List<int>();
            foreach(int card in deck)
            {
                boardManager.RemoveCardFromDiscardPile(card);
                boardManager.PutCardInDeck(card);
            }
            ShuffleDeck();
        }

        if(deck.Count > 0)
        {
            // if the discard pile was empty, it's still possible that the deck
            // is empty at this point, so the if statement is needed
            int lastIndex = deck.Count - 1;
            int topCard = deck[lastIndex];
            deck.RemoveAt(lastIndex);
            boardManager.RemoveCardFromDeck(topCard);
            if(hand.Count < Constants.MAX_HAND_SIZE)
            {
                hand.Add(topCard);
                boardManager.PutCardInHand(topCard);
            }
            else
            {
                discardPile.Add(topCard);
                boardManager.PutCardInDiscardPile(topCard);
            }
        }
    }

    private void DiscardCardAtRandom()
    {
        int discardIndex = UnityEngine.Random.Range(0, hand.Count);
        int discardedCard = hand[discardIndex];
        hand.RemoveAt(discardIndex);
        boardManager.RemoveCardFromHand(discardedCard);
        discardPile.Add(discardedCard);
        boardManager.PutCardInDiscardPile(discardedCard);
    }

    private void AddCardToDCCS(int cardID)
    {
        Debug.Assert(dccs.Count < Constants.DCCS_SIZE, "Tried to add a card to DCCS when it was full!");
        int slot = 0;
        bool foundSlot = false;
        while((slot < Constants.DCCS_SIZE) && !foundSlot)
        {
            if(!dccs.ContainsKey(slot))
            {
                foundSlot = true;
                Card dcCard = CardDatabase.Instance.GetCardByID(cardID);
                dccs.Add(slot, new DCCard(cardID, dcCard.turnsInPlay));
                boardManager.PutCardInDCCS(cardID, dcCard.turnsInPlay, slot);
            }
            slot++;
        }
    }

    private bool CanPlayCard(int cardID)
    {
        Card cardToPlay = CardDatabase.Instance.GetCardByID(cardID);
        bool isDCCard =
                cardToPlay.cardType == CardType.DELAYED ||
                cardToPlay.cardType == CardType.CONTINUOUS;
        bool isBasicAbility = cardToPlay.cardType == CardType.BASIC;
        if((isDCCard && (dccs.Count >= Constants.DCCS_SIZE)) ||
                (!isBasicAbility && (stunnedTurns > 0)))
        {
            // we only want to return false without checking cost if the card is
            // 1) Delayed or Continuous and the DCCS doesn't have space, or
            // 2) not a Basic ability and th eplayer is stunned for the turn
            Debug.Log("Can't play " + cardToPlay + " for a reason other than mana");
            return false;
        }
        else
        {
            // have to be sure to only call TrySpendMana() if the card can be
            // played in the first place, since the method has a side effect of
            // spending the mana
            // NOTE: That should maybe be fixed.
            return PlayerStats.Instance.TrySpendMana(cardToPlay.cost);
        }
    }

    // NOTE: This method used to be public and called directly by a button
    private void PlayCard(int cardID)
    {
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

            Card cardToPlay = CardDatabase.Instance.GetCardByID(cardID);

            switch(cardToPlay.cardType)
            {
                case CardType.IMMEDIATE:
                    hand.Remove(cardID);
                    boardManager.RemoveCardFromHand(cardID);
                    ResolveCardEffects(cardToPlay);
                    discardPile.Add(cardID);
                    boardManager.PutCardInDiscardPile(cardID);
                    break;
                
                case CardType.DELAYED:
                case CardType.CONTINUOUS:
                    // same behavior for both card types
                    hand.Remove(cardID);
                    boardManager.RemoveCardFromHand(cardID);
                    AddCardToDCCS(cardID);
                    break;
                
                case CardType.BASIC:
                    if(!basicAbilityUsed)
                    {
                        // nothing to do related to the hand, but basic abilities
                        // are implemented just like any other card in terms of
                        // effects
                        ResolveCardEffects(cardToPlay);
                        basicAbilityUsed = true;
                        boardManager.DeactivateBasicAbilities();
                    }
                    break;

                default:
                    Debug.Assert(false, "Unknown card type trying to be played!");
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
            blockPercent += card.blockPercent;
            boardManager.UpdatePlayerBlockPercent(blockPercent);
            return true;
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
        // NOTE: update any player status first, since DCCS may affect it and we
        //       don't want to undo that stuff

        // reactivate the player's basic abilities for the turn
        if(basicAbilityUsed)
        {
            basicAbilityUsed = false;
            boardManager.ActivateBasicAbilities();
        }

        // reset block percentage
        blockPercent = 0.0f;
        boardManager.UpdatePlayerBlockPercent(blockPercent);

        // handle being stunned (EndPlayerTurn() will handle decrementing it)
        if(stunnedTurns == 0)
        {
            // NOTE: This will be called every turn the player isn't stunned,
            //       not just the first turn they stop being stunned.
            boardManager.UpdatePlayerStunStatus(false);
        }

        // handle cards in the DCCS
        List<int> cardsToRemove = new List<int>();

        for(int i = 0; i < Constants.DCCS_SIZE; i++)
        {
            if(dccs.ContainsKey(i))
            {
                bool cooldownOver = dccs[i].DecrementCounter();
                boardManager.UpdateDCCSCount(i, dccs[i].cooldownCounter);
                Card dccsCard = CardDatabase.Instance.GetCardByID(dccs[i].cardID);
                Debug.Assert(
                        (dccsCard.cardType == CardType.DELAYED) ||
                        (dccsCard.cardType == CardType.CONTINUOUS)
                );
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
        }

        for(int i = 0; i < cardsToRemove.Count; i++)
        {
            int removedCard = dccs[cardsToRemove[i]].cardID;
            dccs.Remove(cardsToRemove[i]);
            boardManager.RemoveCardFromDCCS(cardsToRemove[i]);
            discardPile.Add(removedCard);
            boardManager.PutCardInDiscardPile(removedCard);
        }

        //reset the debuff of the enemies (currently, the stunning effect), I tried many places and found out placing this line here will work.
        targetEnemy.monsterList[0].clearEffect();

        // the player draws a card every turn (except, technically, the first,
        // since nothing will call StartPlayerTurn() at that point)
        DrawCard();
    }

    private void EndPlayerTurn()
    {
        // if the player was stunned this turn, decrement the counter; doing
        // this here, since it might be confusing to see the number to go to 0
        // at the start of the turn, but still be stunned
        if(stunnedTurns > 0)
        {
            stunnedTurns--;
        }
    }

    private void HandleEndOfPlayerTurn()
    {
        EndPlayerTurn();
        DoEnemyTurn();
        StartPlayerTurn();
    }

    // NOTE: This method used to be public and called directly by a button
    private void DoEnemyTurn()
    {
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
    }

    private void HandleEnemyAttack()
    {
        int actualDamage = (int) (((float) targetEnemy.monsterList[0].basicAtt) * (1.0f - blockPercent));
        if(actualDamage >= 0f)
        {
            // apply actualDamage to the player
            PlayerStats.Instance.ApplyDamage(actualDamage);
            boardManager.UpdatePlayerHealth(
                    PlayerStats.Instance.GetMaxHealth(),
                    PlayerStats.Instance.GetHealth()
            );
            if(PlayerStats.Instance.GetHealth() <= 0)
            {
                boardManager.LoseGame();
            }
        }
        else
        {
            // actualDamage is the negative of the excess block percentage times
            // the original damage, so apply that damage to the enemy
            targetEnemy.monsterList[0].DecreaseHP(actualDamage * -1);
            if(targetEnemy.monsterList[0].currentHp <= 0)
            {
                boardManager.WinGame();
            }
        }
    }

    private void HandleEnemySpecialSkill()
    {
        // NOTE: This method assumes the only special skill is stunning the
        //       player--which may be the case.
        Debug.Assert(targetEnemy.monsterList[0].skilleffect > 0);
        // add turns, just in case stuns get applied one after another
        stunnedTurns += targetEnemy.monsterList[0].skilleffect;
        boardManager.UpdatePlayerStunStatus(true);
    }
}

