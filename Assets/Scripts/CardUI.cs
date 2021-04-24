using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField] private Text name;
    [SerializeField] private Text cost;
    [SerializeField] private Text text;
    [SerializeField] private Image cardDisplay;

    // Start is called before the first frame update
    void Start()
    {
        // createCard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void createCard(Card c) {
        name.text = c.name;
        text.text = c.cardText;
        cost.text = c.cost.ToString();
    }
}
