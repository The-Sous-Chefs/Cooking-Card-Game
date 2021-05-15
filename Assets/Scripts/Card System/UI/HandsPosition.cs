using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HandsPosition : MonoBehaviour
{
    public GameObject CardsSet;
    private int currentNumberOfCard;

    // Update is called once per frame


    public void HandsUIUpdate()
    {
        currentNumberOfCard = CardsSet.transform.childCount;
        int xPos = 270 - (25 * currentNumberOfCard);
        CardsSet.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, -50);
        int spacing = -50 - (3 * currentNumberOfCard);
        CardsSet.GetComponent<GridLayoutGroup>().spacing = new Vector2(spacing, 1);
        GameObject.Find("placeHolder").transform.SetAsLastSibling();
    }
}

////This C# was trying to make fix the position of the hands, try to make the card to arranged in the center of middle. 
