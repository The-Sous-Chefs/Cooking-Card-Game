using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunnedMessage : MonoBehaviour
{
    //-----------------
    // member variables
    //-----------------

    [SerializeField] BoardManager boardManager;
    [SerializeField] Animator animator;

    public void PlayAnimation()
    {
        this.gameObject.SetActive(true);
        if(animator != null)
        {
            animator.Play(Constants.STUN_MESSAGE_ANIMATION);
        }
    }

    public void HandleAnimationEnd()
    {
        this.gameObject.SetActive(false);
        if(boardManager != null)
        {
            boardManager.EndPlayerTurn();
        }
    }
}
