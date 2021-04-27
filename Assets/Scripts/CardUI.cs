using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public Text cardName;
    public Text cost;
    public Text text;
    public Image cardDisplay;
    public int cardID;

    // Start is called before the first frame update
    void Start()
    {
        // createCard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateCard(int cardID)
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
