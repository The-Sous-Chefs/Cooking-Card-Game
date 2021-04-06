using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class can be used to initialize scenes with whatever they need to be
 * initialized. This class just instances any singletons, but if a scene needs
 * more particular behavior, it can have a child class implemented.
 */
public class SceneInitializer : MonoBehaviour
{
    protected CardDatabase cardDB;

    protected void Awake()
    {
        // technically, the singletons only need to be instanced once, but
        // doing it in every scene makes sure we can't get into a state where
        // they're not there when we expect them to be

        // simply accessing CardDatabase.Instance will make sure it exists
        cardDB = CardDatabase.Instance;
    }
}
