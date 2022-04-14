using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton class used to pass configuration values from one scene to the other. 
/// <br></br>
/// For development purposes, default values are put in place.
/// </summary>
public class GlobalManager
{
    private static GlobalManager instance;
    
    private GlobalManager() { }

    public static GlobalManager GetInstance()
    {
        if (instance == null)
            instance = new GlobalManager();
        return instance;
    }

    public int maxGenerationCount = 30;
    public int populationSize = 90;
    public string pathToBrain = "F:\\Facultate\\Licenta\\Flappy-Bird\\_Builds\\version_3\\bestBird.txt";
    public int targetScore = 10;
    public bool evolveFromStartBird = false;
}
