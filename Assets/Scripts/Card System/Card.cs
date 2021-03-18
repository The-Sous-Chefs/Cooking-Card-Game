using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
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
    public string name              { get; }
    public int cost                 { get; }
    public CardType cardType        { get; }
    public string cardText          { get; }

    // values that drive functionality, provided by a List<int>--in this order
    public int turnsInPlay          { get; }
    public int singleDamage         { get; }
    public int aoeDamage            { get; }
    public int heal                 { get; }
    public int draw                 { get; }
    public int discard              { get; }
    public int manaRegen            { get; }
    public float blockPercent       { get; }    // "translated" by constructor
    public bool stuns               { get; }    // "translated" by constructor

    // values that drive functionality, determined based on the above
    public List<CardClass> classes  { get; }    // ??????
    public bool needsTarget         { get; }

    // public Card(string name, string type, int cost, string text, List<int> values)
    // {
    //     //
    // }
}
