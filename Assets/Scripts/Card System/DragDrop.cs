using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler,IDragHandler, IDropHandler,IPointerEnterHandler,IPointerExitHandler
{
    public Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originPos;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //   if (wedonthavecardselected)
        //   Now it will still trigger while we are dragging another card
        if (GameObject.Find("SelectedCard").transform.childCount == 0)
        {
            rectTransform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            //rectTransform.anchoredPosition += new Vector2(-80, 0);
            rectTransform.anchoredPosition += new Vector2(0, 100);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GameObject.Find("SelectedCard").transform.childCount == 0)
        {   
            //rectTransform.anchoredPosition -= new Vector2(-80, 0);
            rectTransform.anchoredPosition -= new Vector2(0, 100);
            rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }

    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
        GetComponent<Image>().color = new Color32(146, 217, 255, 255);
        originPos = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        rectTransform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        transform.SetParent(GameObject.Find("SelectedCard").transform);

    }

    public void OnEndDrag(PointerEventData eventData)
    {

        rectTransform.localScale = new Vector3(1f, 1f, 1f);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        GetComponent<Image>().color = Color.white;
        rectTransform.anchoredPosition = originPos;
        transform.SetParent(GameObject.Find("CardsSet").transform);

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
    }

    public void OnDrop (PointerEventData eventData)
    {
        Debug.Log("onDrop");
    }
}
