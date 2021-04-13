using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public Monster demoMonster = new Monster("demoMonster",1,50,3, 1, new int[4] { 1, 1, 1, 2 });
    public int enemyHp;
    public Text hptxt;
    public Text enmName;

    // Start is called before the first frame update
    void Start()
    {
        enemyHp = demoMonster.currentHp;
        enmName.text = demoMonster.name;
    }

    // Update is called once per frame
    void Update()
    {
        hptxt.text = "Hp :" + demoMonster.currentHp;
    }
}
