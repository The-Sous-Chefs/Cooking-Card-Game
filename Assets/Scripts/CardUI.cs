using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField] private Text cardName;
    [SerializeField] private Text cost;
    [SerializeField] private Text text;
    [SerializeField] private Image cardDisplay;
    private int cardID;

    // Start is called before the first frame update
    void Start()
    {
        // createCard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateCard(int cardID)
    {
        this.cardID = cardID;
        Card card = CardDatabase.Instance.GetCardByID(cardID);
        cardName.text = card.name;
        text.text = card.cardText;
        cost.text = card.cost.ToString();
    }

    public int GetCardID()
    {
        return cardID;
    }
}
