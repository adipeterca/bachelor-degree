using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerTest : MonoBehaviour
{
    [Header("References")]
    // Pipe reference for the creation of future objects
    public GameObject pipeReference;

    // Bird reference for the creation of future objects
    public GameObject birdReference;

    // Reference for run count text object
    public Text runCountText;

    // Reference for the passed pipes text object
    public Text currentScoreText;

    // Score of the last run
    public Text lastScoreText;

    // Reference used to play a sound when a new generation of birds is started or when the simulation ended
    public AudioSource newGenerationAudio;

    [Header("Game settings")]
    // The distance between two pipes
    public float distanceBetweenPipes;


    // Pipe reference to the last created pipe
    private GameObject lastCreatedPipe;

    // Initial spawn location for the pipes
    private Vector3 pipesSpawnLocation;

    // Bird reference (the player)
    private GameObject bird;

    // Number of pipes passed in the current run
    private int currentScore = 0;

    // Number of runs
    private int runCount = 1;

    // Score of the last run
    private int lastScore;

    // Constants
    private int TIME_SCALE = 1;

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
        // Mark the reference as a prefab
        pipeReference.GetComponent<PipeController>().MarkAsPrefab();

        pipesSpawnLocation = pipeReference.transform.position + pipeReference.GetComponent<PipeController>().spawnPosition;

        // Set the bird ref as a prefab
        birdReference.GetComponent<BirdController>().MarkAsPrefab();

        // Instantiate the player
        bird = Instantiate(birdReference);
        bird.GetComponent<BirdController>().SetBrain(GlobalManager.GetInstance().pathToBrain);

        // Set the generation count
        runCountText.text = "Run: " + runCount;

        // Set the current score
        currentScoreText.text = "Current score: " + currentScore;

        lastScoreText.text = "Last score: --";
    }
    private void Update()
    {
        // If the game is stopped, return
        if (Time.timeScale == 0)
            return;

        // Set the speed game (DO NOT SPEED UP THE GAME, IT DOES NOT WORK LIKE THAT!)
        Time.timeScale = TIME_SCALE;

        // The bird is dead
        if (bird.GetComponent<BirdController>().GetHitStatus())
        {
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
    /// Private method that handles the restart of an run.
    /// It deletes all pipes, resets the player and plays a sound.
    /// </summary>
    private void RestartGame()
    {
        Time.timeScale = 0;
        Debug.Log("[INFO] Stopped the game!");

        // First delete the pipes
        var pipesToBeDeleted = GameObject.FindGameObjectsWithTag("FullPipe");
        for (int i = 0; i < pipesToBeDeleted.Length; i++)
            if (!pipesToBeDeleted[i].GetComponent<PipeController>().IsMarkedAsPrefab())
                Destroy(pipesToBeDeleted[i]);

        // Reset the bird (player)
        bird.GetComponent<BirdController>().ResetState();

        // Reset the score
        lastScore = currentScore;
        currentScore = 0;
        currentScoreText.text = "Current score: " + currentScore;

        runCount++;
        runCountText.text = "Run count: " + runCount;

        lastScoreText.text = "Last score: " + lastScore;

        lastCreatedPipe = Instantiate(pipeReference);

        // Set the speed game (DO NOT SPEED UP THE GAME, IT DOES NOT WORK LIKE THAT!)
        Time.timeScale = TIME_SCALE;
        Debug.Log("[INFO] Restarted the game!");

        // Play a sound to alert the user that a new generation was started
        newGenerationAudio.Play();
    }

    /// <summary>
    /// Public method for exiting the simulation.
    /// </summary>
    public void QuitTest()
    {
        // Application.Quit();
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Public method which will get called by each pipe that gets destroyed.<br></br>
    /// It increments the current score and also adjust the text associated with it.
    /// </summary>
    public void IncreaseScore()
    {
        currentScore++;
        currentScoreText.text = "Current score: " + currentScore;
    }
}
