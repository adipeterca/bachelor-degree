using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Utility class for storing information about the best performing bird.
/// </summary>
public class BestBird
{
    public NeuralNetwork brain;
    public int score;
}

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

    // Reference for the passed pipes text object
    public Text passedPipesText;

    // Reference for the highest passed pipes text object
    public Text passedPipesHighestText;

    // Reference used to play a sound when a new generation of birds is started or when the simulation ended
    public AudioSource newGenerationAudio;

    // The distance between two pipes
    public float distanceBetweenPipes;

    // The maximum number of generations (the game stops as soon as it has reached this point)
    public int maxGenerationCount;

    // Reference to the Clock text object
    public Text clockText;


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

    // Best performing bird of all times
    private BestBird bestBird = new BestBird();

    // How much time passed since the simulation started
    private float timePassed = -1f;

    // Constants
    private int TIME_SCALE = 2;

    //private static GameManagerFB instance;

    //public static GameManagerFB getInstance()
    //{
    //    if (instance == null)
    //        instance = new GameManagerFB();
    //    return instance;
    //}

    // Filename for each run to store information into
    private string filenameInfo = "INFO_run_" + System.DateTime.Now.Ticks + ".txt";

    private void Start()
    {
        // Framerate settings
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;

        // Load values from ConfigScene
        maxGenerationCount = GlobalManager.GetInstance().maxGenerationCount;
        populationSize = GlobalManager.GetInstance().populationSize;

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
        generationCountText.text = "Generation: " + numberOfGenerations + " / " + maxGenerationCount;

        // Set the current score
        currentScoreText.text = "Current score: " + currentScore;

        // Set the number of remaining birds
        remainingBirds = populationSize;
        remainingBirdsText.text = "Remaining birds: " + remainingBirds;

        // Set the highest score
        highestScoreText.text = "Highest score: " + highestScore;

        // Set the number of passed pipes
        passedPipesText.text = "0";
        passedPipesHighestText.text = "0";

        // Debug.Log("[DEBUG] [FROM GameManagerFB.Start()] Global Manager pop size : " + GlobalManager.GetInstance().populationSize);
    }
    private void Update()
    {
        // Terminate the application as soon as it has reached the maximum number of generations
        if (numberOfGenerations == maxGenerationCount)
            quitSimulation();

        // If the game is stopped, return
        if (Time.timeScale == 0)
            return;

        // Set the speed game (DO NOT SPEED UP THE GAME, IT DOES NOT WORK LIKE THAT!)
        Time.timeScale = TIME_SCALE;

        // Update score information
        currentScore++;
        currentScoreText.text = "Current score: " + currentScore;

        // Update highest score
        if (currentScore > highestScore)
        {
            highestScore = currentScore;
            highestScoreText.text = "Highest score: " + highestScore + "\n(on generation " + numberOfGenerations + ")";
        }

        // Update time
        SetClockTime();

        // Adjust the maximum number of passed pipes (if neccesary)
        int currentPassedPipes = int.Parse(passedPipesText.text);
        if (currentPassedPipes > int.Parse(passedPipesHighestText.text))
        {
            passedPipesHighestText.text = currentPassedPipes + "";
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

            // Write relevant information to INFO file
            using (System.IO.StreamWriter foutInfo = System.IO.File.AppendText(filenameInfo))
            {
                foutInfo.WriteLine("Highest score: " + currentScore);

                int totalScore = 0;
                for (int i = 0; i < birds.Length; i++)
                {
                    totalScore += birds[i].GetComponent<BirdController>().getScore();
                    Debug.Log("[DEBUG] [FROM GameManagerFB.restartGame()] totalScore: " + totalScore);
                }
                float mean = ((float)totalScore) / birds.Length;
                foutInfo.WriteLine("Mean score: " + mean);

                int birdsOverMean = 0;
                for (int i = 0; i < birds.Length; i++)
                    if (birds[i].GetComponent<BirdController>().getScore() >= mean)
                        birdsOverMean++;
                foutInfo.WriteLine("Number of birds with a score over mean score: " + birdsOverMean);

                foutInfo.WriteLine("Pipes passed: " + passedPipesText.text);
                foutInfo.WriteLine("-------------------------------------------------------------------------------\n\n");
            }

            // Retain a copy of the best bird until this moment
            for (int i = 0; i < birds.Length; i++)
                if (bestBird == null || bestBird.score < birds[i].GetComponent<BirdController>().getScore())
                {
                    bestBird.brain = new NeuralNetwork(birds[i].GetComponent<BirdController>().getBrain());
                    bestBird.score = birds[i].GetComponent<BirdController>().getScore();
                }

            birds = GeneticAlgorithm.getNextGeneration(birds);

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

        // Time.timeScale = 1;

        // Set the generation count
        numberOfGenerations++;
        generationCountText.text = "Generation: " + numberOfGenerations;

        // Set the current score
        currentScore = 0;
        currentScoreText.text = "Current score: " + currentScore;

        // Set the number of remaining birds
        remainingBirds = populationSize;
        remainingBirdsText.text = "Remaining birds: " + remainingBirds;

        // Set the number of passed pipes
        passedPipesText.text = "0";

        // Set the speed game (DO NOT SPEED UP THE GAME, IT DOES NOT WORK LIKE THAT!)
        Time.timeScale = TIME_SCALE;
        Debug.Log("[INFO] Restarted the game!");

        // Play a sound to alert the user that a new generation was started
        newGenerationAudio.Play();
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

    /// <summary>
    /// Private method that handles the clock time display and calculations.
    /// </summary>
    private void SetClockTime()
    {
        // This also takes in account the time that the ConfigScene was loaded
        if (timePassed == -1f)
        {
            timePassed = Time.realtimeSinceStartup;
            return;
        }

        // Number of seconds for the simulation ONLY (ConfigScene not included)
        int realtime = (int)(Time.realtimeSinceStartup - timePassed);

        string hour = (realtime / 3600).ToString().PadLeft(2, '0');
        string minute = (realtime % 3600 / 60).ToString().PadLeft(2, '0');
        string second = (realtime % 60).ToString().PadLeft(2, '0');
        
        clockText.text = hour + ":" + minute + ":" + second;
    }

    /// <summary>
    /// Private method for exiting the simulation.<br></br>
    /// It also exports the best bird so far, if any.
    /// </summary>
    public void quitSimulation()
    {
        Debug.Log("[INFO] [FROM GameManagerFB.quitSimulation()] Simulation over!");

        // Export the best bird
        bestBird.brain.export("bestBird.txt");

        Application.Quit();
    }
}
