using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Text maxMana;
    [SerializeField] private Text currentMana;

    public void setMaxMana(int mana) {
        slider.maxValue = mana;
        // slider.value = mana;
        maxMana.text = mana.ToString();
    }
    
    public void setMana(int mana)
    {
        slider.value = mana;
        currentMana.text = mana.ToString();
    }
}
