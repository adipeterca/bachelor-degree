﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script used to control the movement of a bird instance.
public class BirdController : MonoBehaviour
{
    // The jump force multiplier 
    public float jumpForceMultiplier;

    // Debuging information
    public bool debugInfo;

    // Reference to the rigidbody component used in force application
    private Rigidbody2D rb;

    // Top and bottom margins
    private float top, bottom;

    // Did the bird hit a pipe?
    private bool hitStatus;

    // The score associated with this bird
    private int score;

    // Is this GameObject a prefab?
    private bool isPrefab = false;

    void Start()
    {
        // Pick a random color for the bird
        gameObject.GetComponent<SpriteRenderer>().color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        // Initial score value
        score = 0;

        // Set hit status
        hitStatus = false;

        // Other default values
        // Mass = 10
        // Gravity = 3
        // JumpForceMultiplier = 5

        top = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().orthographicSize;
        bottom = -top;

        // DisplayDebugInfo("Top = " + top + "\nBottom = " + bottom);

        // Assign references
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Don't take any action if the gameObject is marked as a Prefab
        if (isPrefab) return;

        // Make sure the bird stays in between the top and bottom margins
        if (transform.position.y + transform.localScale.y / 10 > top)
        {
            transform.position = new Vector3(transform.position.x, top - transform.localScale.y / 10, transform.position.z);

            // Makes the bird fall instantly
            // CancelVelocity();

            // DisplayDebugInfo("Hit the top!");
        }
        else if (transform.position.y - transform.localScale.y / 10 < bottom)
        {
            transform.position = new Vector3(transform.position.x, bottom + transform.localScale.y / 10, transform.position.z);

            // DisplayDebugInfo("Hit the bottom!");
        }
        else // It means the bird is between the limits
        {
            // If the game is stopped, return
            if (Time.timeScale == 0)
                return;

            // Ignore everything else if the bird hit something
            if (hitStatus)
            {
                // Slide to the left at a constant speed
                transform.position += new Vector3(-5.0f / Time.deltaTime, 0f, 0f);
                return;
            }

            // Check keypresses
            //if (Input.GetMouseButtonDown(0))
            //    Jump();
            
            // Make random choices 
            if (Random.Range(0f, 1f) < 0.01f)
                Jump();

            // Update the score
            score += 1;
        }
    }

    void Jump()
    {
        CancelVelocity();

        Vector2 jumpForce = new Vector2(0, 1000 * jumpForceMultiplier);
        rb.AddRelativeForce(jumpForce);

        // DisplayDebugInfo("Jumped");
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
            DisplayDebugInfo("Hit a pipe! Final score: " + score);

            // We don't destroy the gameObject, because we want to access the score for later user in the GA
            // Destroy(gameObject, 2);
            gameObject.SetActive(false);
        }
    }

    public bool getHitStatus()
    {
        return hitStatus;
    }

    public void markAsPrefab()
    {
        isPrefab = true;
    }

    public int getScore()
    {
        return score;
    }

    public GameObject deepCopy()
    {
        return Instantiate(gameObject);
    }
}
