using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tempchef : MonoBehaviour
{
    public int chefHp;
    public Text hptxt;
    public int globalmana;
    public Text gmtxt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    public void gmDec(int value)
    {
        this.globalmana -= value;
    }

    public void gmInc(int val)
    {
        this.globalmana += val;
    }

    public void hpDecrease(int value)
    {
        this.chefHp -= value;
    }


    public void hpIncrease(int value)
    {
        this.chefHp += value;
    }


    void Update()
    {
        hptxt.text = "Hp :" + chefHp;
        gmtxt.text = "Global Mana :" + globalmana;
    }
}

