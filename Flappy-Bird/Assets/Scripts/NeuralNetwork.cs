﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Neural network class used in the NeuroEvolution algorithm
public class NeuralNetwork
{
    private Matrix[] weightsArray;
    private Matrix[] biasArray;

    // For debugging purposes
    public System.Guid UUID = System.Guid.NewGuid();

    // Create a neural network with the configuration provided.
    // Each element of the array represents how many nodes that layer must have.
    // The weights are initilized using a Xavier approach.
    // The biases are between 0 and 1.
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

    // Construct a new neural network from a given one
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

    // Public function which implements the feed-forward algorithm.
    // 
    // The return value is argmax(output) OR true/false if it is only one output value
    // (it is compared with 0.5 - greater means true, lower/equal means false) (NOT ACCURATE ANYMORE)
    public int guess(Matrix inputs)
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
            sigmoidActivation(layerVar);
        }

        // For Flappy Bird only!!
        // return (hidden.at(0, 0) > 0.9f ? 1 : 0);
        return (layerVar.at(0, 0) > layerVar.at(1, 0) ? 1 : 0);
    }

    // Applies sigmoid activation on a given matrix (in-place)
    private void sigmoidActivation(Matrix m1)
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

    // Applies softmax activation on a given matrix (in-place)
    private void softmaxActivation(Matrix m1)
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
    
    // Mutates with a specific rate the weights of the neural network
    // Mutation means adding a predefined value to a given weight
    public void mutate(float chance)
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
    public void export(string path)
    {
        using (System.IO.StreamWriter fout = new System.IO.StreamWriter(path))
        {
            for (int i = 0; i < weightsArray.Length; i++)
            {
                fout.Write(weightsArray[i].getRows() + " ");
                fout.Write(weightsArray[i].getColumns() + " ");
                for (int j = 0; j < weightsArray[i].getRows(); j++)
                    for (int k = 0; k < weightsArray[i].getColumns(); k++)
                        fout.Write(weightsArray[i].at(j, k) + " ");

                fout.WriteLine("");
            }
        }
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
    static public void crossover(NeuralNetwork n1, NeuralNetwork n2)
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