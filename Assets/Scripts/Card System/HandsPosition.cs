using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsPosition : MonoBehaviour
{
    public Transform handsUI;

    private int cardNumber = 4;
    // Update is called once per frame
    void Update()
    {
        //int newXPos = 380 + (50 * (cardNumber - handsUI.childCount));
        handsUI.GetComponent<RectTransform>().anchoredPosition = new Vector2((380 ), -105);
    }
}

////This C# was trying to make fix the position of the hands, try to make the card to arranged in the center of middle. 
//// Not Used yet!!!!
// NOT WORKING WELL