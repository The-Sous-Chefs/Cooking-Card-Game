using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CardPlayedDelegate(int cardID);
public delegate void PlayerTurnEndedDelgate();

/*
 * This interface can be implemented by both the actual UI for the battle system
 * and a testing UI. The BattleManager can just have a reference to an IUIManager
 * and call these methods on it, regardless of which UI it actually is. This will
 * allow us to separate the backend from our testing UI and also develop the full
 * game UI.
 *
 * NOTE: The parameters passed to these methods may need changing. They're
 *       somewhat informed by needing to work in both UI contexts (which may be
 *       bad design). Some methods are also commented out, since implementing
 *       classes have to implement each method to compile.
 */
public interface IUIManager
{
    //-------
    // events
    //-------

    event CardPlayedDelegate CardPlayedEvent;
    event PlayerTurnEndedDelgate PlayerTurnEndedEvent;

    //--------
    // methods
    //--------

    void UpdatePlayerHealth(int maxHealth, int currentHealth);

    void UpdatePlayerMana(int maxMana, int currentMana);

    void DrawCard(int cardID);

    void RemoveCardFromHand(int cardID, bool discarded);

    void PutCardInDCCS(int cardID);

    void RemoveCardFromDCCS(int cardID);

    void UpdateDCCSCounts();

    void DeactivateBasicAbilities();

    void ActivateBasicAbilities();

    void UpdateEnemyHealth(int enemyID, int maxHealth, int currentHealth);

    void RemoveEnemy(int enemyID);

    void WinGame();

    void LoseGame();
}
