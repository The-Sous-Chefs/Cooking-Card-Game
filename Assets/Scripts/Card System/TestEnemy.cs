using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEngine.UI;

public class TestEnemy : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text hpText;
    [SerializeField] private Image enemyImage;

    public void SetNameText(string name)
    {
        nameText.text = name;
    }

    public void SetHPText(int maxHP, int hp)
    {
        hpText.text = "HP: " + hp.ToString() + " / " + maxHP.ToString();;
    }

    public void ToggleStunned(bool stunned)
    {
        if(stunned)
        {
            enemyImage.color = Color.red;
        }
        else
        {
            enemyImage.color = Color.white;
        }
    }
}
