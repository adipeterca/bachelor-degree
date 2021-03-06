using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script used to control the movement of a bird instance.
public class BirdController : MonoBehaviour
{
    // The jump force multiplier 
    public float jumpForceMultiplier;

    // Is this bird controlled by a player?
    public bool playerControlled;

    // Reference to the rigidbody component used in force application
    private Rigidbody2D rb;

    // Top and bottom margins
    private float top, bottom;

    // Did the bird hit a pipe?
    private bool hitStatus;

    // The score associated with this bird
    private int score;

    // Total number of passed pipes by this individual
    private int passedPipes;

    // Is this GameObject a prefab?
    private bool isPrefab = false;

    // The NeuralNetwork which will make decisions
    private NeuralNetwork brain;

    // Static reference to the closest pipe
    static private GameObject closestPipe;

    void Start()
    {
        // Pick a random color for the bird
        gameObject.GetComponent<SpriteRenderer>().color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        top = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().orthographicSize;

        // Create the brain if it does not exist
        if (brain == null)
        {
            // The neural network will receive as input (must be normalized):
            // - the y position of the bird
            // - the velocity
            // - the x position of the closest pipe
            // - the y position of the top pipe (calculated by adding +2 to the y position of the Pipe object)
            // - the y position of the bottom pipe (calculated by adding -2 to the y position of the Pipe object)
            if (GlobalManager.GetInstance().evolveFromStartBird)
            {
                brain = new NeuralNetwork("startBird.txt");
            }
            else
            {
                brain = new NeuralNetwork(new int[3] { 5, 40, 2 });
            }
        }

        // Assign references
        rb = GetComponent<Rigidbody2D>();

        // Assign values
        InitState();
    }

    void Update()
    {
        // Debug.Log("[DEBUG] [FROM BirdController.Update()] Position: " + transform.position);

        // Don't take any action if the gameObject is marked as a Prefab
        if (isPrefab) return;

        // Make sure the bird stays in between the top and bottom margins
        if (transform.position.y + transform.localScale.y / 10 > top)
        {
            transform.position = new Vector3(transform.position.x, top - transform.localScale.y / 10, transform.position.z);
        }
        else if (transform.position.y - transform.localScale.y / 10 < bottom)
        {
            transform.position = new Vector3(transform.position.x, bottom + transform.localScale.y / 10, transform.position.z);
        }
        else // It means the bird is between the limits
        {
            // If the game is stopped, return
            if (Time.timeScale == 0)
                return;

            // Ignore everything else if the bird hit something
            if (hitStatus)
            {
                // Slide to the left at a constant speed (useful for nice viewing when playing the game)
                transform.position += new Vector3(-5.0f / Time.deltaTime, 0f, 0f);
                return;
            }

            if (playerControlled)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    Jump();
                return;
            }

            // Use the neural network to make predictions
                Matrix inputs = new Matrix(5, 1);
            
            // The y position of the bird (between [-4.5f, 4.5f]
            inputs.set(0, 0, transform.position.y / 4.5f);

            // The y velocity of the bird (divided by 10)
            inputs.set(1, 0, rb.velocity.y / 10.0f);

            if (closestPipe == null || closestPipe.transform.position.x <= transform.position.x)
                FindClosestPipe();

            // The screen width is about 15 units, so normalize it by that value
            inputs.set(2, 0, (closestPipe.transform.position.x - 0.5f) / 15.0f);

            // Set top pipe y position
            inputs.set(3, 0, (closestPipe.transform.position.y + 2.0f) / 4.5f);
            
            // Set bottom pipe y position
            inputs.set(4, 0, (closestPipe.transform.position.y - 2.0f) / 4.5f);

            if (brain.Guess(inputs) == 1)
                Jump();

            // Update the score
            score += 1;
        }
    }

    /// <summary>
    /// Private function used for setting initial values to some variables of this class.
    /// It was created because in Start() only references are set (the Start() function calls this function too).
    /// </summary>
    private void InitState()
    {
        // Other default values
        // Mass = 10
        // Gravity = 3
        // JumpForceMultiplier = 5

        // Initial score value
        score = 0;

        // Initial number of passed pipes
        passedPipes = 0;

        // Set hit status
        hitStatus = false;

        bottom = -top;

        transform.position = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// Private method called when the Jump action is perfomed.
    /// </summary>
    private void Jump()
    {
        CancelVelocity();

        Vector2 jumpForce = new Vector2(0, 10 * jumpForceMultiplier);
        rb.AddRelativeForce(jumpForce);

        // DisplayDebugInfo("Jumped");
    }
    
    /// <summary>
    /// Private method used to stop the player's velocity.
    /// </summary>
    private void CancelVelocity()
    {
        rb.velocity = new Vector2(0, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Don't take any action if the gameObject is marked as a Prefab
        if (isPrefab) return;

        // Ignore future collisions
        if (hitStatus) return;

        if (collision.CompareTag("Pipe"))
        {
            hitStatus = true;
            CancelVelocity();
            // Debug.Log("[DEBUG] [FROM BirdController.OnTriggerEnter2D()] Hit a pipe! Final score: " + score);

            // We don't destroy the gameObject, because we want to access the score for later user in the GA
            // Destroy(gameObject, 2);
            gameObject.SetActive(false);

            // Debug.Log("[DEBUG] [FROM BirdController.OnTriggerEnter2D()] UUID of object set to inactive: " + UUID);
        }
    }

    /// <summary>
    /// Public method which checks whether or not this bird hit a pipe.
    /// </summary>
    /// <returns>true if it did, false otherwise</returns>
    public bool GetHitStatus()
    {
        return hitStatus;
    }

    /// <summary>
    /// Public method called when this individual passes a pipe.
    /// </summary>
    public void IncreasePassedPipes()
    {
        passedPipes++;
    }

    /// <summary>
    /// Public method which marks this object as a prefab (makes it inactive and it does not interact with the enviroment).
    /// </summary>
    public void MarkAsPrefab()
    {
        isPrefab = true;
    }

    /// <summary>
    /// Public method for getting the score of this bird.
    /// </summary>
    /// <returns>the score of this bird</returns>
    public int GetScore()
    {
        return score;
    }

    /// <summary>
    /// Getter for the number of passed pipes by this individual.
    /// </summary>
    /// <returns>number of passed pipes</returns>
    public int GetPassedPipes()
    {
        return passedPipes;
    }

    /// <summary>
    /// Public method used mainly by the GeneticAlgorithm class, used to obtain information about this bird's NeurnalNetwork object.
    /// </summary>
    /// <returns>the NeuralNetwork object associated with this bird</returns>
    public NeuralNetwork GetBrain()
    {
        return brain;
    }

    /// <summary>
    /// Public method for overriding the default brain.<br></br>
    /// Used for TestABird enviroment, when importing an already existing NeuralNetwork object is necessary.
    /// </summary>
    /// <param name="path">path to the file where a NeuralNetwork object was exported</param>
    public void SetBrain(string path)
    {
        brain = new NeuralNetwork(path);
    }

    /// <summary>
    /// Resets the bird to the original position and sets the default values.
    /// </summary>
    public void ResetState()
    {
        // Only reset the values, not the references or other unique stuff
        InitState();

        // Set the game object as active
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Public function for returning a deep copy (a clone) of this particular game object.
    /// It deep clones the Neural Network of the object and also resets it's default values.
    /// </summary>
    /// <returns>the reqeusted clone</returns>
    public GameObject DeepCopy()
    {
        // Stanard cloning
        GameObject clone = GameObject.Instantiate<GameObject>(gameObject);

        // Clone the brain
        clone.GetComponent<BirdController>().brain = new NeuralNetwork(brain);

        // Reset to default values
        clone.GetComponent<BirdController>().ResetState();

        clone.name = "Bird (Clone)";

        return clone;
    }

    /// <summary>
    /// Static function used to find the closest pipe for ALL birds.
    /// The idea is that all birds are on the same X position, so there is no need to recalculate the closest pipe over and over again (it will always
    /// yield the same result).
    /// After calling this function, static variable BirdController.closestPipe will hold the reference.
    /// </summary>
    static private void FindClosestPipe()
    {
        // Constant value of the bird position on the X axis
        float birdPosition = 0.0f;

        // The x position of the closest pipe
        GameObject[] pipes = GameObject.FindGameObjectsWithTag("FullPipe");
        int closest = -1;
        for (int i = 0; i < pipes.Length; i++)
            if (pipes[i].transform.position.x > birdPosition && (closest == -1 || pipes[closest].transform.position.x > pipes[i].transform.position.x))
                closest = i;

        closestPipe = pipes[closest];
    }
}
