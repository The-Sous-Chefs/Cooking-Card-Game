using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tempchef : MonoBehaviour
{
    public int chefHp;
    public Text hptxt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hptxt.text = "Hp :" + chefHp;
    }
}
