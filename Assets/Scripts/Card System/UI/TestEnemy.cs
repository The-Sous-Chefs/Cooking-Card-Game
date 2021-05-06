using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestEnemy : MonoBehaviour, IDropHandler
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text hpText;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private Image enemyImage;
    [SerializeField] private GameObject boardManageGameObject;

    public void SetNameText(string name)
    {
        nameText.text = name;
    }

    public void SetHPText(int maxHP, int hp)
    {
        if(hpText != null)
        {
            hpText.text = "HP: " + hp.ToString() + " / " + maxHP.ToString();
        }
    }

    public void SetHealthBar(int maxHP, int hp)
    {
        if(healthBar != null)
        {
            healthBar.setMaxHealth(maxHP);
            healthBar.setHealth(hp);
        }
    }

    public void ToggleStunned(bool stunned)
    {
        if(stunned)
        {
            enemyImage.color = Color.red;
        }
        else
        {
            enemyImage.color = Color.white;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject tempCard = GameObject.Find("SelectedCard");
        boardManageGameObject.GetComponent<BoardManager>().playCardByID(tempCard.transform.GetChild(0).GetComponent<CardUI>().cardID);
        //Destroy(tempCard);
    }
}
