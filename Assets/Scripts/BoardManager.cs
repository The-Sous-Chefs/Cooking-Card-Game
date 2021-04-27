using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour, IUIManager
{
    public List<int> deck;
    public List<int> discardPile;
    public List<Card> hand;
    public GameObject card;
    [SerializeField] private GameObject boardManagerObject;
    private IUIManager boardManager;
    public HealthBar playerHealthBar;
    public HealthBar enemyHealthBar;
    public ManaBar playerManaBar;

    public event CardPlayedDelegate CardPlayedEvent;
    public event PlayerTurnEndedDelgate PlayerTurnEndedEvent;

    // Start is called before the first frame update
    void Start()
    { 
        // // get a reference to the boardManager (we have to do it this way, since
        // // interfaces can't be serialized and set in-inspector)
        // boardManager = boardManagerObject.GetComponent<IUIManager>();
        // // doing this here and in OnEnable(), because OnEnable() runs before
        // // Start(), so boardManager will be null at the time; if for some
        // // reason, however BattleManager is disabled, then re-enabled, we'll
        // // want it in OnEnable() as well
        // if(boardManager != null)
        // {
        //     boardManager.CardPlayedEvent += PlayCard;
        //     boardManager.PlayerTurnEndedEvent += HandlePlayerTurnEnded;
        // }

        // // initialize the deck, discardPile, hand, and DCCS
        // deck = PlayerStats.Instance.GetCollectedCardIDs();
        // ShuffleDeck();
        // discardPile = new List<int>();
        // hand = new List<int>();
        // dccs = new Dictionary<int, DCCard>();

        // // draw the player's opening hand
        // for(int i = 0; i < Constants.STARTING_HAND_SIZE; i++)
        // {
        //     DrawCard();
        // }

        Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
    }

    // void OnEnable()
    // {
    //     if(boardManager != null)
    //     {
    //         boardManager.CardPlayedEvent += PlayCard;
    //         boardManager.PlayerTurnEndedEvent += HandlePlayerTurnEnded;
    //     }
    // }

    // void OnDisable()
    // {
    //     if(boardManager != null)
    //     {
    //         boardManager.CardPlayedEvent -= PlayCard;
    //         boardManager.PlayerTurnEndedEvent -= HandlePlayerTurnEnded;
    //     }
    // }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePlayerHealth(int maxHealth, int currentHealth) {
        playerHealthBar.setMaxHealth(maxHealth);
        playerHealthBar.setHealth(currentHealth);
    }

    public void UpdatePlayerMana(int maxMana, int currentMana) {
        playerManaBar.setMaxMana(maxMana);
        playerManaBar.setMana(currentMana);
    }

    public void DrawCard(int cardID) {

    }

    public void RemoveCardFromHand(int cardID, bool discarded) {

    }

    public void PutCardInDCCS(int cardID, int countDown, int dccsSlot) {

    }

    public void RemoveCardFromDCCS(int dccsSlot) {

    }

    public void UpdateDCCSCount(int dccsSlot, int newCountDown) {

    }

    public void DeactivateBasicAbilities() {

    }

    public void ActivateBasicAbilities() {

    }

    public void UpdateEnemyHealth(int enemyID, int maxHealth, int currentHealth) {
        enemyHealthBar.setMaxHealth(maxHealth);
        enemyHealthBar.setHealth(currentHealth);
    }

    public void RemoveEnemy(int enemyID) {

    }

    public void WinGame() {

    }

    public void LoseGame() {

    }
}
