using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Algebra;

public class Matrix
{
    double[,] items;
    public Matrix(int rows, int columns)
    {
        this.items = new double[rows, columns];
    }

    public Matrix(double[,] items)
    {
        if (items == null)
            throw new ArgumentNullException("Cannot create a null Matrix");
        this.items = items;
    }

    public int Rows => items.GetLength(0);
    public int Columns => items.GetLength(1);
    public int Size => this.items.Length;

    
    public double this[int i, int j]
    {
        get
        {
            if (i >= Rows || i < 0)
                throw new ArgumentOutOfRangeException(" index i out of Range");
            if (j >= Columns || j < 0)
                throw new ArgumentOutOfRangeException(" index j out of Range");
            return items[i, j];
        }
        set
        {
            if (i >= Rows || i < 0)
                throw new ArgumentOutOfRangeException(" index i out of Range");
            if (j >= Columns || j < 0)
                throw new ArgumentOutOfRangeException(" index j out of Range");

            items[i, j] = value;
        }
    }

    public bool Equals(Matrix m)
    {
        if (this.Rows != m.Rows || this.Columns != m.Columns)
            return false;

        for (int i = 0; i < this.Rows; i++)
            for (int j = 0; j < this.Columns; j++)
                if (this[i, j] != m[i, j])
                    return false;
        return true;
    }

    public Matrix Add(Matrix other)
    {
        if (other == null)
            throw new ArgumentException("other");
        if (this.Rows != other.Rows || this.Columns != other.Columns)
            throw new ArgumentException ("Both Matrix dimensions must match");

        Matrix sum = new Matrix(this.Rows, this.Columns);

        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Columns; j++)
                sum[i, j] = this[i, j] + other[i, j];

        return sum;
    }

    public Matrix Subtract(Matrix other)
    {
        if (other == null)
            throw new ArgumentNullException("other");
        if (this.Rows != other.Rows || this.Columns != other.Columns)
            throw new InvalidOperationException("Invalid Dimensions");

        Matrix sub = new Matrix(this.Rows, this.Columns);

        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Columns; j++)
                sub[i, j] = this[i, j] - other[i, j];

        return sub;
    }

public Matrix DotProduct(Matrix other)
    {
        if (other == null)
            throw new ArgumentException("Null Operand");
        if (this.Columns != other.Rows)
            throw new ArgumentException("Rows of first Matrix and Columns of the second must match");

        double[,] product = new double[this.Rows, other.Columns];

        for (int i = 0; i < this.Rows; i++)
            for (int j = 0; j < other.Columns; j++)
                for (int k = 0; k < this.Columns; k++)
                    product[i, j] += this[i, k] * other[k, j];

        return new Matrix(product);
    }

    public Matrix ScalarProduct(double a)
    {
        var result = (double[,])this.items.Clone();

        for (var i = 0; i < this.Rows; i++)
            for (var j = 0; i < this.Columns; i++)
                result[i, j] *= a;

        return new Matrix(result);
    }

    public void Transpose()
    {
        var transposed = new double[Columns, Rows];

        for (int i = 0; i < this.Rows; i++)
            for (int j = 0; j < this.Columns; j++)
                transposed[j, i] = this.items[i, j];

        this.items = transposed;
    }

    #region Operators
    public static Matrix operator +(Matrix a, Matrix b)
    {
        return a.Add(b);
    }

    public static Matrix operator -(Matrix a, Matrix b)
    {
        return a.Subtract(b);
    }

    public static Matrix operator *(Matrix a, Matrix b)
    {
        return a.DotProduct(b);
    }
    public static Matrix operator *(double a, Matrix m)
    {
        return m.ScalarProduct(a);
    }

}
#endregion