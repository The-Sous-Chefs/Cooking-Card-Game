using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler,IDragHandler,IPointerEnterHandler,IPointerExitHandler
{
    public Canvas canvas;
    private bool canDrag;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private int index;


    private void Awake()
    {
        canDrag = true;
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    
    public void SetCanDrag(bool canDrag)
    {
        this.canDrag = canDrag;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //   if (wedonthavecardselected)
        //   Now it will still trigger while we are dragging another card
        if (GameObject.Find("SelectedCard").transform.childCount == 0)
        {
            rectTransform.localScale = (canDrag) ? new Vector3(1.2f, 1.2f, 1.2f) : new Vector3(1f, 1f, 1f);
            GameObject.Find("placeHolder").transform.SetSiblingIndex(rectTransform.GetSiblingIndex()+1);


        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GameObject.Find("SelectedCard").transform.childCount == 0)
        {   
            rectTransform.localScale = new Vector3(1f, 1f, 1f);
            GameObject.Find("placeHolder").transform.SetAsLastSibling();
        }

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(canDrag)
        {
            canvasGroup.alpha = .6f;
            canvasGroup.blocksRaycasts = false;
            index = GetComponent<RectTransform>().GetSiblingIndex();

        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(canDrag)
        {

            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            rectTransform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            transform.SetParent(GameObject.Find("SelectedCard").transform);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(canDrag)
        {
            rectTransform.localScale = new Vector3(1f, 1f, 1f);
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            transform.SetParent(GameObject.Find("CardsSet").transform);
            GetComponent<RectTransform>().SetSiblingIndex(index);
            GameObject.Find("placeHolder").transform.SetAsLastSibling();

        }
    }

}
