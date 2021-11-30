using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerFB : MonoBehaviour
{
    // Pipe reference for the creation of future objects
    public GameObject pipeReference;

    // Bird reference for the creation of future objects
    public GameObject birdReference;

    // Number of birds (initial population size)
    public int populationSize;

    // It should have a starting speed that updates every frame, to speed up the game

    // The distance between two pipes
    public float distanceBetweenPipes;

    // Pipe reference to the last created pipe
    private GameObject lastCreatedPipe;

    // Initial spawn location for the pipes
    private Vector3 pipesSpawnLocation;

    // Bird reference list
    private GameObject[] birds;

    private void Start()
    {
        // Mark the reference as a prefab
        pipeReference.GetComponent<PipeController>().markAsPrefab();

        lastCreatedPipe = Instantiate(pipeReference);

        pipesSpawnLocation = pipeReference.transform.position + pipeReference.GetComponent<PipeController>().spawnPosition;

        // Set the bird ref as a prefab
        birdReference.GetComponent<BirdController>().markAsPrefab();

        // Instantiate de birds
        birds = new GameObject[populationSize];
        for (int i = 0; i < populationSize; i++)
            birds[i] = Instantiate(birdReference);

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
            Debug.Log("Stopped the game!");
            return;
        }

        // Check the distance between the last created pipe and the original start point
        if (pipesSpawnLocation.x - lastCreatedPipe.transform.position.x > distanceBetweenPipes)
        {
            lastCreatedPipe = Instantiate(pipeReference);
            // Debug.Log("Created a new pipe!");
        }
        
    }
}
