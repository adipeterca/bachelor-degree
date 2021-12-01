using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Neural network class used in the NEAT algorithm
public class NeuralNetwork
{
    private Matrix[] weightsArray;
    private Matrix[] biasArray;


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
            float min = -(1.0f / Mathf.Sqrt(layers[i]));
            float max = (1.0f / Mathf.Sqrt(layers[i]));

            // Weights initialization
            weightsArray[i] = new Matrix(layers[i + 1], layers[i], min, max);

            // Bias initialization
            biasArray[i] = new Matrix(layers[i + 1], layers[i], 0f, 1f);
        }
    }

    // Public function which implements the feed-forward algorithm.
    // The return value is argmax(output) OR true/false if it is only one output value (it is compared with 0.5 - greater means true, lower/equal means false)
    public int guess(Matrix inputs)
    {
        Matrix hidden = inputs;

        for (int i = 0; i < weightsArray.Length; i++)
        {
            // Calculate the weighted sum
            hidden = (weightsArray[i] * inputs) + biasArray[i];

            // Activate it
            // sigmoidActivation(hidden);
        }

        // For Flappy Bird only!!
        return (hidden.at(0, 0) > 0.5f ? 1 : 0);
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
    
    // must implement cross over
    // must implement mutation
}
