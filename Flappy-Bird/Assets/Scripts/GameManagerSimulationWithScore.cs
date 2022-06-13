using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerSimulationWithScore : MonoBehaviour
{
    [Header("References")]
    // Pipe reference for the creation of future objects
    public GameObject pipeReference;

    // Bird reference for the creation of future objects
    public GameObject birdReference;

    // Reference for generation count text object
    public Text generationCountText;

    // Reference for current score text object
    public Text currentScoreText;

    // Reference for the highest score text object
    public Text highestScoreText;

    // Reference for the number of remianing birds text object
    public Text remainingBirdsText;

    // Reference used to play a sound when a new generation of birds is started or when the simulation ended
    public AudioSource newGenerationAudio;

    [Header("Genetic Algorithm settings")]
    // Number of birds (initial population size)
    public int populationSize;

    // The maximum score (the game stops at it)
    public int targetScore;

    [Header("Game settings")]

    // The distance between two pipes
    public float distanceBetweenPipes;


    // Pipe reference to the last created pipe
    private GameObject lastCreatedPipe;

    // Initial spawn location for the pipes
    private Vector3 pipesSpawnLocation;

    // Bird reference list
    private GameObject[] birds;

    // Number of generations
    private int generationCount = 1;

    // The highest (or current) score of the current generation
    private int currentScore = 0;

    // Highest score of all generations
    private int highestScore = 0;

    // Number of remaining birds in the current generation
    private int remainingBirds;

    // Best performing bird of all times
    private BestBird bestBird = new BestBird();

    // Filename for each run to store information into
    private string filenameInfo = "ScoreSimulation_" + System.DateTime.Now.Ticks + ".txt";

    private GameManagerSimulationWithScore() { }

    /// <summary>
    /// Public static method for retrieving the only instance of the GameManagerSimulationWithScore.
    /// </summary>
    public static GameManagerSimulationWithScore Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        // Load values from ConfigScene
        targetScore = GlobalManager.GetInstance().targetScore;
        populationSize = GlobalManager.GetInstance().populationSize;

        // Mark the reference as a prefab
        pipeReference.GetComponent<PipeController>().MarkAsPrefab();

        pipesSpawnLocation = pipeReference.transform.position + pipeReference.GetComponent<PipeController>().spawnPosition;

        // Set the bird ref as a prefab
        birdReference.GetComponent<BirdController>().MarkAsPrefab();

        // Instantiate de birds
        if (birds == null)
        {
            birds = new GameObject[populationSize];
            for (int i = 0; i < populationSize; i++)
                birds[i] = Instantiate(birdReference);
        }

        // Set the generation count
        generationCountText.text = "Generation: " + generationCount;

        // Set the current score
        currentScoreText.text = "Current score: " + currentScore;

        // Set the number of remaining birds
        remainingBirds = populationSize;
        remainingBirdsText.text = "Remaining birds: " + remainingBirds;

        // Set the highest score
        highestScoreText.text = "Highest score: " + highestScore;

        // Debug.Log("[DEBUG] [FROM GameManagerFB.Start()] Global Manager pop size : " + GlobalManager.GetInstance().populationSize);
    }
    private void Update()
    {
        // If the game is stopped, return
        if (Time.timeScale == 0)
            return;

        // Set the speed game (DO NOT SPEED UP THE GAME, IT DOES NOT WORK LIKE THAT!)
        Time.timeScale = 1;

        // Are there any birds left?
        bool noMoreBirds = true;
        remainingBirds = 0;
        for (int i = 0; i < populationSize; i++)
        {
            if (!birds[i].GetComponent<BirdController>().GetHitStatus())
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
            // Terminate the application as soon as it has reached the target score
            if (currentScore >= targetScore)
                QuitSimulation();

            RestartGame();
            return;
        }

        // Check the distance between the last created pipe and the original start point
        if (lastCreatedPipe == null || pipesSpawnLocation.x - lastCreatedPipe.transform.position.x > distanceBetweenPipes)
        {
            lastCreatedPipe = Instantiate(pipeReference);
            // Debug.Log("Created a new pipe!");
        }

    }

    /// <summary>
    /// Private method for handling the restart of a game iteration.<br></br>
    /// It stops the game, writes the output log, retains the best bird (if any), creates the next generation of birds and repositions 
    /// them and the pipes. After all calculations are done, it plays a sound to alert the user.
    /// </summary>
    private void RestartGame()
    {
        Time.timeScale = 0;
        Debug.Log("[INFO] Stopped the game!");

        // Write relevant information to INFO file
        using (System.IO.StreamWriter foutInfo = System.IO.File.AppendText(filenameInfo))
        {
            foutInfo.WriteLine("Iteration for generation: " + generationCount);
            foutInfo.WriteLine("Number of individuals: " + birds.Length);
            foutInfo.WriteLine("Highest score (number of passed pipes): " + currentScore);

            if (currentScore != 0)
            {
                // Here we make an average of the number of passed pipes, because
                // it is more important and relevant than the score (points), which is used
                // only by the fitness function.
                float totalPassedPipes = 0;
                for (int i = 0; i < birds.Length; i++)
                {
                    totalPassedPipes += birds[i].GetComponent<BirdController>().GetPassedPipes();
                }
                Debug.Log("[DEBUG] [FROM GameManagerSimulationWithScore.RestartGame()] totalPassedPipes : " + totalPassedPipes);

                float mean = totalPassedPipes / birds.Length;
                foutInfo.WriteLine("Average number of passed pipes: " + mean);

                int birdsOverMean = 0;
                for (int i = 0; i < birds.Length; i++)
                    if (birds[i].GetComponent<BirdController>().GetPassedPipes() >= mean)
                        birdsOverMean++;
                foutInfo.WriteLine("Number of birds which passed more pipes, on average: " + birdsOverMean);
            }
            foutInfo.WriteLine("-------------------------------------------------------------------------------\n");
        }

        // Retain a copy of the best bird until this moment
        for (int i = 0; i < birds.Length; i++)
            if (bestBird == null || bestBird.score < birds[i].GetComponent<BirdController>().GetScore())
            {
                bestBird.brain = new NeuralNetwork(birds[i].GetComponent<BirdController>().GetBrain());
                bestBird.score = birds[i].GetComponent<BirdController>().GetScore();
            }

        birds = GeneticAlgorithm.GetNextGeneration(birds);

        // First delete the pipes
        var pipesToBeDeleted = GameObject.FindGameObjectsWithTag("FullPipe");
        for (int i = 0; i < pipesToBeDeleted.Length; i++)
            if (!pipesToBeDeleted[i].GetComponent<PipeController>().IsMarkedAsPrefab())
                Destroy(pipesToBeDeleted[i]);

        // Then reset all the birds
        for (int i = 0; i < birds.Length; i++)
        {
            birds[i].GetComponent<BirdController>().ResetState();
        }

        lastCreatedPipe = Instantiate(pipeReference);

        // Set the generation count
        generationCount++;
        generationCountText.text = "Generation: " + generationCount;

        // Set the current score
        currentScore = 0;
        currentScoreText.text = "Current score: " + currentScore;

        // Set the number of remaining birds
        remainingBirds = populationSize;
        remainingBirdsText.text = "Remaining birds: " + remainingBirds;

        // Set the speed game (DO NOT SPEED UP THE GAME, IT DOES NOT WORK LIKE THAT!)
        Time.timeScale = 1;
        // Debug.Log("[INFO] Restarted the game!");

        // Play a sound to alert the user that a new generation was started
        newGenerationAudio.Play();
    }

    /// <summary>
    /// Private method for exiting the simulation.<br></br>
    /// It also exports the best bird so far, if any.
    /// </summary>
    public void QuitSimulation()
    {
        // Export the best bird
        if (bestBird.score != BestBird.defaultScore)
            bestBird.brain.Export("bestBird.txt");

        // Do not quit. Rather, return to main menu.
        // Application.Quit();
        SceneManager.LoadScene(0);
    }


    /// <summary>
    /// Public method used to update the score and to handle its display on the screen.<br></br>
    /// This function will be called by each Pipe that gets passed
    /// </summary>
    public void IncreaseScore()
    {
        // For each active bird, increase it's number of passed pipes.
        for (int i = 0; i < birds.Length; i++)
            if (!birds[i].GetComponent<BirdController>().GetHitStatus())
                birds[i].GetComponent<BirdController>().IncreasePassedPipes();

        // Update score information
        currentScore++;
        currentScoreText.text = "Current score: " + currentScore;

        // Update highest score
        if (currentScore > highestScore)
        {
            highestScore = currentScore;
            highestScoreText.text = "Highest score: " + highestScore + "\n(on generation " + generationCount + ")";
        }
    }
}
