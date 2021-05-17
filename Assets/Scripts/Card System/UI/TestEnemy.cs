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
    private int id;

    void Awake()
    {
        id = -1;
    }

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
        Debug.Assert(
                id != -1,
                "A card is being played on a monster whose UI representation " +
                "didn't have an ID set!"
        );
        if(boardManageGameObject != null)
        {
            BoardManager boardManager = boardManageGameObject.GetComponent<BoardManager>();
            if(boardManager != null)
            {
                boardManager.PlayCardByID(
                        tempCard.transform.GetChild(0).GetComponent<CardUI>().cardID,
                        id
                );
            }
        }
        //Destroy(tempCard);
    }

    public void SetID(int newID)
    {
        id = newID;
    }

    public int GetID()
    {
        return id;
    }

    public void SetBoardManager(BoardManager boardManager)
    {
        boardManageGameObject = boardManager.gameObject;
    }
}
