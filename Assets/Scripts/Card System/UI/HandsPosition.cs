using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HandsPosition : MonoBehaviour
{
    public GameObject CardsSet;
    private int currentNumberOfCard;
    private Vector2 ogsize= new Vector2(142,187);
    // Update is called once per frame


    public void HandsUIUpdate()
    {
        currentNumberOfCard = CardsSet.transform.childCount;
        int xPos = 260 - (27 * currentNumberOfCard);
        CardsSet.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, -50);
        //int spacing = -60 - (1 * currentNumberOfCard);

        CardsSet.GetComponent<GridLayoutGroup>().cellSize = new Vector2(ogsize.x - 1f * currentNumberOfCard, ogsize.y);
        //CardsSet.GetComponent<GridLayoutGroup>().spacing = new Vector2(spacing, 0);
        GameObject.Find("placeHolder").transform.SetAsLastSibling();
    }
}

////This C# was trying to make fix the position of the hands, try to make the card to arranged in the center of middle. 
