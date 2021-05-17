using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUIManager : MonoBehaviour, IUIManager
{
    //-----------------
    // member variables
    //-----------------

    // variables the UI needs to keep track of to work
    [SerializeField] private BattleManager battleManager;
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

    // NOTE: This TestUIManager is essentially hard-coded to only support one
    //       enemy. AddEnemy() ignore the monsterID and just updates the text
    //       fields of the TestEnemy below. Additionally, RemoveEnemy() does
    //       nothing, since there's no point in removing the only enemy, since
    //       the game will end when its health reaches 0. Also, UpdateEnemyHealth()
    //       also ignores the monsterID and just updates the HP text field of
    //       the TestEnemy below. Finally, UpdateEnemyStunStatus() also ignores
    //       the monsterID and just updates the stunned appearance of the
    //       TestEnemy below.
    [SerializeField] private TestEnemy enemy;

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
        numCardsInDeck = 0;
        numCardsInDiscardPile = 0;
        playerStunned = false;

        chefHPText.text = "HP: #";
        chefManaText.text = "Mana: #";
        chefBlockText.text = "Blocking 0% of damage from enemies.";
        deckSizeText.text = numCardsInDeck.ToString();
        discardPileSizeText.text = numCardsInDiscardPile.ToString();
        stunIndicatorImage.enabled = false;
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

    // public void pickClickedCurrentCard(int id)
    // {
    //     //Debug.Assert(cardsInHand.Contains(id));
    //     currentCardIndex = cardsInHand.IndexOf(id);
    //     Debug.Log("card id "+id+" selected because of dragging");
    // }

    public void PlayCurrentCard()
    {
        Debug.Assert((currentCardIndex >= 0) && (currentCardIndex < cardsInHand.Count));
        if(battleManager != null)
        {
            battleManager.PlayCard(cardsInHand[currentCardIndex], 0);
        }
    }

    // NOTE: This method should only be passed the ID of the basic abilities,
    //       because if it gets any other CardID, it will just result in that
    //       resolve that card having its effects resolved.
    public void UseBasicAbility(int abilityID)
    {
        Debug.Assert(CardDatabase.Instance.GetBasicAbilityIDs().Contains(abilityID));
        if(battleManager != null)
        {
            battleManager.PlayCard(abilityID, Constants.NO_TARGET);
        }
    }

    private void MakeMonsterSwitcherTransparent()
    {
        monsterSwitcherImage.color = new Color(255, 255, 255, 0);
    }

    public void EndPlayerTurn()
    {
        monsterSwitcherImage.color = new Color(255, 255, 255, 255);
        Invoke("MakeMonsterSwitcherTransparent", 1);
        if(battleManager != null)
        {
            battleManager.HandleEndOfPlayerTurn();
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
        chefHPText.text = "HP: " + currentHealth + " / " + maxHealth;
    }

    public void UpdatePlayerMana(int maxMana, int currentMana)
    {
        chefManaText.text = "Mana: " + currentMana + " / " + maxMana;
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

    // add a card to the player's hand and update the visuals accordingly
    public void PutCardInHand(int cardID)
    {
        cardsInHand.Add(cardID);

        UpdateHandList();

        if(currentCardIndex == -1)
        {
            currentCardIndex = 0;
            SetHandButtonsActive(true);
            
            // FIXME: Shouldn't this be called here!?
            // UpdateSelectedCardText();
        }
    }

    // remove the card from the player's hand and update the visuals accordingly
    public void RemoveCardFromHand(int cardID)
    {
        Debug.Assert(cardsInHand.Contains(cardID));
        cardsInHand.Remove(cardID);

        UpdateHandList();

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

    // add a card to the players deck and update the visuals accordingly
    public void PutCardInDeck(int cardID)
    {
        numCardsInDeck++;
        deckSizeText.text = numCardsInDeck.ToString();
    }

    public void RemoveCardFromDeck(int cardID)
    {
        numCardsInDeck--;
        deckSizeText.text = numCardsInDeck.ToString();
    }

    public void PutCardInDiscardPile(int cardID)
    {
        numCardsInDiscardPile++;
        discardPileSizeText.text = numCardsInDiscardPile.ToString();
    }

    public void RemoveCardFromDiscardPile(int cardID)
    {
        numCardsInDiscardPile--;
        discardPileSizeText.text = numCardsInDiscardPile.ToString();
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
    }

    public void UpdateDCCSCount(int dccsSlot, int newCountDown)
    {
        Debug.Assert((dccsSlot >= 0) && (dccsSlot < Constants.DCCS_SIZE));
        Debug.Assert(dccs.ContainsKey(dccsSlot));
        dccs[dccsSlot] = (dccs[dccsSlot].cardID, dccs[dccsSlot].countDown - 1);

        UpdateDCCSContents();
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

    // see the big comment by the TestEnemy member variable
    public void AddEnemy(int monsterID, Monster monster)
    {
        enemy.SetNameText(monster.name);
        enemy.SetHPText(monster.maxHP, monster.currentHP);
    }

    // see the big comment by the TestEnemy member variable
    public void RemoveEnemy(int monsterID)
    {
        // do nothing, this UI doesn't really need to worry about it since it
        // just has one enemy
    }

    // see the big comment by the TestEnemy member variable
    public void UpdateEnemyHealth(int monsterID, int maxHealth, int currentHealth)
    {
        enemy.SetHPText(maxHealth, currentHealth);
    }

    // see the big comment by the TestEnemy member variable
    public void UpdateEnemyStunStatus(int monsterID, bool stunned)
    {
        enemy.ToggleStunned(stunned);
    }

    /*
     * This method must somehow cause the backend to call RunNextEnemyTurn().
     * The backend will call this method to play the animation for the enemy's
     * turn (if there is one), then when the animation is over, it's relying on
     * something calling RunNextEnemyTurn() to actually update the state of the
     * game based on that enemy's action and start the next enemy turn if there
     * is one or start the player's next turn.
     *
     * This UI doesn't have animations, so it simply calls RunNextEnemyTurn().
     */
    public void PlayEnemyTurnAnimation(int monsterID, MonsterAction action)
    {
        battleManager.RunNextEnemyTurn();
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
