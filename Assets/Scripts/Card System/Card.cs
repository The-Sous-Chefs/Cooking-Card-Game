using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    BASIC,
    IMMEDIATE,
    DELAYED,
    CONTINUOUS
}

public enum CardClass
{
    DAMAGE,         // cards that do damage to enemies
    HEAL,           // cards that heal the player
    DEFEND,         // cards that block incoming damage
    CONTROL,        // cards that prevent enemies from acting
    SPECIAL         // cards that draw cards, discard cards, generate mana, etc.
}

/*
 * NOTE: By being a struct, Card is a "value type," meaning that the variable
 *       the struct is "stored" in is actually storing the struct's values, so
 *       copying a struct will copy by-value. Classes in C# are "reference
 *       types," so copying one will copy by-reference. If we're expecting
 *       modifications of a copy to be reflected across all copies, this may
 *       need to be a class.
 */
public struct Card
{
    //-----------------
    // member variables
    //-----------------

    // visual properties of the card
    public string name          { get; }
    public int cost             { get; }
    public CardType cardType    { get; }
    public string cardText      { get; }

    // values that drive functionality
    public int turnsInPlay      { get; }
    public int singleDamage     { get; }
    public int aoeDamage        { get; }
    public int heal             { get; }
    public int draw             { get; }
    public int discard          { get; }
    public int manaRegen        { get; }
    public float blockPercent   { get; }    // "translated" by constructor
    public bool stuns           { get; }    // "translated" by constructor

    // values that drive functionality, determined based on the above
    public List<CardClass> cardClasses  { get; }    // ??????
    public bool needsTarget             { get; }

    public Card(
            string name,
            int cost,
            string cardType,
            string cardText,
            int turnsInPlay,
            int singleDamage,
            int aoeDamage,
            int heal,
            int draw,
            int discard,
            int manaRegen,
            float blockPercent,
            bool stuns
    )
    {
        this.name = name;
        this.cost = cost;

        switch(cardType)
        {
            case "Basic":
                this.cardType = CardType.BASIC;
                break;

            case "Immediate":
                this.cardType = CardType.IMMEDIATE;
                break;

            case "Delayed":
                this.cardType = CardType.DELAYED;
                break;

            case "Continuous":
                this.cardType = CardType.CONTINUOUS;
                break;

            default:
                Debug.Assert(
                        false,
                        "Invalid card type string in Card constructor.\n" +
                        cardType
                );
                // have to assign a CardType to compile...
                this.cardType = CardType.IMMEDIATE;
                break;
        }

        this.cardText = cardText;
        this.turnsInPlay = turnsInPlay;
        this.singleDamage = singleDamage;
        this.aoeDamage = aoeDamage;
        this.heal = heal;
        this.draw = draw;
        this.discard = discard;
        this.manaRegen = manaRegen;
        this.blockPercent = blockPercent;
        this.stuns = stuns;
        if(this.stuns)
        {
            Debug.Assert(
                this.cardType == CardType.CONTINUOUS,
                "Only continuous cards can stun enemies."
            );
        }

        this.cardClasses = new List<CardClass>();
        if((this.singleDamage > 0) || (this.aoeDamage > 0))
        {
            this.cardClasses.Add(CardClass.DAMAGE);
        }
        if(this.heal > 0)
        {
            this.cardClasses.Add(CardClass.HEAL);
        }
        if(this.blockPercent > 0f)
        {
            this.cardClasses.Add(CardClass.DEFEND);
        }
        if(this.stuns)
        {
            this.cardClasses.Add(CardClass.CONTROL);
        }
        if((this.draw > 0) || (this.discard > 0) || (this.manaRegen > 0))
        {
            this.cardClasses.Add(CardClass.SPECIAL);
        }

        this.needsTarget = ((this.singleDamage > 0) || this.stuns);

        if(this.cardType == CardType.IMMEDIATE)
        {
            Debug.Assert(
                    this.turnsInPlay == 0,
                    "Immediate cards should have 0 turns in play."
            );
        }
        else if(
                this.cardType == CardType.DELAYED ||
                this.cardType == CardType.CONTINUOUS
        )
        {
            Debug.Assert(
                this.turnsInPlay > 0,
                "Delayed and Continuous cards should have >0 turns in play."
            );
        }
    }

    public override string ToString()
    {
        string lineOne = name + ",\tCost: " + cost;
        string lineTwo = cardText;
        return lineOne + "\n" + lineTwo;
    }

    public string AllInfoToString()
    {
        string lineOne = name + ",\tCost: " + cost;
        string lineTwo = System.Enum.GetName(typeof(CardType), cardType) + " (";
        for(int i = 0; i < cardClasses.Count; i++)
        {
            lineTwo += System.Enum.GetName(typeof(CardClass), cardClasses[i]);
            if(i != cardClasses.Count - 1)
            {
                lineTwo += ", ";
            }
        }
        lineTwo += ")";
        string lineThree =
                "Turns: " + turnsInPlay + ", Dmg: " + singleDamage +
                ", AOE Dmg: " + aoeDamage + ", Heal: " + heal + ", Draw: " +
                draw + ", Discard: " + discard + ", Mana: " + manaRegen +
                ", Blocks: " + blockPercent + ", Stuns: " + stuns +
                ", Needs Target: " + needsTarget;
        string lineFour = cardText;
        return lineOne + "\n" + lineTwo + "\n" + lineThree + "\n" + lineFour;
    }
}
