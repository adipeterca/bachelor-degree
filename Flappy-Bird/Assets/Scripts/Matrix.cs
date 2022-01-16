using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Utility class used in matrix operations
public class Matrix
{
    private int rows;
    private int columns;

    private float[,] data;

    // Default constructor for the Matrix class
    public Matrix(int rows, int columns)
    {
        data = new float[rows, columns];
        this.rows = rows;
        this.columns = columns;
    }

    // Matrix construtor with random values (both inclusive)
    public Matrix(int rows, int columns, float min, float max)
    {
        data = new float[rows, columns];
        this.rows = rows;
        this.columns = columns;

        for (int i = 0; i < rows; i++)
            for (int j = 0; j < columns; j++)
                data[i, j] = Random.Range(min, max);
    }

    // Construct a Matrix object from an existing Matrix object.
    public Matrix(Matrix m)
    {
        data = new float[m.getRows(), m.getColumns()];
        rows = m.getRows();
        columns = m.getColumns();

        for (int i = 0; i < rows; i++)
            for (int j = 0; j < columns; j++)
                data[i, j] = m.data[i, j];
    }

    // Default rows getter
    public int getRows()
    {
        return rows;
    }

    // Default columns getter
    public int getColumns()
    {
        return columns;
    }

    // Public function which returns the transposed matrix
    public Matrix T()
    {
        Matrix result = new Matrix(columns, rows);
        for (int i = 0; i < columns; i++)
            for (int j = 0; j < rows; j++)
                result.data[i, j] = data[j, i];
        return result;
    }
    
    // Default data getter
    public float at(int row, int column)
    {
        if (row >= rows || column >= columns)
        {
            Debug.LogError("Invalid access to memory with function at()!");
            return 0f;
        }
        return data[row, column];
    }

    // Default data setter
    public void set(int row, int column, float value)
    {
        if (row >= rows || column >= columns)
        {
            Debug.LogError("Invalid access to memory with function set()!");
            return;
        }
        data[row, column] = value;
    }

    override public string ToString()
    {
        string info = "[";

        for (int i = 0; i < rows; i++)
        {
            info += "[";
            for (int j = 0; j < columns; j++)
                info += (data[i, j] + ", ");
            info += "], ";
        }
        info += "]";
        return info;
    }

    // Matrix addition (element-wise)
    public static Matrix operator+(Matrix m1, Matrix m2)
    {
        if (m1.getColumns() != m2.getColumns())
        {
            Debug.LogError("Matrix addition with invalid sizes (columns): M1 = " + m1.getColumns() + ", M2 = " + m2.getColumns());
            return null;
        }
        if (m1.getRows() != m2.getRows())
        {
            Debug.LogError("Matrix addition with invalid sizes (rows): M1 = " + m1.getRows() + ", M2 = " + m2.getRows());
            return null;
        }

        Matrix result = new Matrix(m1.getRows(), m1.getColumns());
        for (int i = 0; i < result.getRows(); i++)
            for (int j = 0; j < result.getColumns(); j++)
                result.data[i, j] = m1.data[i, j] + m2.data[i, j];

        return result;
    }

    public static Matrix operator*(Matrix m1, Matrix m2)
    {
        if (m1.getColumns() != m2.getRows())
        {
            Debug.LogError("Matrix multiplication with invalid sizes! M1 columns: " + m1.getColumns() + "-- M2 rows: " + m2.getRows());
            return null;
        }

        Matrix result = new Matrix(m1.getRows(), m2.getColumns());
        for (int i = 0; i < result.getRows(); i++)
            for (int j = 0; j < result.getColumns(); j++)
            {
                result.data[i, j] = 0f;
                for (int k = 0; k < m1.getColumns(); k++)
                    result.data[i, j] += m1.data[i, k] * m2.data[k, j];
            }
        return result;
    }
    
}
