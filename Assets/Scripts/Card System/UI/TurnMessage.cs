using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnMessage : MonoBehaviour
{
    //-----------------
    // member variables
    //-----------------

    [SerializeField] private bool forPlayer = true;
    [SerializeField] BoardManager boardManager = null;
    [SerializeField] Animator animator = null;

    public void PlayAnimation()
    {
        this.gameObject.SetActive(true);
        if(animator != null)
        {
            animator.Play(Constants.TURN_MESSAGE_ANIMATION);
        }
    }

    public void HandleAnimationEnd()
    {
        this.gameObject.SetActive(false);
        if(boardManager != null)
        {
            if(forPlayer)
            {
                boardManager.EnableInteraction();
            }
            else
            {
                boardManager.HandleEndOfPlayerTurn();
            }
        }
    }
}
