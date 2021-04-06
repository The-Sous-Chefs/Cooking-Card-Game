using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Battlemanager : MonoBehaviour
{
    [SerializeField] private int cardID;
    [SerializeField] private Enemy targetEnemy;
    private Card currentCard;

    void Start()
    {
        currentCard = CardDatabase.Instance.GetCardByID(cardID);
        Debug.Log("Card " + currentCard.name + " loaded.");
    }

    public void scancard()
    {
        Debug.Log("Scanning " + currentCard.name);
        singleDamageHandler(targetEnemy);
    }

    // 3/30 and 4/6 demos only: currently just defaultly take effect to the one
    // enemy
    // Need to implement target later
    private bool singleDamageHandler(Enemy targetEnemy)
    {
        if (currentCard.singleDamage > 0)
        {
            Debug.Log("Dealing " + currentCard.singleDamage + " damage to " + targetEnemy.enmName);
            targetEnemy.demoMonster.hpDecrease(currentCard.singleDamage);
            return true;
        }
        return false;
    }
}
