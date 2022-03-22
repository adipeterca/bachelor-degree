using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Singleton script that handles everything related to the clock/timer object. <br></br>
/// It sets the initial time and accounts for every second that passes.
/// </summary>
public class ClockController : MonoBehaviour
{
    // Reference to the Clock text object
    public Text clockText;

    // How much time passed since the simulation started
    private float timePassed = -1f;

    private ClockController () { }

    public static ClockController Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(Instance);
        else
            Instance = this;
    }
    
    void Update()
    {
        // Update time
        SetClockTime();
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
}
