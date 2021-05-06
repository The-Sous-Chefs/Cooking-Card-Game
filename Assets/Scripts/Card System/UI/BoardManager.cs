﻿using System.Collections;
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
    [SerializeField] private GameObject[] dccsSlots = new GameObject[Constants.DCCS_SIZE];  // if more than 5 are added in inspector, they'll be ignored
    private List<CardUI> hand;
    private Dictionary<int, GameObject> dccs;
    private int numCardsInDeck;
    private int numCardsInDiscardPile;
    private bool playerStunned;
    // FIXME: Make enemies dynamically (See the comments about the TestEnemy in
    //        TestUIManager.cs)
    [SerializeField] TestEnemy enemy;
    //For animation
    //Will need to refactor chef, simply pass in the whole group, not so many variable.
    public GameObject ChefGroup;

    // variables used only to display information to the player
    [SerializeField] private HealthBar chefHealthBar;
    [SerializeField] private ManaBar chefManaBar;

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

        hand = new List<CardUI>();
        dccs = new Dictionary<int, GameObject>();
        for(int i = 0; i < Constants.DCCS_SIZE; i++)
        {
            dccs.Add(i, dccsSlots[i]);
            Transform countTransform = dccsSlots[i].transform.Find(Constants.DCCS_COUNT_NAME);
            if(countTransform != null)
            {
                Text countText = countTransform.gameObject.GetComponent<Text>();
                countText.text = "";
            }
        }

        numCardsInDeck = 0;
        numCardsInDiscardPile = 0;
        playerStunned = false;

        chefHealthBar.setMaxHealth(PlayerStats.Instance.GetMaxHealth());
        chefHealthBar.setHealth(PlayerStats.Instance.GetHealth());
        chefManaBar.setMaxMana(PlayerStats.Instance.GetMaxGlobalMana());
        chefManaBar.setMana(PlayerStats.Instance.GetGlobalMana());
        chefBlockText.text = "0%";
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


    // don't know why the timer wait is not working here.
    IEnumerator TimerHang()
    {
        Debug.Log("start timer");
        yield return new WaitForSeconds(5);
    }
    public void EndPlayerTurn()
    {
        if(PlayerTurnEndedEvent != null)
        {
            Animator animator = enemy.GetComponent<Animator>();
            if (animator != null)
            {
                Debug.Log("animating");
                animator.SetTrigger("attack");
            }
            StartCoroutine(TimerHang());
            Debug.Log("end timer");
            PlayerTurnEndedEvent();

            if (animator != null)
            {
               // animator.ResetTrigger("attack");
            }

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
        chefHealthBar.setMaxHealth(maxHealth);
        chefHealthBar.setHealth(currentHealth);
    }

    public void UpdatePlayerMana(int maxMana, int currentMana)
    {
        chefManaBar.setMaxMana(maxMana);
        chefManaBar.setMana(currentMana);
    }

    public void UpdatePlayerBlockPercent(float blockPercent)
    {
        chefBlockText.text = (blockPercent * 100) + "%";
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
        hand.Add(newCardInUI.GetComponent<CardUI>());
        //newCardInUI.GetComponent<FadeAnimation>().startFading();
    }

    public void RemoveCardFromHand(int cardID)
    {
        for(int i = 0; i < hand.Count; i++)
        {
            if(hand[i].GetCardID() == cardID)
            {
                CardUI removedCard = hand[i];
                hand.RemoveAt(i);
                Destroy(removedCard.gameObject);
                break;
            }
        }
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
        Debug.Assert((dccsSlot >= 0) && (dccsSlot < Constants.DCCS_SIZE));
        Transform dccsSlotTransform = dccs[dccsSlot].transform;
        if(dccsSlotTransform != null)
        {
            Transform cardHolderTransform = dccsSlotTransform.Find(Constants.DCCS_CARD_HOLDER_NAME);
            if(cardHolderTransform != null)
            {
                Debug.Assert(cardHolderTransform.childCount == 0);
                GameObject dccsCard = Instantiate(card, cardHolderTransform);
                RectTransform dccsCardTransform = (RectTransform) dccsCard.transform;
                dccsCardTransform.anchoredPosition = Vector2.zero;
                dccsCard.GetComponent<CardUI>().CreateCard(cardID);
                dccsCard.GetComponent<DragDrop>().SetCanDrag(false);
            }
            Transform countTransform = dccsSlotTransform.Find(Constants.DCCS_COUNT_NAME);
            if(countTransform != null)
            {
                Text countText = countTransform.gameObject.GetComponent<Text>();
                if(countText != null)
                {
                    countText.text = countDown.ToString();
                }
            }
        }
    }

    public void RemoveCardFromDCCS(int dccsSlot)
    {
        Debug.Assert((dccsSlot >= 0) && (dccsSlot < Constants.DCCS_SIZE));
        Transform dccsSlotTransform = dccs[dccsSlot].transform;
        if(dccsSlotTransform != null)
        {
            Transform cardHolderTransform = dccsSlotTransform.Find(Constants.DCCS_CARD_HOLDER_NAME);
            if(cardHolderTransform != null)
            {
                Debug.Assert(cardHolderTransform.childCount == 1);
                GameObject dccsCard = cardHolderTransform.GetChild(0).gameObject;
                Destroy(dccsCard);
            }
            Transform countTransform = dccsSlotTransform.Find(Constants.DCCS_COUNT_NAME);
            if(countTransform != null)
            {
                Text countText = countTransform.gameObject.GetComponent<Text>();
                if(countText != null)
                {
                    countText.text = "";
                }
            }
        }
    }

    public void UpdateDCCSCount(int dccsSlot, int newCountDown)
    {
        Debug.Assert((dccsSlot >= 0) && (dccsSlot < Constants.DCCS_SIZE));
        Transform dccsSlotTransform = dccs[dccsSlot].transform;
        if(dccsSlotTransform != null)
        {
            Transform countTransform = dccsSlotTransform.Find(Constants.DCCS_COUNT_NAME);
            if(countTransform != null)
            {
                Text countText = countTransform.gameObject.GetComponent<Text>();
                if(countText != null)
                {
                    // countText.text of "" would indicate that there's not a card in that slot
                    Debug.Assert(countText.text != "");
                    countText.text = newCountDown.ToString();
                }
            }
        }
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
        enemy.SetNameText(monster.name);
        enemy.SetHealthBar(monster.maxHP, monster.currentHP);
    }

    public void RemoveEnemy(int monsterID)
    {
        // do nothing, since there's only one sort of hard-coded enemy at the moment
    }

    public void UpdateEnemyHealth(int monsterID, int maxHealth, int currentHealth)
    {
        enemy.SetHealthBar(maxHealth, currentHealth);
    }

    public void UpdateEnemyStunStatus(int monsterID, bool stunned)
    {
        enemy.ToggleStunned(stunned);
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