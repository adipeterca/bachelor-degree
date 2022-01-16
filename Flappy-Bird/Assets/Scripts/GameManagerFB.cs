using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerFB : MonoBehaviour
{
    // Pipe reference for the creation of future objects
    public GameObject pipeReference;

    // Bird reference for the creation of future objects
    public GameObject birdReference;

    // Number of birds (initial population size)
    public int populationSize;

    // Reference for generation count text
    public Text generationCountText;

    // It should have a starting speed that updates every frame, to speed up the game

    // The distance between two pipes
    public float distanceBetweenPipes;

    // Pipe reference to the last created pipe
    private GameObject lastCreatedPipe;

    // Initial spawn location for the pipes
    private Vector3 pipesSpawnLocation;

    // Bird reference list
    private GameObject[] birds;

    // Number of maximum frames per second
    private int targetFrameRate = 60;

    // Number of generations
    private int numberOfGenerations = 1;

    private void Start()
    {
        // Framerate settings
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;

        // Mark the reference as a prefab
        pipeReference.GetComponent<PipeController>().markAsPrefab();

        lastCreatedPipe = Instantiate(pipeReference);

        pipesSpawnLocation = pipeReference.transform.position + pipeReference.GetComponent<PipeController>().spawnPosition;

        // Set the bird ref as a prefab
        birdReference.GetComponent<BirdController>().markAsPrefab();

        // Instantiate de birds
        if (birds == null)
        {
            birds = new GameObject[populationSize];
            for (int i = 0; i < populationSize; i++)
                birds[i] = Instantiate(birdReference);
        }

        // Set the generation count
        generationCountText.text = "Generation: " + numberOfGenerations;

        // Try to speed up the game
        Time.timeScale = 2.0f;
    }
    private void Update()
    {
        // If the game is stopped, return
        if (Time.timeScale == 0)
            return;

        // Are there any birds left?
        bool noMoreBirds = true;
        for (int i = 0; i < populationSize; i++)
        {
            if (!birds[i].GetComponent<BirdController>().getHitStatus())
                noMoreBirds = false;
        }

        // If no more birds are left, restart the iteration
        if (noMoreBirds)
        {
            Time.timeScale = 0;
            Debug.Log("[INFO] Stopped the game!");

            // 13 JAN
            // NeuralNetwork[] brains = new NeuralNetork[birds.Length];
            // brains = GeneticAlgorithm.getNextGeneration(brains);
            // foreach (int i = 0; i < birds.Length; i++)
            //      birds[i] = Instantiate(birdReference);
            //      birds[i].setBrain(brains[i]); // maybe implement smth like NeuralNetwork.DeepCopy(brains[i])

            birds = GeneticAlgorithm.getNextGeneration(birds);

            //Debug.Log("[DEBUG] Stopped the game for debugging reasons.");
            //Time.timeScale = 0;

            restartGame();
            return;
        }

        // Check the distance between the last created pipe and the original start point
        if (pipesSpawnLocation.x - lastCreatedPipe.transform.position.x > distanceBetweenPipes)
        {
            lastCreatedPipe = Instantiate(pipeReference);
            // Debug.Log("Created a new pipe!");
        }
        
    }

    // Delete all pipes (except for the pipeRef), reposition all the birds
    private void restartGame()
    {
        // First delete the pipes
        var pipesToBeDeleted = GameObject.FindGameObjectsWithTag("FullPipe");
        for (int i = 0; i < pipesToBeDeleted.Length; i++)
            if (!pipesToBeDeleted[i].GetComponent<PipeController>().isMarkedAsPrefab())
                Destroy(pipesToBeDeleted[i]);

        // Then reset all the birds
        for (int i = 0; i < birds.Length; i++)
        {
            birds[i].GetComponent<BirdController>().reset();
        }

        lastCreatedPipe = Instantiate(pipeReference);

        Time.timeScale = 1;

        // Set the generation count
        generationCountText.text = "Generation: " + numberOfGenerations;

        // Try to speed up the game
        Time.timeScale = 2.0f;
        Debug.Log("[INFO] Restarted the game!");
    }

    /// <summary>
    /// Debug function for displaying to console how many birds are active (as GameObjects).
    /// </summary>
    /// <param name="time">at which moment does the calculation take place (can be used as a unique identifier when debugging)</param>
    private void debugActiveBirds(string time)
    {
        int activeBirds = 0;
        foreach (var bird in birds)
            if (!bird.GetComponent<BirdController>().getHitStatus())
                activeBirds++;

        Debug.Log("active birds " + time + ": " + activeBirds);
    }
}
