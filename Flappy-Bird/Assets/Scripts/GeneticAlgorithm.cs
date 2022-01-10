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
        if (currentGeneration.Length < k)
        {
            // Cannot select 'k' elements from a list with less than 'k' elements
            return null;
        }
        GameObject[] best = new GameObject[k];
        float[] scores = new float[k];
        for (int i = 0; i < currentGeneration.Length; i++)
        {
            scores[i] = fitness(currentGeneration[i].GetComponent<BirdController>().getScore());
        }

        GameObject auxGO;
        float auxFloat;
        bool ok;

        do
        {
            ok = false;
            for (int i = 0; i < scores.Length - 1; i++)
                if (scores[i] > scores[i + 1])
                {
                    // Switch scores
                    auxFloat = scores[i];
                    scores[i] = scores[i + 1];
                    scores[i + 1] = auxFloat;

                    // Also switch game objects
                    auxGO = currentGeneration[i];
                    currentGeneration[i] = currentGeneration[i + 1];
                    currentGeneration[i + 1] = auxGO;

                    // Mark ok as true
                    ok = true;
                }
        } while (ok);

        for (int i = 0; i < k; i++)
            best[i] = currentGeneration[i];
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
                    // Idea - create a new Bird and copy the brain from the previous one
                    // TO DO! LE: done

                    // Keep the current bird but reset status
                    newGeneration[i] = oldGeneration[j];
                    newGeneration[i].GetComponent<BirdController>().reset();

                    break;
                }
        }

        // Apply elitism
        //GameObject[] elitismList = getBestK(oldGeneration);

        //for (int i = 0; i < elitismList.Length; i++)
        //    newGeneration[i] = elitismList[i];

    }

    static private void mutation()
    {
        for (int i = 0; i < newGeneration.Length; i++)
            newGeneration[i].GetComponent<BirdController>().getBrain().mutate();
    }

    static private void crossover()
    {
        // TODO!
        // I1 : IH1, HO1
        // I2: IH2, HO2 -> C1: IH1, HO2, C2: IH2, HO1

        // crossover intre ponderi
    }

    // Public function for creating the next generation of gameObjects
    static public GameObject[] getNextGeneration(GameObject[] currentGeneration)
    {
        oldGeneration = currentGeneration;
        // Not tested, may not work
        selection();
        Debug.Log("Finished selection...");
        mutation();
        Debug.Log("Finished mutation...");
        crossover();
        Debug.Log("Finished crossover...");

        return newGeneration;
    }
}
