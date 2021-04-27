using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BoardManager : MonoBehaviour, IUIManager
{
    //-------
    // events
    //-------

    public event CardPlayedDelegate CardPlayedEvent;
    public event BasicAbilityUsedDelegate BasicAbilityUsedEvent;
    public event PlayerTurnEndedDelgate PlayerTurnEndedEvent;

    //-----------------
    // member variables
    //-----------------

    // variables the UI needs to keep track of to work
    [SerializeField] private GameObject card;
    private List<Card> hand;
    private int numCardsInDeck;
    private int numCardsInDiscardPile;
    private bool playerStunned;

    // variables used only to display information to the player
    [SerializeField] private Text chefHPText;
    [SerializeField] private Text chefManaText;
    [SerializeField] private Text chefBlockText;
    [SerializeField] private Text deckCountText;
    [SerializeField] private Text discardPileCountText;

    [SerializeField] private Button basicAttackButton;
    [SerializeField] private Button basicBlockButton;
    [SerializeField] private Button basicManaRefreshButton;

    [SerializeField] private Image stunIndicatorImage;

    [SerializeField] private GameObject winMessage;
    [SerializeField] private GameObject loseMessage;

    [SerializeField] private Transform handContainer;
    [SerializeField] private Canvas canvas;

    void Awake()
    {
        Debug.Assert(card != null);

        hand = new List<Card>();

        numCardsInDeck = 0;
        numCardsInDiscardPile = 0;
        playerStunned = false;

        chefHPText.text = "HP: #";
        chefManaText.text = "Mana: #";
        chefBlockText.text = "Blocking 0% of damage from enemies.";
        deckCountText.text = numCardsInDeck.ToString();
        discardPileCountText.text = numCardsInDiscardPile.ToString();
        stunIndicatorImage.enabled = false;
    }

    // NOTE: This method should only be passed the ID of the basic abilities,
    //       because if it gets any other CardID, it will just result in that
    //       resolve that card having its effects resolved.
    public void UseBasicAbility(int abilityID)
    {
        Debug.Assert(CardDatabase.Instance.GetBasicAbilityIDs().Contains(abilityID));
        if(BasicAbilityUsedEvent != null)
        {
            BasicAbilityUsedEvent(abilityID);
        }
    }

    public void EndPlayerTurn()
    {
        if(PlayerTurnEndedEvent != null)
        {
            PlayerTurnEndedEvent();
        }
    }

    public void RestartBattle()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //-------------------
    // IUIManager methods
    //-------------------

    public void UpdatePlayerHealth(int maxHealth, int currentHealth)
    {
        chefHPText.text = "HP: " + currentHealth + " / " + maxHealth;
    }

    public void UpdatePlayerMana(int maxMana, int currentMana)
    {
        chefManaText.text = "Mana: " + currentMana + " / " + maxMana;
    }

    public void UpdatePlayerBlockPercent(float blockPercent)
    {
        chefBlockText.text = "Blocking " + (blockPercent * 100) + "% of damage";
    }

    public void UpdatePlayerStunStatus(bool stunned) {
        playerStunned = stunned;
        stunIndicatorImage.enabled = playerStunned;
    }

    // NOTE: Use the line below to instantiate a card.
    // GameObject cardObject = Instantiate(card, handContainer);
    public void PutCardInHand(int cardId)
    {
        GameObject newCardInUI = Instantiate(card, handContainer);
        newCardInUI.GetComponent<DragDrop>().canvas = canvas;
        newCardInUI.GetComponent<CardUI>().CreateCard(cardId);
    }

    public void RemoveCardFromHand(int cardID)
    {
        //
    }

    public void PutCardInDeck(int cardID)
    {
        numCardsInDeck++;
        deckCountText.text = numCardsInDeck.ToString();
    }

    public void RemoveCardFromDeck(int cardID)
    {
        numCardsInDeck--;
        deckCountText.text = numCardsInDeck.ToString();
    }

    public void PutCardInDiscardPile(int cardID)
    {
        numCardsInDiscardPile++;
        discardPileCountText.text = numCardsInDiscardPile.ToString();
    }

    public void RemoveCardFromDiscardPile(int cardID)
    {
        numCardsInDiscardPile--;
        discardPileCountText.text = numCardsInDiscardPile.ToString();
    }

    public void PutCardInDCCS(int cardID, int countDown, int dccsSlot)
    {
        //
    }

    public void RemoveCardFromDCCS(int dccsSlot)
    {
        //
    }

    public void UpdateDCCSCount(int dccsSlot, int newCountDown)
    {
        //
    }

    public void ActivateBasicAbilities()
    {
        basicAttackButton.interactable = true;
        basicBlockButton.interactable = true;
        basicManaRefreshButton.interactable = true;
    }

    public void DeactivateBasicAbilities()
    {
        basicAttackButton.interactable = false;
        basicBlockButton.interactable = false;
        basicManaRefreshButton.interactable = false;
    }

    public void AddEnemy(int monsterID, Monster monster)
    {
        //
    }

    public void RemoveEnemy(int monsterID)
    {
        //
    }

    public void UpdateEnemyHealth(int monsterID, int maxHealth, int currentHealth)
    {
        //
    }

    public void UpdateEnemyStunStatus(int monsterID, bool stunned)
    {
        //
    }

    public void WinGame()
    {
        winMessage.SetActive(true);
    }

    public void LoseGame()
    {
        loseMessage.SetActive(true);
    }

    public void playCardByID(int cardId)
    {
        CardPlayedEvent(cardId);
    }
}
