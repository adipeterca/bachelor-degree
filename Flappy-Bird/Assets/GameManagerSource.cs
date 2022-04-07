using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Default class for every GameManager needed in a Scene.
/// </summary>
public abstract class GameManagerSource : MonoBehaviour
{
    [Header("References")]
    public GameObject birdReference;
    public GameObject pipeReference;
    public AudioSource newRunSound;
}
