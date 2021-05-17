using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    void EnableBasicAbilities();

    void DisableBasicAbilities();

    void StartPlayerTurn(bool isPlayerStunned);

    void UpdatePlayerHealth(int maxHealth, int currentHealth);

    void UpdatePlayerMana(int maxMana, int currentMana);

    void UpdatePlayerBlockPercent(float blockPercent);

    void UpdatePlayerStunStatus(bool stunned);

    void PutCardInHand(int cardID);

    void RemoveCardFromHand(int cardID);

    void ShowCardPlayed(int cardID);

    void PutCardInDeck(int cardID);

    void RemoveCardFromDeck(int cardID);

    void PutCardInDiscardPile(int cardID);

    void RemoveCardFromDiscardPile(int cardID);

    void PutCardInDCCS(int cardID, int countDown, int dccsSlot);

    void RemoveCardFromDCCS(int dccsSlot);

    void UpdateDCCSCount(int dccsSlot, int newCountDown);

    void AddEnemy(int monsterID, Monster monster);

    void RemoveEnemy(int monsterID);

    void UpdateEnemyHealth(int monsterID, int maxHealth, int currentHealth);

    void UpdateEnemyStunStatus(int monsterID, bool stunned);

    /*
     * This method must somehow cause the backend to call RunNextEnemyTurn().
     * The backend will call this method to play the animation for the enemy's
     * turn (if there is one), then when the animation is over, it's relying on
     * something calling RunNextEnemyTurn() to actually update the state of the
     * game based on that enemy's action and start the next enemy turn if there
     * is one or start the player's next turn.
     */
    void PlayEnemyTurnAnimation(int monsterID, MonsterAction action);

    void WinGame();

    void LoseGame();
}
