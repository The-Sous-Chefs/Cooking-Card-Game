using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour, IUIManager
{
    public List<Card> hand;
    public GameObject card;

    public event CardPlayedDelegate CardPlayedEvent;
    public event BasicAbilityUsedDelegate BasicAbilityUsedEvent;
    public event PlayerTurnEndedDelgate PlayerTurnEndedEvent;

    // Start is called before the first frame update
    void Start()
    { 
        Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePlayerHealth(int maxHealth, int currentHealth) {

    }

    public void UpdatePlayerMana(int maxMana, int currentMana) {

    }

    public void UpdatePlayerBlockPercent(float blockPercent) {

    }

    public void UpdatePlayerStunStatus(bool stunned) {

    }

    public void PutCardInHand(int cardId) {

    }

    public void RemoveCardFromHand(int cardID) {

    }

    public void PutCardInDeck(int cardID) {

    }

    public void RemoveCardFromDeck(int cardID) {

    }

    public void PutCardInDiscardPile(int cardID) {

    }

    public void RemoveCardFromDiscardPile(int cardID) {

    }

    public void PutCardInDCCS(int cardID, int countDown, int dccsSlot) {

    }

    public void RemoveCardFromDCCS(int dccsSlot) {

    }

    public void UpdateDCCSCount(int dccsSlot, int newCountDown) {
        
    }

    public void ActivateBasicAbilities() {

    }

    public void DeactivateBasicAbilities() {

    }

    public void AddEnemy(int monsterID, Monster monster) {

    }

    public void RemoveEnemy(int monsterID) {

    }

    public void UpdateEnemyHealth(int monsterID, int maxHealth, int currentHealth) {

    }

    public void UpdateEnemyStunStatus(int monsterID, bool stunned) {

    }

    public void WinGame() {

    }

    public void LoseGame() {

    }
}
