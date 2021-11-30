using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeController : MonoBehaviour
{
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

    private void Start()
    {
        // Set the speed value
        speed = new Vector3(-10, 0, 0);

        // Get the references
        top = transform.Find("TopPipe");
        bottom = transform.Find("BottomPipe");

        top.transform.position += new Vector3(0, gapSize / 2, 0);
        bottom.transform.position += new Vector3(0, -gapSize / 2, 0);

        float y = Random.Range(-3.0f, 3.0f);

        // Set the spawn location
        transform.position = spawnPosition + new Vector3(0, y, 0);
    }

    public void Update()
    {
        // Don't take any action if the gameObject is marked as a Prefab
        if (isPrefab) return;

        // If the game is stopped, return
        if (Time.timeScale == 0)
            return;

        // Move the object each frame
        transform.Translate(speed * speedMultiplier * Time.deltaTime);

        // Check to see if it should be destroyed
        if (transform.position.x <= destroyPosition.x)
        {
            // Debug.Log("Destroying Pipe " + gameObject.GetInstanceID() + "!");
            Destroy(gameObject);
        }
    }

    // Marks this gameObject as a Prefab and prevents it from being destroyed and from moving.
    public void markAsPrefab()
    {
        isPrefab = true;
    }

}
