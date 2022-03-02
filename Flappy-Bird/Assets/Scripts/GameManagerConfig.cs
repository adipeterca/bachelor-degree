using System.Collections;
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

    // Color for logs tagged with [INFO]
    private string infoColor;

    // Color for logs tagged with [ERROR]
    private string errorColor;

    // Start is called before the first frame update
    void Start()
    {
        infoColor = "#85ff75";
        errorColor = "#ff4040";

        infoText.text = string.Format("<color={0}>[INFO] Leave blank for default values.</color>", infoColor);
    }

    /// <summary>
    /// Public function used to transition from the current scene to the simulation.<br></br>
    /// It validates the given input and sends it to the next scene.
    /// </summary>
    public void StartSimulation()
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
        SceneManager.LoadScene("MainScene");

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
