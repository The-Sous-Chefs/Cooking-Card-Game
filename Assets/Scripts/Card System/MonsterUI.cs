using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MonsterUI : MonoBehaviour, IDropHandler
{
    public Image playedCardUIPlaceHolder;
    public GameObject battleManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            Debug.Log("card played!!!!");
            Destroy(playedCardUIPlaceHolder);
        //    battleManager.GetComponent<TestUIManager>().pickClickedCurrentCard(4);
            battleManager.GetComponent<TestUIManager>().PlayCurrentCard();
        }
    }
}
