using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tempchef : MonoBehaviour
{
    public Button chefmove;
    public Text status;
    public Text hptxt;
    public Text gmtxt;
    public int[,] buff;
    public PlayerStats playerSta;
    // Start is called before the first frame update
    void Start()
    {
        //current buff list [0] = stunned or not 
        // [1] = current block rate in %, the actual damage = damage gonna receive * (1 - block rate)
        // the second value means the duration, for example buff{{1,2 }, {0,0}} means the stun will last for 2 turns
        buff = new int[2, 2] { { 0, 0 }, { 0, 0 } };
    }


    // easy to read since we don't have much status now
    public void getStunned(int turns)
    {
        buff[0,0] = 1;
        buff[0,1] = turns;
    }
    
    public void blocking(int blockrate, int turns)
    {
        buff[1,0] = blockrate;
        buff[1,1] = turns;
    }

    // to update as turns pass, once the remaining turns for one status become 0, set the value of that status to 0 to neutralize the buff.
    public void buffupdate()
    {
        // go through all the status we have, and decrease the turns number
        for (int i = 0; i < buff.GetLength(0);i++)
        {
            buff[i, 1] -= 1;
            if (buff[i,1] <= 0) {
                buff[i, 1] = 0;
                buff[i, 0] = 0;
            }
        }
    }

    private void showStunned() {
        if (buff[0,0] != 0) {
            chefmove.interactable = false;
        } else {
            chefmove.interactable = true;
        }
    }



    void Update()
    {
        hptxt.text = "Hp :" + PlayerStats.Instance.GetHealth();
        gmtxt.text = "Global Mana :" + PlayerStats.Instance.GetGlobalMana();
        showStunned();
        status.text = "Current status: Stunned :" + buff[0,0] + " Turns remaining:" + buff[0,1] +"\r\n" +"Blocking rate:" + buff[1,0] + "%" + " Turns remaining: " + buff[1,1];
    }
}

