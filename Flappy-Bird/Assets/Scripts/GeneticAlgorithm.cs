using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Utility class for generating new generations for a given population
public class GeneticAlgorithm
{
    // The number of individuals which are going to be selected for elitism
    static private int k = 10;

    // The chance that two individuals will perform crossover
    static private float crossoverChance = 0.4f;

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

    /// <summary>
    /// Private function for getting the best K individuals (based on their fitness) from a given generation of objects.
    /// </summary>
    /// <param name="currentGeneration">the generation to choose from</param>
    /// <returns>a list containing the best K individuals</returns>
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
                    // Clone the bird
                    newGeneration[i] = oldGeneration[j].GetComponent<BirdController>().deepCopy();
                    break;
                }
        }

        //// Apply elitism
        //GameObject[] elitismList = getBestK(oldGeneration);

        //for (int i = 0; i < elitismList.Length; i++)
        //    newGeneration[i] = elitismList[i];

    }

    static private void mutation()
    {
        for (int i = 0; i < newGeneration.Length; i++)
        {
            newGeneration[i].GetComponent<BirdController>().getBrain().mutate();
        }
    }

    /// <summary>
    /// Private function that does the crossover over the new generation of individuals.
    /// </summary>
    static private void crossover()
    {
        // A list containing the chances of each individual to be selected for crossover
        float[] chances = new float[newGeneration.Length];
        for (int i = 0; i < chances.Length; i++)
            chances[i] = Random.Range(0.0f, 1.0f);

        // Sort both lists (individuals and chances alike) in ascending order of chances
        float auxFloat;
        GameObject auxGameObject;
        bool ok;

        do
        {
            ok = false;
            for (int i = 0; i < chances.Length - 1; i++)
                if (chances[i] > chances[i + 1])
                {
                    // Switch chances
                    auxFloat = chances[i];
                    chances[i] = chances[i + 1];
                    chances[i + 1] = auxFloat;

                    // Switch gameObjects
                    auxGameObject = newGeneration[i];
                    newGeneration[i] = newGeneration[i + 1];
                    newGeneration[i + 1] = auxGameObject;

                    // Set ok
                    ok = true;
                }
        } while (ok);

        // Perform crossover only if the chance is smaller than the predefined crossoverChance in the class
        for (int i = 0; i < chances.Length - 1 && chances[i] <= GeneticAlgorithm.crossoverChance; i += 2)
        {
            // If both individuals have chances smaller than the predefined crossoverChance, do crossover
            if (chances[i + 1] < GeneticAlgorithm.crossoverChance)
            {
                NeuralNetwork.crossover(
                    newGeneration[i].GetComponent<BirdController>().getBrain(),
                    newGeneration[i + 1].GetComponent<BirdController>().getBrain()
                    );
            }
            else
            {
                // If the second individual has a higher chance than the crossoverChance
                if (Random.Range(0, 1) == 1)
                {
                    NeuralNetwork.crossover(
                        newGeneration[i].GetComponent<BirdController>().getBrain(),
                        newGeneration[i + 1].GetComponent<BirdController>().getBrain()
                    );
                }
            }
        }
    }

    // Public function for creating the next generation of gameObjects
    static public GameObject[] getNextGeneration(GameObject[] currentGeneration)
    {
        // Change made to highlight the need of a deep copy function
        // return currentGeneration;

        oldGeneration = currentGeneration;

        // Not tested, may not work
        selection();
        Debug.Log("Finished selection...");
        mutation();
        Debug.Log("Finished mutation...");
        crossover();
        Debug.Log("Finished crossover...");

        // Destroy the old generation of birds
        for (int i = 0; i < oldGeneration.Length; i++)
            GameObject.Destroy(oldGeneration[i]);
        
        return newGeneration;
    }
}
