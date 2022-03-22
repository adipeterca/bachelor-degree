using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerTest : MonoBehaviour
{
    // Pipe reference for the creation of future objects
    public GameObject pipeReference;

    // Bird reference for the creation of future objects
    public GameObject birdReference;

    // Reference for run count text object
    public Text runCountText;

    // Reference for the passed pipes text object
    public Text passedPipesText;

    // Reference for the highest passed pipes text object
    public Text passedPipesHighestText;

    // Reference used to play a sound when a new generation of birds is started or when the simulation ended
    public AudioSource newGenerationAudio;

    // The distance between two pipes
    public float distanceBetweenPipes;

    // Reference to the Clock text object
    public Text clockText;


    // Pipe reference to the last created pipe
    private GameObject lastCreatedPipe;

    // Initial spawn location for the pipes
    private Vector3 pipesSpawnLocation;

    // Bird reference (the player)
    private GameObject bird;

    // Number of pipes passed in the current run
    private int passedPipesCount = 0;

    // Highest number of pipes passed in all runs
    private int passedPipesHighestCount = 0;

    // Number of maximum frames per second
    private int targetFrameRate = 60;

    // Number of runs
    private int runCount = 1;

    // How much time passed since the simulation started
    private float timePassed = -1f;


    // Constants
    private int TIME_SCALE = 2;

    public static GameManagerTest Instance
    {
        get;
        private set;
    }

    // Don't bother, taken from here
    // https://gamedev.stackexchange.com/questions/116009/in-unity-how-do-i-correctly-implement-the-singleton-pattern
    private void Awake()
    {
        // Make sure that there is always only one reference for the singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Framerate settings
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;

        // Mark the reference as a prefab
        pipeReference.GetComponent<PipeController>().MarkAsPrefab();

        lastCreatedPipe = Instantiate(pipeReference);

        pipesSpawnLocation = pipeReference.transform.position + pipeReference.GetComponent<PipeController>().spawnPosition;

        // Set the bird ref as a prefab
        birdReference.GetComponent<BirdController>().markAsPrefab();

        // Instantiate de player
        bird = Instantiate(birdReference);

        // Set the generation count
        runCountText.text = "Run: " + runCount;

        // Set the current and highest score
        passedPipesText.text = "Current score: " + passedPipesCount;
        passedPipesHighestText.text = "High score: " + passedPipesHighestCount;
    }
    private void Update()
    {
        // If the game is stopped, return
        if (Time.timeScale == 0)
            return;

        // Set the speed game (DO NOT SPEED UP THE GAME, IT DOES NOT WORK LIKE THAT!)
        Time.timeScale = TIME_SCALE;

        // Update time
        SetClockTime();

        // The bird is dead
        if (!bird.activeSelf)
            RestartGame();

        // Check the distance between the last created pipe and the original start point
        if (pipesSpawnLocation.x - lastCreatedPipe.transform.position.x > distanceBetweenPipes)
        {
            lastCreatedPipe = Instantiate(pipeReference);
            // Debug.Log("Created a new pipe!");
        }

    }

    /// <summary>
    /// Private method that handles the restart of an run.
    /// It deletes all pipes, resets the player and plays a sound.
    /// </summary>
    private void RestartGame()
    {
        // First delete the pipes
        var pipesToBeDeleted = GameObject.FindGameObjectsWithTag("FullPipe");
        for (int i = 0; i < pipesToBeDeleted.Length; i++)
            if (!pipesToBeDeleted[i].GetComponent<PipeController>().IsMarkedAsPrefab())
                Destroy(pipesToBeDeleted[i]);

        // Reset the bird (player)
        bird.GetComponent<BirdController>().reset();

        // Reset the score
        passedPipesCount = 0;
        passedPipesText.text = "Current score: " + passedPipesCount;

        lastCreatedPipe = Instantiate(pipeReference);

        // Set the speed game (DO NOT SPEED UP THE GAME, IT DOES NOT WORK LIKE THAT!)
        Time.timeScale = TIME_SCALE;
        Debug.Log("[INFO] Restarted the game!");

        // Play a sound to alert the user that a new generation was started
        newGenerationAudio.Play();
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
    /// Public method for exiting the simulation.
    /// </summary>
    public void QuitSimulation()
    {
        Application.Quit();
    }

    /// <summary>
    /// Public method which will get called by each pipe that gets destroyed.<br></br>
    /// It increments the current score and also adjust the text associated with it.
    /// </summary>
    public void IncreaseScore()
    {
        passedPipesCount++;
        passedPipesText.text = "Current score: " + passedPipesCount;
    }
}
