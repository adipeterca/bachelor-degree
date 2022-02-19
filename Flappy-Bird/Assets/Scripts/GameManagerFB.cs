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

    // Reference for generation count text object
    public Text generationCountText;

    // Reference for current score text object
    public Text currentScoreText;

    // Reference for the number of remianing birds text object
    public Text remainingBirdsText;

    // Reference for the highest score text object
    public Text highestScoreText;

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

    // The highest (or current) score of the current generation
    private int currentScore = 0;

    // Number of remaining birds in the current generation
    private int remainingBirds;

    // Highest score of all generations
    private int highestScore = 0;

    // Time speed up
    // private float timeSpeedUp = 1.0f;

    //private static GameManagerFB instance;

    //public static GameManagerFB getInstance()
    //{
    //    if (instance == null)
    //        instance = new GameManagerFB();
    //    return instance;
    //}

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

        // Set the current score
        currentScoreText.text = "Current score: " + currentScore;

        // Set the number of remaining birds
        remainingBirds = populationSize;
        remainingBirdsText.text = "Remaining birds: " + remainingBirds;

        // Set the highest score
        highestScoreText.text = "Highest score: " + highestScore;

        // Try to speed up the game
        // Time.timeScale = timeSpeedUp;
    }
    private void Update()
    {
        // If the game is stopped, return
        if (Time.timeScale == 0)
            return;

        // Update score information
        currentScore++;
        currentScoreText.text = "Current score: " + currentScore;

        // Update highest score
        if (currentScore > highestScore)
        {
            highestScore = currentScore;
            highestScoreText.text = "Highest score: " + highestScore + "\n(on generation " + numberOfGenerations + ")";
        }

        // Are there any birds left?
        bool noMoreBirds = true;
        remainingBirds = 0;
        for (int i = 0; i < populationSize; i++)
        {
            if (!birds[i].GetComponent<BirdController>().getHitStatus())
            {
                noMoreBirds = false;
                remainingBirds++;
            }
        }

        // Update text information
        remainingBirdsText.text = "Remaining birds: " + remainingBirds;

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
        numberOfGenerations++;
        generationCountText.text = "Generation: " + numberOfGenerations;

        // Set the current score
        currentScore = 0;
        currentScoreText.text = "Current score: " + currentScore;

        // Set the number of remaining birds
        remainingBirds = populationSize;
        remainingBirdsText.text = "Remaining birds: " + remainingBirds;

        // Try to speed up the game
        // Time.timeScale = timeSpeedUp;
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
