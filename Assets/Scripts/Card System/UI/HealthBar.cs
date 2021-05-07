using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Text maxHP;
    [SerializeField] private Text currentHP;

    public void setMaxHealth(int health) {
        slider.maxValue = health;
        // slider.value = health;
        maxHP.text = health.ToString();
    }
    
    public void setHealth(int health)
    {
        slider.value = health;
        currentHP.text = health.ToString();
    }
}
