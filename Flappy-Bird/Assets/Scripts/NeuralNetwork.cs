using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Neural network class used in the NeuroEvolution algorithm
/// </summary>
public class NeuralNetwork
{
    private Matrix[] weightsArray;
    private Matrix[] biasArray;

    // For debugging purposes
    public System.Guid UUID = System.Guid.NewGuid();

    /// <summary>
    /// Create a neural network with the configuration provided.<br></br>
    /// Each element of the array represents how many nodes that layer must have.<br></br>
    /// The weights are initilized using a Xavier approach.<br></br>
    /// The biases are between 0 and 1.
    /// </summary>
    /// <param name="layers">integer vector representing how many nodes each layer should have</param>
    public NeuralNetwork(int[] layers)
    {
        weightsArray = new Matrix[layers.Length - 1];
        biasArray = new Matrix[layers.Length - 1];

        for (int i = 0; i < layers.Length - 1; i++)
        {
            //float min = -(1.0f / Mathf.Sqrt(layers[i]));
            //float max = (1.0f / Mathf.Sqrt(layers[i]));

            // Weights initialization
            weightsArray[i] = new Matrix(layers[i + 1], layers[i], -1.0f, 1.0f);
            // weightsArray[i] = new Matrix(layers[i + 1], layers[i], min, max);

            // Bias initialization
            biasArray[i] = new Matrix(layers[i + 1], 1, 0f, 1f);
        }
    }

    /// <summary>
    /// Construct a new neural network from a given one
    /// </summary>
    /// <param name="n">the neural network to copy</param>
    public NeuralNetwork(NeuralNetwork n)
    {
        weightsArray = new Matrix[n.weightsArray.Length];
        biasArray = new Matrix[n.biasArray.Length];

        for (int i = 0; i < weightsArray.Length; i++)
        {
            weightsArray[i] = new Matrix(n.weightsArray[i]);
            biasArray[i] = new Matrix(n.biasArray[i]);
        }
    }

    /// <summary>
    /// Creates a NeuralNetwork object from a given file, if the format is respected.<br></br>
    /// </summary>
    /// <param name="filePath">the file from which to read data</param>
    public NeuralNetwork(string filePath)
    {
        System.IO.StreamReader fin = new System.IO.StreamReader(filePath);
        
        int rows, columns;
        string[] lines;
        string[] values;

        lines = fin.ReadToEnd().Split('\n');

        weightsArray = new Matrix[lines.Length];
        biasArray = new Matrix[lines.Length];

        for (int lineCount = 0; lineCount < lines.Length; lineCount++)
        {
            values = lines[lineCount].Split(' ');
            int k = 0;

            // Read weights data
            rows = int.Parse(values[k++]);
            columns = int.Parse(values[k++]);
            weightsArray[lineCount] = new Matrix(rows, columns);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    // Debug.Log("trying to parse " + values[k]);
                    weightsArray[lineCount].set(i, j, float.Parse(values[k++]));
                }

            // Read biases data
            rows = int.Parse(values[k++]);
            columns = int.Parse(values[k++]);
            biasArray[lineCount] = new Matrix(rows, columns);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    // Debug.Log("trying to parse " + values[k]);
                    biasArray[lineCount].set(i, j, float.Parse(values[k++]));
                }
        }
    }

    /// <summary>
    /// Public function which implements the feed-forward algorithm.<br></br>
    /// Raises errors if something does not go right.
    /// </summary>
    /// <param name="inputs">the input matrix</param>
    /// <returns>argmax(output) OR true/false if it is only one output value</returns>
    public int Guess(Matrix inputs)
    {
        Matrix layerVar = inputs;

        for (int i = 0; i < weightsArray.Length; i++)
        {
            // Debug.Log("hidden: " + hidden.getRows() + ", " + hidden.getColumns());
            // Debug.Log("weightsArray[i]: " + weightsArray[i].getRows() + ", " + weightsArray[i].getColumns() + ", i = " + i);
            // Debug.Log("biasArray[i]: " + biasArray[i].getRows() + ", " + biasArray[i].getColumns() + ", i = " + i);

            // Calculate the weighted sum
            layerVar = (weightsArray[i] * layerVar) + biasArray[i];

            // Activate it
            SigmoidActivation(layerVar);
        }

        // For Flappy Bird only!!
        // return (hidden.at(0, 0) > 0.9f ? 1 : 0);
        return (layerVar.at(0, 0) > layerVar.at(1, 0) ? 1 : 0);
    }

    /// <summary>
    /// Applies sigmoid activation on a given matrix (in-place).
    /// </summary>
    /// <param name="m1">the matrix to apply to</param>
    private void SigmoidActivation(Matrix m1)
    {
        float value;
        for (int i = 0; i < m1.getRows(); i++)
            for (int j = 0; j < m1.getColumns(); j++)
            {
                value = m1.at(i, j);
                value = 1f / (1f + Mathf.Exp(-value));
                m1.set(i, j, value);
            }
    }

    /// <summary>
    /// Applies softmax activation on a given matrix (in-place)
    /// </summary>
    /// <param name="m1">the matrix to apply to</param>
    private void SoftmaxActivation(Matrix m1)
    {
        float sum = 0;
        float value;

        for (int i = 0; i < m1.getRows(); i++)
            for (int j = 0; j < m1.getColumns(); j++)
                sum += m1.at(i, j);
        for (int i = 0; i < m1.getRows(); i++)
            for (int j = 0; j < m1.getColumns(); j++)
            {
                value = m1.at(i, j) / sum;
                m1.set(i, j, value);
            }
    }

    /// <summary>
    /// Mutates with a specific rate the weights of the neural network.<br></br>
    /// Mutation means adding a predefined value to a given weight
    /// </summary>
    /// <param name="chance">mutation rate on a scale from 0f to 1f</param>
    public void Mutate(float chance)
    {
        for (int i = 0; i < weightsArray.Length; i++)
        {
            for (int j = 0; j < weightsArray[i].getRows(); j++)
                for (int k = 0; k < weightsArray[i].getColumns(); k++)
                    if (Random.Range(0f, 1f) < chance)
                    {
                        float oldValue = weightsArray[i].at(j, k);
                        oldValue += Random.Range(-0.1f, 0.1f);
                        weightsArray[i].set(j, k, oldValue);
                    }
        }
    }

    /// <summary>
    /// Public function which exports the neural network to a given file. <br></br>
    /// If the file does not exist, it will be created. Otherwise, it will be overwriten. <br></br>
    /// The format is as follows: <br></br>
    /// <br></br>
    /// [ROWS_WEIGHTS_LAYER_1] [COLUMNS_WEIGHTS_LAYER_1] [DATA_WEIGHTS_LAYER_1] [ROWS_BIAS_LAYER_1] [COLUMNS_BIAS_LAYER_1] [DATA_BIAS_LAYER_1] <br></br>
    /// [ROWS_WEIGHTS_LAYER_2] [COLUMNS_WEIGHTS_LAYER_2] [DATA_WEIGHTS_LAYER_2] [ROWS_BIAS_LAYER_2] [COLUMNS_BIAS_LAYER_2] [DATA_BIAS_LAYER_2] <br></br>
    /// ... <br></br>
    /// [ROWS_WEIGHTS_LAYER_n] [COLUMNS_WEIGHTS_LAYER_n] [DATA_WEIGHTS_LAYER_n] [ROWS_BIAS_LAYER_n] [COLUMNS_BIAS_LAYER_n] [DATA_BIAS_LAYER_n] <br></br>
    /// <br></br>
    /// where each DATA_WEIGHTS gets its size from the product of ROWS_WEIGHTS and COLUMNS_WEIGHTS (the same goes for BIAS). <br></br>
    /// </summary>
    /// <param name="path">path to the file (commonly ended in ".txt")</param>
    public void Export(string path)
    {
        using (System.IO.StreamWriter fout = new System.IO.StreamWriter(path))
        {
            for (int i = 0; i < weightsArray.Length; i++)
            {
                // Write weights
                fout.Write(weightsArray[i].getRows() + " ");
                fout.Write(weightsArray[i].getColumns() + " ");
                for (int j = 0; j < weightsArray[i].getRows(); j++)
                    for (int k = 0; k < weightsArray[i].getColumns(); k++)
                        fout.Write(weightsArray[i].at(j, k) + " ");

                // Write biases
                fout.Write(biasArray[i].getRows() + " ");
                fout.Write(biasArray[i].getColumns() + " ");
                for (int j = 0; j < biasArray[i].getRows(); j++)
                    for (int k = 0; k < biasArray[i].getColumns(); k++)
                            fout.Write(biasArray[i].at(j, k) + " ");

                if (i + 1 != weightsArray.Length)
                    fout.WriteLine("");
            }
        }
    }
    
    /// <summary>
    /// Default method for displaying (usually for debugging purposes) a NeuralNetwork object.
    /// </summary>
    /// <returns>a string representation of the current object</returns>
    public override string ToString()
    {
        string result = "";
        result += "Weights: \n";

        for (int i = 0; i < weightsArray.Length; i++)
            result += weightsArray[i].ToString() + "\n";

        result += "Biases: \n";
        for (int i = 0; i < biasArray.Length; i++)
            result += biasArray[i].ToString() + "\n";

        return result;
    }

    /// <summary>
    /// Public function for applying crossover to two given neural networks.
    /// The crossover is done in-place.
    /// 
    /// The idea is to switch each others set of weights from one layer to the next, meaning that, given
    /// two neural networks A1 and A2, with layers I_H_1, H_O_1, I_H_2, H_O_2, the resulting kids will be
    /// B1 with I_H_1, H_O_2 and B2 with I_H_2, H_O_1.
    /// </summary>
    /// <param name="n1">the first individual for crossover</param>
    /// <param name="n2">the second individual for crossover</param>
    static public void Crossover(NeuralNetwork n1, NeuralNetwork n2)
    {
        if (n1.weightsArray.Length != n2.weightsArray.Length)
        {
            Debug.LogError("We have a problem...");
            return;
        }
        NeuralNetwork[] kids = new NeuralNetwork[2];
        kids[0] = new NeuralNetwork(n1);
        kids[1] = new NeuralNetwork(n2);

        // Build the first kid
        for (int i = 0; i < n1.weightsArray.Length; i++)
        {
            if (i % 2 == 1)
            {
                kids[0].weightsArray[i] = new Matrix(n2.weightsArray[i]);
                kids[0].biasArray[i] = new Matrix(n2.biasArray[i]);
            }
        }

        // Build the second kid
        for (int i = 0; i < n1.weightsArray.Length; i++)
        {
            if (i % 2 == 1)
            {
                kids[1].weightsArray[i] = new Matrix(n1.weightsArray[i]);
                kids[1].biasArray[i] = new Matrix(n1.biasArray[i]);
            }
        }

        // Update the values
        n1 = kids[0];
        n2 = kids[1];
    }
}