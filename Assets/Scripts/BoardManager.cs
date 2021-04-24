﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour, IUIManager
{
    public List<Card> hand;
    public GameObject card;

    public event CardPlayedDelegate CardPlayedEvent;

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

    public void DrawCard(int cardID) {

    }

    public void PlayCard() {

    }

    public void PutCardInDCCS(Card card) {

    }

    public void RemoveCardFromDCCS(Card card) {

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
