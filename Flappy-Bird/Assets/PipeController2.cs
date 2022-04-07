using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script for controlling the behaviour and information for each Pipe2 object.<br></br>
/// It handles movement, initial position, information gathering (for each sensors).
/// </summary>
public class PipeController2 : MonoBehaviour
{
    // The top pipe object
    public GameObject top;

    // The bottom pipe object
    public GameObject bottom;

    // Sensor which will be used in Neural Network prediction
    public GameObject topLeftCorner;

    // Sensor which will be used in Neural Network prediction
    public GameObject bottomLeftCorner;


    // The gap size between the top and bottom pipes
    private float gapSize = 5;

    // Has this pipe already incremented the overall score?
    private bool alreadyIncremented;

    private void Start()
    {
        InitObjectState();
    }

    private void Update()
    {
        if (Time.deltaTime == 0)
            return;

        MoveObject();
        UpdateSceneScore();
    }

    /// <summary>
    /// Private method which handles the movement of this object.
    /// </summary>
    private void MoveObject()
    {
        transform.position += 10.0f * Time.deltaTime * Vector3.left;
    }

    /// <summary>
    /// Private method which checks to see if the score needs to be updated.
    /// </summary>
    private void UpdateSceneScore()
    {
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
    }

    /// <summary>
    /// Private method which initilizez all values needed for this object.<br></br>
    /// It handles the default spawn position and the positioning of the top and bottom pipes.<br></br>
    /// It also deactives the object on spawning.
    /// </summary>
    private void InitObjectState()
    {
        // Random from {-7, -6.5, -6, ..., 6, 6.5, 7}
        float y = Random.Range(-14, 15) * 0.5f;
        transform.position += new Vector3(30, y, 0);

        top.transform.localPosition = new Vector3(0, top.transform.localScale.y + gapSize, 0);
        bottom.transform.localPosition = new Vector3(0, -(bottom.transform.localScale.y + gapSize), 0);

        gameObject.SetActive(false);
    }
}
