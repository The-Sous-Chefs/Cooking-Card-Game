using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUIManager : MonoBehaviour, IUIManager
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
    private List<int> cardsInHand;
    private Dictionary<int, (int cardID, int countDown)> dccs;
    private int currentCardIndex;
    private int numCardsInDeck;
    private int numCardsInDiscardPile;
    private bool playerStunned;

    // variables used only to display information to the player
    [SerializeField] private Text chefHPText;
    [SerializeField] private Text chefManaText;
    [SerializeField] private Text chefBlockText;
    [SerializeField] private Text handListText;
    [SerializeField] private Text deckSizeText;
    [SerializeField] private Text discardPileSizeText;
    [SerializeField] private Text dccsContentsText;
    [SerializeField] private Text selectedCardText;

    [SerializeField] private Button incrementButton;
    [SerializeField] private Button decrementButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button basicAttackButton;
    [SerializeField] private Button basicBlockButton;
    [SerializeField] private Button basicManaRefreshButton;

    [SerializeField] private Image stunIndicatorImage;
    [SerializeField] private Image monsterSwitcherImage;

    [SerializeField] private GameObject winMessage;
    [SerializeField] private GameObject loseMessage;

    // NOTE: Using Awake() instead of Start(), since the BattleManager gets a
    //       reference to a IUIManager in it's Start() method, and since Awake()
    //       always runs before Start(), we're guaranteed it will be initialized
    //       in BattleManager's Start().
    void Awake()
    {
        cardsInHand = new List<int>();
        dccs = new Dictionary<int, (int cardID, int countDown)>();

        currentCardIndex = -1;
        SetHandButtonsActive(false);
        numCardsInDeck = PlayerStats.Instance.GetCollectedCardIDs().Count;
        numCardsInDiscardPile = 0;
        playerStunned = false;

        chefHPText.text = "HP: " + PlayerStats.Instance.GetHealth();
        chefManaText.text = "Mana: " + PlayerStats.Instance.GetGlobalMana();
        chefBlockText.text = "Blocking 0% of damage from enemies.";
        stunIndicatorImage.enabled = false;
        deckSizeText.text = "0";
        discardPileSizeText.text = "0";
        UpdateDCCSContents();
    }

    public void IncrementCurrentCard()
    {
        currentCardIndex++;
        if(currentCardIndex >= cardsInHand.Count)
        {
            currentCardIndex = 0;
        }
        UpdateSelectedCardText();
    }

    public void DecrementCurrentCard()
    {
        currentCardIndex--;
        if(currentCardIndex < 0)
        {
            currentCardIndex = cardsInHand.Count - 1;
        }
        UpdateSelectedCardText();
    }

    public void pickClickedCurrentCard(int id)
    {
        //Debug.Assert(cardsInHand.Contains(id));
        currentCardIndex = cardsInHand.IndexOf(id);
        Debug.Log("card id "+id+" selected because of dragging");
    }

    public void PlayCurrentCard()
    {
        Debug.Assert((currentCardIndex >= 0) && (currentCardIndex < cardsInHand.Count));
        if(CardPlayedEvent != null)
        {
            CardPlayedEvent(cardsInHand[currentCardIndex]);
        }
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

    /*public void PlayCardById(int id)
    {
        Debug.Assert(cardsInHand.Contains(id));
        if (CardPlayedEvent != null)
        {
            CardPlayedEvent(id);
        }
    }*/

    private void MakeMonsterSwitcherTransparent()
    {
        monsterSwitcherImage.color = new Color(255, 255, 255, 0);
    }

    public void EndPlayerTurn()
    {
        monsterSwitcherImage.color = new Color(255, 255, 255, 255);
        Invoke("MakeMonsterSwitcherTransparent", 1);
        if(PlayerTurnEndedEvent != null)
        {
            PlayerTurnEndedEvent();
        }
    }

    public void RestartBattle()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    private void UpdateSelectedCardText()
    {
        Card selectedCard = CardDatabase.Instance.GetCardByID(cardsInHand[currentCardIndex]);
        string lineOne = "Selected Card Index: " + currentCardIndex + "\n";
        string lineTwo = "Card Name: " + selectedCard.name + ", Cost: " + selectedCard.cost + "\n";
        string lineThree = "Card Text: " + selectedCard.cardText;
        selectedCardText.text = lineOne + lineTwo + lineThree;
    }

    private void UpdateHandList()
    {
        string handList = "";
        for(int i = 0; i < cardsInHand.Count; i++)
        {
            handList += i + ":\n";
            handList += CardDatabase.Instance.GetCardByID(cardsInHand[i]).ToString() + "\n\n";
        }
        handListText.text = handList;
    }

    private void UpdateDCCSContents()
    {
        string dccsList = "";
        for(int i = 0; i < Constants.DCCS_SIZE; i++)
        {
            dccsList += i + ":\n";
            if(dccs.ContainsKey(i))
            {
                dccsList += CardDatabase.Instance.GetCardByID(dccs[i].cardID).ToString() + "\n";
                dccsList += "Countdown: " + dccs[i].countDown + "\n\n";
            }
            else
            {
                dccsList += "No card in this slot.\n\n";
            }
        }
        dccsContentsText.text = dccsList;
    }

    private void SetHandButtonsActive(bool enabled)
    {
        incrementButton.interactable = playerStunned ? false : enabled;
        decrementButton.interactable = playerStunned ? false : enabled;
        playButton.interactable = playerStunned ? false : enabled;
        
        if(!playerStunned && enabled)
        {
            UpdateSelectedCardText();
        }
    }

    //-------------------
    // IUIManager methods
    //-------------------

    public void UpdatePlayerHealth(int maxHealth, int currentHealth)
    {
        chefHPText.text = "HP: " + currentHealth;
    }

    public void UpdatePlayerMana(int maxMana, int currentMana)
    {
        chefManaText.text = "Mana: " + currentMana;
    }

    public void UpdatePlayerBlockPercent(float blockPercent)
    {
        chefBlockText.text = "Blocking " + (blockPercent * 100) + "% of damage from enemies.";
    }

    public void UpdatePlayerStunStatus(bool stunned)
    {
        playerStunned = stunned;
        if(currentCardIndex != -1)
        {
            SetHandButtonsActive(!playerStunned);
        }
        stunIndicatorImage.enabled = playerStunned;
    }

    public void DrawCard(int cardID)
    {
        cardsInHand.Add(cardID);
        numCardsInDeck--;

        UpdateHandList();

        deckSizeText.text = numCardsInDeck.ToString();

        if(currentCardIndex == -1)
        {
            currentCardIndex = 0;
            SetHandButtonsActive(true);
        }
    }

    public void RemoveCardFromHand(int cardID, bool discarded)
    {
        Debug.Assert(cardsInHand.Contains(cardID));
        cardsInHand.Remove(cardID);

        UpdateHandList();

        Card playedCard = CardDatabase.Instance.GetCardByID(cardID);
        if(discarded || (playedCard.cardType == CardType.IMMEDIATE))
        {
            PutCardInDiscardPile(cardID, false);
        }

        if(currentCardIndex >= cardsInHand.Count)
        {
            if(cardsInHand.Count == 0)
            {
                currentCardIndex = -1;
                SetHandButtonsActive(false);
                // don't UpdateSelectedCardText() in this case, since the index
                // will be -1
                selectedCardText.text = "Your hand is empty...";
            }
            else
            {
                currentCardIndex = cardsInHand.Count - 1;
                UpdateSelectedCardText();
            }
        }
        else
        {
            UpdateSelectedCardText();
        }
    }

    public void PutCardInDiscardPile(int cardID, bool fromDeck)
    {
        numCardsInDiscardPile++;
        discardPileSizeText.text = numCardsInDiscardPile.ToString();
        if(fromDeck)
        {
            numCardsInDeck--;
            deckSizeText.text = numCardsInDeck.ToString();
        }
    }

    public void PutCardInDCCS(int cardID, int countDown, int dccsSlot)
    {
        Debug.Assert((dccsSlot >= 0) && (dccsSlot < Constants.DCCS_SIZE));
        dccs.Add(dccsSlot, (cardID, countDown));

        UpdateDCCSContents();
    }

    public void RemoveCardFromDCCS(int dccsSlot)
    {
        Debug.Assert((dccsSlot >= 0) && (dccsSlot < Constants.DCCS_SIZE));
        Debug.Assert(dccs.ContainsKey(dccsSlot));
        dccs.Remove(dccsSlot);

        UpdateDCCSContents();

        numCardsInDiscardPile++;

        discardPileSizeText.text = numCardsInDiscardPile.ToString();
    }

    public void UpdateDCCSCount(int dccsSlot, int newCountDown)
    {
        Debug.Assert((dccsSlot >= 0) && (dccsSlot < Constants.DCCS_SIZE));
        Debug.Assert(dccs.ContainsKey(dccsSlot));
        int cardID = dccs[dccsSlot].cardID;
        int oldCountDown = dccs[dccsSlot].countDown;
        dccs[dccsSlot] = (cardID, oldCountDown - 1);

        UpdateDCCSContents();
    }

    public void DeactivateBasicAbilities()
    {
        basicAttackButton.interactable = false;
        basicBlockButton.interactable = false;
        basicManaRefreshButton.interactable = false;
    }

    public void ActivateBasicAbilities()
    {
        basicAttackButton.interactable = true;
        basicBlockButton.interactable = true;
        basicManaRefreshButton.interactable = true;
    }

    public void UpdateEnemyHealth(int enemyID, int maxHealth, int currentHealth)
    {
        //
    }

    public void RemoveEnemy(int enemyID)
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
}
