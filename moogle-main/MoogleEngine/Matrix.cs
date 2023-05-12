namespace MoogleEngine;

//Clase que crea matrices personalizadas y hace operaciones con ellas. No fue necesario implementarla en el proyecto (F por las matrices)
public class Matrix
{
    private readonly int rows;
    private readonly int cols;
    private readonly double[,] data;

    //Constructor. Crea solo por filas y columnas
    public Matrix(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;
        this.data = new double[rows, cols];
    }

    //Otro constructor. Toma cantidad de filas, cantidad de columnas, y valores en cada posicion de la matriz
    public Matrix(double[,] data)
    {
        this.rows = data.GetLength(0);
        this.cols = data.GetLength(1);
        this.data = data;
    }

    //Metodo que suma dos matrices
    public Matrix Add(Matrix other)
    {
        if (this.rows != other.rows || this.cols != other.cols)
        {
            throw new ArgumentException("Matrices must have the same dimensions.");
        }

        var result = new Matrix(this.rows, this.cols);
        for (int i = 0; i < this.rows; i++)
        {
            for (int j = 0; j < this.cols; j++)
            {
                result.data[i, j] = this.data[i, j] + other.data[i, j];
            }
        }
        return result;
    }

    //Metodo que multiplica dos matrices
    public Matrix Multiply(Matrix other)
    {
        if (this.cols != other.rows)
        {
            throw new ArgumentException("The number of columns in the first matrix must match the number of rows in the second matrix.");
        }

        var result = new Matrix(this.rows, other.cols);
        for (int i = 0; i < this.rows; i++)
        {
            for (int j = 0; j < other.cols; j++)
            {
                double sum = 0;
                for (int k = 0; k < this.cols; k++)
                {
                    sum += this.data[i, k] * other.data[k, j];
                }
                result.data[i, j] = sum;
            }
        }
        return result;
    }

    //Metodo que calcula el determinante de una matriz
    public double Determinant()
    {
        if (this.rows != this.cols)
        {
            throw new ArgumentException("Matrix must be square.");
        }

        if (this.rows == 1)
        {
            return this.data[0, 0];
        }

        if (this.rows == 2)
        {
            return this.data[0, 0] * this.data[1, 1] - this.data[0, 1] * this.data[1, 0];
        }

        double det = 0;
        for (int j = 0; j < this.cols; j++)
        {
            var submatrix = new Matrix(this.rows - 1, this.cols - 1);
            for (int i = 1; i < this.rows; i++)
            {
                for (int k = 0; k < this.cols; k++)
                {
                    if (k < j)
                    {
                        submatrix.data[i - 1, k] = this.data[i, k];
                    }
                    else if (k > j)
                    {
                        submatrix.data[i - 1, k - 1] = this.data[i, k];
                    }
                }
            }
            det += Math.Pow(-1, j) * this.data[0, j] * submatrix.Determinant();
        }
        return det;
    }
}
