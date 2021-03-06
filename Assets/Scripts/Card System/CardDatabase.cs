using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class CardDatabase
{
    //-----------------
    // static variables
    //-----------------

    // this essentially allows any script to access CardDatabase.Instance and be
    // able to get the exact same object, since it will either construct the
    // original instance or return the existing one
    private static CardDatabase instance;
    public static CardDatabase Instance
    {
        get
        {
            if(CardDatabase.instance == null)
            {
                Debug.Log("Instancing the card database.");
                CardDatabase.instance = new CardDatabase();
            }
            else
            {
                // Debug.Log("Using the existing card database");
            }
            return CardDatabase.instance;
        }
    }

    //-----------------
    // member variables
    //-----------------

    private List<Card> cardsInGame = null;

    //--------
    // methods
    //--------

    CardDatabase()
    {
        cardsInGame = new List<Card>();

        TextAsset cardsCSV = Resources.Load<TextAsset>("Cards");
        List<string> lines = new List<string>(cardsCSV.text.Split('\n'));
        for(int i = 1; i < lines.Count; i++)
        {
            List<string> lineFields = ParseLine(lines[i]);
            // Debug.Assert(
            //         lineFields.Count == Constants.CARD_DESCRIPTION + 1,
            //         "Line " + (i + 1) + " didn't have all the fields."
            // );
            if(lineFields[Constants.IGNORE_CARD].Length == 0)
            {
                // this method will add the card to cardsInGame to save a copy
                CreateCardFromLine(lineFields);
            }
        }

        foreach(Card card in cardsInGame)
        {
            Debug.Log(card);
        }
    }

    private List<string> ParseLine(string line)
    {
        List<string> returnMe = new List<string>();

        returnMe.Add("");
        int currIndex = 0;
        bool betweenQuotes = false;
        foreach(char currChar in line)
        {
            if(currChar == '"')
            {
                betweenQuotes = !betweenQuotes;
            }
            else if(currChar == ',')
            {
                if(!betweenQuotes)
                {
                    returnMe.Add("");
                    currIndex++;
                }
                else
                {
                    returnMe[currIndex] += currChar;
                }
            }
            else
            {
                returnMe[currIndex] += currChar;
            }
        }

        return returnMe;
    }

    private void CreateCardFromLine(List<string> lineFields)
    {
        Debug.Assert(cardsInGame != null);

        string name = lineFields[Constants.NAME];
        Debug.Assert(name.Length > 0, "Name shouldn't be empty.");
        string type = lineFields[Constants.TYPE].Trim(' ');
        Debug.Assert(type.Length > 0, "Type shouldn't be empty.");
        Debug.Assert(
                (type == "Immediate") || (type == "Delayed") ||
                    (type == "Continuous") || (type == "Basic"),
                "Type should be Immediate, Delayed, Continuous, or Basic."
        );
        string text = lineFields[Constants.CARD_DESCRIPTION];
        Debug.Assert(text.Length > 0, "Text shouldn't be empty.");
        string s_cost = lineFields[Constants.COST];
        Debug.Assert(s_cost.Length > 0, "Cost shouldn't be empty.");
        string s_turns = lineFields[Constants.TURNS];
        string s_singleDamage = lineFields[Constants.S_DAMAGE];
        string s_aoeDamage = lineFields[Constants.AOE_DAMAGE];
        string s_heal = lineFields[Constants.HEAL];
        string s_draw = lineFields[Constants.DRAW];
        string s_discard = lineFields[Constants.DISCARD];
        string s_manaRegen = lineFields[Constants.MANA_REGEN];
        string s_block = lineFields[Constants.BLOCK];
        string s_stuns = lineFields[Constants.STUNS];

        int cost = Int32.Parse(s_cost);
        Debug.Assert(cost >= 0, "Cost should be non-negative.");
        int turns = (s_turns.Length > 0) ? Int32.Parse(s_turns) : 0;
        Debug.Assert(turns >= 0, "Turns should be non-negative");
        int singleDamage = (s_singleDamage.Length > 0) ?
                Int32.Parse(s_singleDamage) :
                0;
        Debug.Assert(singleDamage >= 0, "Damage should be non-negative.");
        int aoeDamage = (s_aoeDamage.Length > 0) ? Int32.Parse(s_aoeDamage) : 0;
        Debug.Assert(aoeDamage >= 0, "AOE Damage Should be non-negative.");
        int heal = (s_heal.Length > 0) ? Int32.Parse(s_heal) : 0;
        Debug.Assert(heal >= 0, "Heal should be non-negative.");
        int draw = (s_draw.Length > 0) ? Int32.Parse(s_draw) : 0;
        Debug.Assert(draw >= 0, "Draw should be non-negative.");
        int discard = (s_discard.Length > 0) ? Int32.Parse(s_discard) : 0;
        Debug.Assert(discard >= 0, "Discard should be non-negative.");
        int manaRegen = (s_manaRegen.Length > 0) ? Int32.Parse(s_manaRegen) : 0;
        Debug.Assert(manaRegen >= 0, "Mana Regen should be non-negative.");
        float block = (s_block.Length > 0) ?
                float.Parse(
                        s_block,
                        CultureInfo.InvariantCulture.NumberFormat
                ) :
                0f;
        Debug.Assert(block >= 0f, "Block should be non-negative.");
        bool stuns = (s_stuns.Length > 0);

        cardsInGame.Add(new Card(
                name,
                cost,
                type,
                text,
                turns,
                singleDamage,
                aoeDamage,
                heal,
                draw,
                discard,
                manaRegen,
                block,
                stuns
        ));
    }

    public bool CardIDExists(int id)
    {
        return (id >= 0) && (id < cardsInGame.Count);
    }

    public Card GetCardByID(int id)
    {
        Debug.Assert(CardIDExists(id));
        return cardsInGame[id];
    }

    public List<int> GetBasicAbilityIDs()
    {
        List<int> basicAbilityIDs = new List<int>();
        for(int i = 0; i < cardsInGame.Count; i++)
        {
            if(cardsInGame[i].cardType == CardType.BASIC)
            {
                basicAbilityIDs.Add(i);
            }
        }
        return basicAbilityIDs;
    }

    public List<int> GetAllCardIDs()
    {
        List<int> nonBasicIDs = new List<int>();
        for(int i = 0; i < cardsInGame.Count; i++)
        {
            if(cardsInGame[i].cardType != CardType.BASIC)
            {
                nonBasicIDs.Add(i);
            }
        }
        return nonBasicIDs;
    }

    // we may want methods here to return random cards, when the player is
    // adding cards to their deck
}
