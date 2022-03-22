using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PipeController : MonoBehaviour
{
    [Header("Pipe settings")]
    // The initial position at which this object will spawn
    public Vector3 spawnPosition;

    // The position at which this object will be destroyed
    public Vector3 destroyPosition;

    // The gap size
    public float gapSize;

    // The movement speed multiplier for gliding to the left
    public float speedMultiplier;

    // A speed vector
    private Vector3 speed;

    // Top & bottom reference
    private Transform top, bottom;

    // Is this GameObject a prefab?
    private bool isPrefab = false;

    // Has this pipe already been passed by a bird?
    bool alreadyIncremented = false;

    private void Start()
    {
        // Set the speed value
        speed = new Vector3(-10, 0, 0);

        // Get the references
        top = transform.Find("TopPipe");
        bottom = transform.Find("BottomPipe");

        top.transform.position += new Vector3(0, gapSize / 2, 0);
        bottom.transform.position += new Vector3(0, -gapSize / 2, 0);

        float y = Random.Range(-2.0f, 2.0f);

        // Set the spawn location
        transform.position = new Vector3(spawnPosition.x, y, spawnPosition.z);
    }

    public void Update()
    {
        // Don't take any action if the gameObject is marked as a Prefab
        if (isPrefab) return;

        // If the game is stopped, return
        if (Time.timeScale == 0)
            return;

        // Move the object each frame
        transform.Translate(speedMultiplier * Time.deltaTime * speed);

        // Increment number of pipes passed
        if (transform.position.x <= 0 && !alreadyIncremented)
        {
            alreadyIncremented = true;

            if (SceneManager.GetActiveScene().name == "SimulateEvolutionScene")
            {
                GameManagerSimulation.Instance.IncreaseScore();
            }
            else if (SceneManager.GetActiveScene().name == "TestABirdScene")
            {
                GameManagerTest.Instance.IncreaseScore();
            }
            
        }

        // Check to see if it should be destroyed
        if (transform.position.x <= destroyPosition.x)
        {
            // Debug.Log("Destroying Pipe " + gameObject.GetInstanceID() + "!");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Public method that marks this gameObject as a Prefab and prevents it from being destroyed and from moving.
    /// </summary>
    public void MarkAsPrefab()
    {
        isPrefab = true;
    }

    /// <summary>
    /// Is this object a prefab?
    /// </summary>
    /// <returns>true if it is, false otherwise</returns>
    public bool IsMarkedAsPrefab()
    {
        return isPrefab;
    }

}
