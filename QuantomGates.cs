using System;

public static class QuantumGates
{
    // Identity
    public static ComplexMatrix I()
    {
        return new ComplexMatrix(new ComplexNumber[,]
        {
            { new ComplexNumber(1,0), new ComplexNumber(0,0) },
            { new ComplexNumber(0,0), new ComplexNumber(1,0) }
        });
    }

    // Pauli-X
    public static ComplexMatrix X()
    {
        return new ComplexMatrix(new ComplexNumber[,]
        {
            { new ComplexNumber(0,0), new ComplexNumber(1,0) },
            { new ComplexNumber(1,0), new ComplexNumber(0,0) }
        });
    }

    // Pauli-Y
    public static ComplexMatrix Y()
    {
        return new ComplexMatrix(new ComplexNumber[,]
        {
            { new ComplexNumber(0,0), new ComplexNumber(0,-1) },
            { new ComplexNumber(0,1), new ComplexNumber(0,0) }
        });
    }

    // Pauli-Z
    public static ComplexMatrix Z()
    {
        return new ComplexMatrix(new ComplexNumber[,]
        {
            { new ComplexNumber(1,0), new ComplexNumber(0,0) },
            { new ComplexNumber(0,0), new ComplexNumber(-1,0) }
        });
    }

    // Hadamard
    public static ComplexMatrix H()
    {
        double invSqrt2 = 1.0 / Math.Sqrt(2);
        return new ComplexMatrix(new ComplexNumber[,]
        {
            { new ComplexNumber(invSqrt2,0), new ComplexNumber(invSqrt2,0) },
            { new ComplexNumber(invSqrt2,0), new ComplexNumber(-invSqrt2,0) }
        });
    }

    // Phase (S gate)
    public static ComplexMatrix S()
    {
        return new ComplexMatrix(new ComplexNumber[,]
        {
            { new ComplexNumber(1,0), new ComplexNumber(0,0) },
            { new ComplexNumber(0,0), new ComplexNumber(0,1) }
        });
    }

    // T gate
    public static ComplexMatrix T()
    {
        double invSqrt2 = 1.0 / Math.Sqrt(2);
        return new ComplexMatrix(new ComplexNumber[,]
        {
            { new ComplexNumber(1,0), new ComplexNumber(0,0) },
            { new ComplexNumber(0,0), new ComplexNumber(invSqrt2,invSqrt2) }
        });
    }

    // Rotation around X
    public static ComplexMatrix Rx(double theta)
    {
        return new ComplexMatrix(new ComplexNumber[,]
        {
            { new ComplexNumber(Math.Cos(theta/2),0), new ComplexNumber(0,-Math.Sin(theta/2)) },
            { new ComplexNumber(0,-Math.Sin(theta/2)), new ComplexNumber(Math.Cos(theta/2),0) }
        });
    }

    // Rotation around Y
    public static ComplexMatrix Ry(double theta)
    {
        return new ComplexMatrix(new ComplexNumber[,]
        {
            { new ComplexNumber(Math.Cos(theta/2),0), new ComplexNumber(-Math.Sin(theta/2),0) },
            { new ComplexNumber(Math.Sin(theta/2),0), new ComplexNumber(Math.Cos(theta/2),0) }
        });
    }

    // Rotation around Z
    public static ComplexMatrix Rz(double theta)
    {
        return new ComplexMatrix(new ComplexNumber[,]
        {
            { new ComplexNumber(Math.Cos(theta/2), -Math.Sin(theta/2)), new ComplexNumber(0,0) },
            { new ComplexNumber(0,0), new ComplexNumber(Math.Cos(theta/2), Math.Sin(theta/2)) }
        });
    }

    // Phase-shift with angle θ
    public static ComplexMatrix P(double theta)
    {
        return new ComplexMatrix(new ComplexNumber[,]
        {
            { new ComplexNumber(1,0), new ComplexNumber(0,0) },
            { new ComplexNumber(0,0), new ComplexNumber(Math.Cos(theta), Math.Sin(theta)) }
        });
    }

    // Controlled-U: control=|1>, applies U to target
    public static ComplexMatrix CU(ComplexMatrix U)
    {
        if (U.Rows != 2 || U.Cols != 2)
            throw new ArgumentException("Controlled-U requires a 2x2 unitary.");

        return new ComplexMatrix(new ComplexNumber[,]
        {
            { new ComplexNumber(1,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0) },
            { new ComplexNumber(0,0), new ComplexNumber(1,0), new ComplexNumber(0,0), new ComplexNumber(0,0) },
            { new ComplexNumber(0,0), new ComplexNumber(0,0), U.Data[0,0], U.Data[0,1] },
            { new ComplexNumber(0,0), new ComplexNumber(0,0), U.Data[1,0], U.Data[1,1] }
        });
    }

    // CNOT = Controlled-X
    public static ComplexMatrix CNOT()
    {
        return CU(X());
    }

    // SWAP gate: swaps two qubits
public static ComplexMatrix SWAP()
{
    return new ComplexMatrix(new ComplexNumber[,]
    {
        { new ComplexNumber(1,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0) },
        { new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(1,0), new ComplexNumber(0,0) },
        { new ComplexNumber(0,0), new ComplexNumber(1,0), new ComplexNumber(0,0), new ComplexNumber(0,0) },
        { new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(1,0) }
    });
}

// Toffoli gate (CCNOT): two controls, one target
public static ComplexMatrix Toffoli()
{
    return new ComplexMatrix(new ComplexNumber[,]
    {
        { new ComplexNumber(1,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0) },
        { new ComplexNumber(0,0), new ComplexNumber(1,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0) },
        { new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(1,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0) },
        { new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(1,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0) },
        { new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(1,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0) },
        { new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(1,0), new ComplexNumber(0,0), new ComplexNumber(0,0) },
        { new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(1,0) },
        { new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(1,0), new ComplexNumber(0,0) }
    });
}

// Fredkin gate (CSWAP): controlled swap
public static ComplexMatrix Fredkin()
{
    return new ComplexMatrix(new ComplexNumber[,]
    {
        { new ComplexNumber(1,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0) },
        { new ComplexNumber(0,0), new ComplexNumber(1,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0) },
        { new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(1,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0) },
        { new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(1,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0) },
        { new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(1,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0) },
        { new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(1,0), new ComplexNumber(0,0) },
        { new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(1,0), new ComplexNumber(0,0), new ComplexNumber(0,0) },
        { new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(0,0), new ComplexNumber(1,0) }
    });
}

// Measurement gate (in computational basis)
public static ComplexMatrix M()
{
    return new ComplexMatrix(new ComplexNumber[,]
    {
        { new ComplexNumber(1,0), new ComplexNumber(0,0) },
        { new ComplexNumber(0,0), new ComplexNumber(0,0) }
    });
}

}
