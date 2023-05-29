using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Algebra;

public class Vector
{
    double[] items;
    public Vector(double[] items)
    {
        this.items = items;
    }

    public Vector(int length) => this.items = new double[length];

    public double this[int i]
    {
        get { return items[i]; }

        set { items[i] = value; }
    }

    public int Dimension => this.items.Length;

    public double Magnitude
    {
        get
        {
            double sum = 0;
            foreach (var item in items)
                sum += item * item;

            return Math.Sqrt(sum);
        }
    }

    public Vector UnitVector 
    {
        get{
            var unitVector = new Vector(Dimension);
            double magnitude = Magnitude;
            
            for (var i = 0; i < Dimension; i++)
                unitVector[i] = this.items[i]/magnitude;
            return unitVector;
        }
    }
    public static Vector operator +(Vector v, Vector u)
    {
        if (v.Dimension != u.Dimension)
            throw new InvalidOperationException("Vectors dimensions must match");

        double[] items = new double[v.Dimension];
        for (int i = 0; i < v.Dimension; i++)
            items[i] = u[i] + v[i];

        return new Vector(items);
    }

    public static Vector operator -(Vector v, Vector u)
    {
        if (v.Dimension != u.Dimension)
            throw new InvalidOperationException("Vectors dimensions must match");

        double[] items = new double[v.Dimension];
        for (int i = 0; i < v.Dimension; i++)
            items[i] = v[i] - u[i];

        return new Vector(items);
    }

    // Dot Product
    public static double operator *(Vector v, Vector u)
    {
        if (v.Dimension != u.Dimension)
            throw new InvalidOperationException("Vectors dimensions must match");

        double product = 0;
        for (int i = 0; i < v.Dimension; i++)
            product += v[i] * u[i];

        return product;
    }

    // Product by a scalar
    public static Vector operator *(double scalar, Vector v)
    {
        double[] product = new double[v.Dimension];

        for (int i = 0; i < v.Dimension; i++)
            product[i] = v[i] * scalar;

        return new Vector(product);
    }

    public static double CosineSimilarity(Vector v1, Vector v2)
    {
        if (v1.Dimension != v2.Dimension)
            throw new ArgumentException("Vector's dimensions must match");

        double sim = (v1 * v2) / (v1.Magnitude * v2.Magnitude);

        return sim;
    }

}
