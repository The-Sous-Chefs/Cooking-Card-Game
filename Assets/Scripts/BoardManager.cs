﻿using System.Collections;
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

    public void DrawCard(int cardID) {

    }

    public void RemoveCardFromHand(int cardID, bool discarded) {

    }

    public void PutCardInDiscardPile(int cardID, bool fromDeck) {

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

    }

    public void RemoveEnemy(int enemyID) {

    }

    public void WinGame() {

    }

    public void LoseGame() {

    }
}
