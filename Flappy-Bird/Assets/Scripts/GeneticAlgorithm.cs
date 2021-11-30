using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Utility class for generating new generations for a given population
public class GeneticAlgorithm
{
    // The number of individuals which are going to be selected for elitism
    static private int k = 10;

    // Fitness scores for each individual
    static private float[] scores;

    // The new generation of objects
    static GameObject[] newGeneration;

    // The old generation of objects
    static GameObject[] oldGeneration;

    static private float fitness(int score)
    {
        return score;
    }

    // TODO
    static private GameObject[] getBestK(GameObject[] currentGeneration)
    {
        GameObject[] best = new GameObject[k];


        return best;
    }

    static private void selection()
    {
        newGeneration = new GameObject[oldGeneration.Length];
        scores = new float[oldGeneration.Length];

        // Calculate the fitness scores
        for (int i = 0; i < oldGeneration.Length; i++)
            scores[i] = fitness(oldGeneration[i].GetComponent<BirdController>().getScore());

        // Normalize the fitness values
        float normalizingFactor = 0.0f;
        for (int i = 0; i < scores.Length; i++)
            normalizingFactor += scores[i];

        for (int i = 0; i < scores.Length; i++)
            scores[i] = scores[i] / normalizingFactor;

        // Accumulated selection probability - used for roulette selection
        float[] q = new float[scores.Length + 1];
        q[0] = 0f;
        for (int i = 0; i < scores.Length; i++)
            q[i + 1] = q[i] + scores[i];

        // Apply selection
        for (int i = 0; i < newGeneration.Length; i++)
        {
            float r = Random.Range(0.0000001f, 1f);
            // Select individual
            for (int j = 0; j < q.Length - 1; j++)
                if (q[j] < r && r <= q[j + 1])
                {
                    // This should be a deepcopy
                    // But it should deep copy what?
                    newGeneration[i] = oldGeneration[j].GetComponent<BirdController>().deepCopy();
                    break;
                }
        }

        // Apply elitism
        // TODO
    }

    static private void mutation()
    {

    }

    static private void crossover()
    {

    }

    // Public function for creating the next generation of gameObjects
    static public GameObject[] getNextGeneration(GameObject[] currentGeneration)
    {
        oldGeneration = currentGeneration;
        // Not tested, may not work
        selection();
        mutation();
        crossover();

        return newGeneration;
    }
}
