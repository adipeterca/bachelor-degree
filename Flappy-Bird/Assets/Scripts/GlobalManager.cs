using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public int maxGenerationCount;
    public int populationSize;
}
