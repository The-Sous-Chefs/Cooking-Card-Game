using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    // parsing-related constants
    public static int NAME = 0;
    public static int COST = 1;
    public static int TYPE = 2;
    public static int TURNS = 3;
    public static int S_DAMAGE = 4;
    public static int AOE_DAMAGE = 5;
    public static int HEAL = 6;
    public static int DRAW = 7;
    public static int DISCARD = 8;
    public static int MANA_REGEN = 9;
    public static int BLOCK = 10;
    public static int STUNS = 11;
    public static int CARD_DESCRIPTION = 14;
    public static int IGNORE_CARD = 13;

    // player stats related constants
    public static int MAX_HEALTH = 100;
    public static int MAX_GLOBAL_MANA = 100;

    // deck-related constants
    public static int STARTING_HAND_SIZE = 4;
    public static int MAX_HAND_SIZE = 10;
    public static int DCCS_SIZE = 5;

    // enemy-related constants
    public static int MAX_ENEMIES = 3;

    // UI-related constants
    public static string DCCS_CARD_HOLDER_NAME = "CardHolder";
    public static string DCCS_COUNT_NAME = "Count";

    // temporary constants
    public static int TEMPORARY_SINGLE_ENEMY_ID = 0;
}
