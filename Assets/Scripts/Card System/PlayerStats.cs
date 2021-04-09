using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    //-----------------
    // static variables
    //-----------------

    private static PlayerStats instance;
    public static PlayerStats Instance
    {
        get
        {
            if(PlayerStats.instance == null)
            {
                Debug.Log("Instancing the player stats singleton.");
                PlayerStats.instance = new PlayerStats();
            }
            else
            {
            //    Debug.Log("Using the existing player stats singleton.");
            }
            return PlayerStats.instance;
        }
    }

    //-----------------
    // member variables
    //-----------------

    private int maxHealth;
    private int currentHealth;
    private int maxGlobalMana;
    private int currentGlobalMana;

    PlayerStats()
    {
        maxHealth = Constants.MAX_HEALTH;
        currentHealth = maxHealth;
        maxGlobalMana = Constants.MAX_GLOBAL_MANA;
        currentGlobalMana = maxGlobalMana;
    }

    public void ApplyDamage(int damage)
    {
        Debug.Assert(damage >= 0);
        currentHealth = (currentHealth - damage > 0) ?
                (currentHealth - damage) :
                0;
    }

    public void ApplyHealing(int healing)
    {
        Debug.Assert(healing >= 0);
        currentHealth = (currentHealth + healing < maxHealth) ?
                (currentHealth + healing) :
                maxHealth;
    }

    /*
     * This method takes in an amount of mana to spend and, if it can be spent,
     * will subtract that mana from currentGlobalMana and return true. If the
     * player doesn't have enough mana to spend the amount, it will return false.
     */
    public bool TrySpendMana(int amount)
    {
        Debug.Assert(amount >= 0);
        if(amount <= currentGlobalMana)
        {
            currentGlobalMana -= amount;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddGlobalMana(int amount)
    {
        Debug.Assert(amount >= 0);
        currentGlobalMana = (currentGlobalMana + amount < maxGlobalMana) ?
                (currentGlobalMana + amount) :
                maxGlobalMana;
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetHealthAsPercentage()
    {
        return ((float) currentHealth) / ((float) maxHealth);
    }

    public int GetGlobalMana()
    {
        return currentGlobalMana;
    }

    public int GetMaxGlobalMana()
    {
        return maxGlobalMana;
    }

    public float GetGlobalManaAsPercentage()
    {
        return ((float) currentGlobalMana) / ((float) maxGlobalMana);
    }

    public override string ToString()
    {
        return "PlayerStats (Health: " +
                currentHealth + "/" + maxHealth + ", Global Mana: " +
                currentGlobalMana + "/" + maxGlobalMana + ")";
    }
}
