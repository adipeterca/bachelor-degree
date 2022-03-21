﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerConfig : MonoBehaviour
{
    // Reference for MaxGenerationCountText object
    public Text maxGenCountText;

    // Reference for PopulationSizeText object
    public Text populationSizeText;

    // Reference for InfoText object
    public Text infoText;

    // Reference for the Dropdown object
    public Dropdown dropdownObject;

    // Public reference to the PathText object
    public Text pathText;

    // List of different menus
    [Tooltip("index 0 - simulate evolution\nindex 1 - test a bird")]
    public GameObject[] menuArray;

    // Color for logs tagged with [INFO]
    private string infoColor;

    // Color for logs tagged with [ERROR]
    private string errorColor;

    // The type of menu to display.
    // 0 means "Simulate evolution"
    // 1 means "Test a bird"
    private int menuType = 0;

    // Start is called before the first frame update
    void Start()
    {
        infoColor = "#85ff75";
        errorColor = "#ff4040";

        infoText.text = string.Format("<color={0}>[INFO] Select the type of simulation you want to run.</color>", infoColor);

        UpdateSelection();
    }

    /// <summary>
    /// Public method which handles the type of menu to display ("Run a bird" or "Simulate evolution").
    /// </summary>
    public void UpdateSelection()
    {
        string selected = dropdownObject.options[dropdownObject.value].text;

        Debug.Log("Selected " + selected);

        if (selected == "Simulate evolution")
            menuType = 0;
        else if (selected == "Test a bird")
            menuType = 1;

        // Iterate through all menus and hide the ones which are not needed.
        for (int i = 0; i < menuArray.Length; i++)
        {
            menuArray[i].SetActive(false);
        }

        // Display only the selected one
        menuArray[menuType].SetActive(true);
    }

    /// <summary>
    /// Public method which handles what type of simulation to start, based on the user input values.
    /// </summary>
    public void StartSimulation()
    {
        if (menuType == 0)
            SimulateEvolution();
        else if (menuType == 1)
            TestABird();
    }

    /// <summary>
    /// Private function used to transition from the current scene to the simulation.<br></br>
    /// It validates the given input and sends it to the next scene.
    /// </summary>
    private void SimulateEvolution()
    {
        string info = "";
        bool containsErrors = false;
        if (maxGenCountText.text == "" || ValidateInput(maxGenCountText.text))
            info += string.Format("<color={0}>[INFO] Generation count number : " + (maxGenCountText.text == "" ? "150" : maxGenCountText.text) + "</color>\n", infoColor);
        else
        {
            info += string.Format("<color={0}>[ERROR] Max generation count must be a positive number!</color>\n", errorColor);
            containsErrors = true;
        }
        if (populationSizeText.text == "" || ValidateInput(populationSizeText.text))
            info += string.Format("<color={0}>[INFO] Population size number : " + (populationSizeText.text == "" ? "100" : populationSizeText.text) + "</color>\n", infoColor);
        else
        {
            info += string.Format("<color={0}>[ERROR] Population size must be a positive number!</color>\n", errorColor);
            containsErrors = true;
        }

        // Display the information
        infoText.text = info;

        // Do not advance if there are errors present
        if (containsErrors)
            return;

        int maxGenCount = (maxGenCountText.text == "" ? 150 : int.Parse(maxGenCountText.text));
        int populationSize = (populationSizeText.text == "" ? 100 : int.Parse(populationSizeText.text));

        // Store the values for future use
        GlobalManager.GetInstance().maxGenerationCount = maxGenCount;
        GlobalManager.GetInstance().populationSize = populationSize;

        infoText.text += string.Format("<color={0}>[INFO] Starting the simulation...</color>\n", infoColor);

        StartCoroutine(LoadNextScene());
    }

    /// <summary>
    /// Private method which handles the transition to the testing area.<br></br>
    /// It validates the given path and sends it to the next scene.
    /// </summary>
    private void TestABird()
    {
        string info = "";
        bool containsErrors = false;

        // Assign default value "./brain.txt"
        string filepath = (pathText.text == "" ? "./brain.txt" : pathText.text);
        if (System.IO.File.Exists(filepath))
        {
            info += string.Format("<color={0}>[INFO] File path set to '" + filepath + "'.</color>\n", infoColor);
        }
        else
        {
            info += string.Format("<color={0}>[ERROR] Invalid file path '" + filepath + "'.</color>\n", errorColor);
            containsErrors = true;
        }

        // Display the information
        infoText.text = info;

        // Do not advance if there are errors present
        if (containsErrors)
            return;

        GlobalManager.GetInstance().pathToBrain = filepath;

        infoText.text += string.Format("<color={0}>[INFO] Starting the simulation...</color>\n", infoColor);

        StartCoroutine(LoadNextScene());
    }

    /// <summary>
    /// Private method for creating a wait effect for the next scene.
    /// </summary>
    private IEnumerator LoadNextScene()
    {
        // Wait for 3 seconds
        yield return new WaitForSeconds(1);
        infoText.text += string.Format("<color={0}>[INFO] 3...</color>\n", infoColor);
        yield return new WaitForSeconds(1);
        infoText.text += string.Format("<color={0}>[INFO] 2...</color>\n", infoColor);
        yield return new WaitForSeconds(1);
        infoText.text += string.Format("<color={0}>[INFO] 1...</color>\n", infoColor);

        // Start the next scene
        if (menuType == 0)
            SceneManager.LoadScene("SimulateEvolutionScene");
        else if (menuType == 1)
            SceneManager.LoadScene("TestABirdScene");

    }

    /// <summary>
    /// Private function used to validate a given number.<br></br>
    /// </summary>
    /// <param name="number">the number to validate</param>
    /// <returns>true if the number is positive, false otherwise</returns>
    private bool ValidateInput(string number)
    {
        return number[0] != '-';
    }
}
