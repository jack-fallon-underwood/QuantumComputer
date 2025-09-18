using System;
using System.Collections.Generic;
using System.Linq;

public static class QuantumStates
{
    // 1) Convert bitstring to basis ket (column vector with a single 1)
    public static ComplexMatrix KetFromBits(string bits, int dimension = 32)
    {
        int index = Convert.ToInt32(bits, 2); // parse binary string to int
        var data = new ComplexNumber[dimension, 1];
        for (int i = 0; i < dimension; i++)
            data[i, 0] = new ComplexNumber(0, 0);

        data[index, 0] = new ComplexNumber(1, 0);
        return new ComplexMatrix(data);
    }

    // 2) Uniform superposition ket of size 2^n
    public static ComplexMatrix UniformKet(int n)
    {
        int dim = 1 << n; // 2^n
        double value = 1.0 / Math.Sqrt(dim); // normalized amplitude
        var data = new ComplexNumber[dim, 1];

        for (int i = 0; i < dim; i++)
            data[i, 0] = new ComplexNumber(value, 0);

        return new ComplexMatrix(data);
    }

    // 3) Bloch sphere angles (theta, phi) from a qubit state [α, β]
    public static (double theta, double phi) BlochAngles(ComplexNumber alpha, ComplexNumber beta)
    {
        double norm = Math.Sqrt(alpha.Modulus() * alpha.Modulus() + beta.Modulus() * beta.Modulus());
        if (norm == 0) throw new ArgumentException("State cannot be zero.");

        alpha = new ComplexNumber(alpha.Real / norm, alpha.Imag / norm);
        beta = new ComplexNumber(beta.Real / norm, beta.Imag / norm);

        double theta = 2 * Math.Acos(alpha.Modulus());
        double phi = Math.Atan2(beta.Imag, beta.Real);

        return (theta, phi);
    }

    // 4) Probability of observing a basis state given a ket
    public static double ProbabilityOfOutcome(string bits, ComplexMatrix state)
    {
        int index = Convert.ToInt32(bits, 2);
        if (index < 0 || index >= state.Rows)
            throw new ArgumentException("Bitstring index out of bounds for state vector.");

        var amp = state.Data[index, 0];
        return amp.Real * amp.Real + amp.Imag * amp.Imag; // |amp|^2
    }
}
