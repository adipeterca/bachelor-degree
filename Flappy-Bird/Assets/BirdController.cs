using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script used to control the movement of a bird instance.
public class BirdController : MonoBehaviour
{
    // The score associated with this bird
    [HideInInspector]
    public int score;

    // The jump force multiplier 
    public float jumpForceMultiplier;

    // Debuging information
    public bool debugInfo;

    // Reference to the rigidbody component used in force application
    private Rigidbody2D rb;

    // Top and bottom margins
    private float top, bottom;

    void Start()
    {
        // Initial score value
        score = 0;

        top = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().orthographicSize;
        bottom = -top;

        // Other default values
        // Mass = 10
        // Gravity = 3
        // JumpForceMultiplier = 5

        DisplayDebugInfo("Top = " + top + "\nBottom = " + bottom);

        // Assign references
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Make sure the bird stays in between the top and bottom margins
        if (transform.position.y + transform.localScale.y / 10 > top)
        {
            transform.position = new Vector3(transform.position.x, top - transform.localScale.y / 10, transform.position.z);

            // Makes the bird fall instantly
            // CancelVelocity();

            DisplayDebugInfo("Hit the top!");
        }
        else if (transform.position.y - transform.localScale.y / 10 < bottom)
        {
            transform.position = new Vector3(transform.position.x, bottom + transform.localScale.y / 10, transform.position.z);

            DisplayDebugInfo("Hit the bottom!");
        }
        else // It means the bird is between the limits
        {
            // Check keypresses
            if (Input.GetMouseButtonDown(0))
                Jump();
        }
    }

    void Jump()
    {
        CancelVelocity();

        Vector2 jumpForce = new Vector2(0, 1000 * jumpForceMultiplier);
        rb.AddRelativeForce(jumpForce);

        DisplayDebugInfo("Jumped");
    }
    void CancelVelocity()
    {
        rb.velocity = new Vector2(0, 0);
    }

    void DisplayDebugInfo(string message)
    {
        if (!debugInfo) return;
        Debug.Log(message);
    }
}
