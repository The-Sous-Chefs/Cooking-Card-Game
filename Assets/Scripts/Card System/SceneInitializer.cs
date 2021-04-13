using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class can be used to initialize scenes with whatever they need to be
 * initialized. This class just instances any singletons, but if a scene needs
 * more particular behavior, it can have a child class implemented.
 *
 * Actually, this class may not be needed at all, because the things that need
 * the singletons will just instance them by virtue of using them.
 */
public class SceneInitializer : MonoBehaviour
{
    protected CardDatabase cardDB;
    protected PlayerStats playerStats;

    protected void Awake()
    {
        // technically, the singletons only need to be instanced once, but
        // doing it in every scene makes sure we can't get into a state where
        // they're not there when we expect them to be

        // simply accessing the Instances will make sure they exist
        cardDB = CardDatabase.Instance;
        // PlayerStats uses CardDatabase, so do it second (even though
        // it would technically cause CardDatabase to be instanced, itself)
        playerStats = PlayerStats.Instance;
    }
}
