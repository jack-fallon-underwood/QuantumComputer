using System;

public static class GeneralGates
{
    private static readonly ComplexNumber I1 = new ComplexNumber(0, 1);

    // === Basic 2x2 Pauli matrices === REMOVE THESE
    public static readonly ComplexMatrix X = new ComplexMatrix(new ComplexNumber[,] {
        { new ComplexNumber(0,0), new ComplexNumber(1,0) },
        { new ComplexNumber(1,0), new ComplexNumber(0,0) }
    });

    public static readonly ComplexMatrix Y = new ComplexMatrix(new ComplexNumber[,] {
        { new ComplexNumber(0,0), -I1 },
        { I1, new ComplexNumber(0,0) }
    });

    public static readonly ComplexMatrix Z = new ComplexMatrix(new ComplexNumber[,] {
        { new ComplexNumber(1,0), new ComplexNumber(0,0) },
        { new ComplexNumber(0,0), new ComplexNumber(-1,0) }
    });

    public static readonly ComplexMatrix H = new ComplexMatrix(new ComplexNumber[,] {
        { new ComplexNumber(1/Math.Sqrt(2),0), new ComplexNumber(1/Math.Sqrt(2),0) },
        { new ComplexNumber(1/Math.Sqrt(2),0), new ComplexNumber(-1/Math.Sqrt(2),0) }
    });

    // === Phase & Rotation gates ===
    public static ComplexMatrix Phase(double theta) =>
        new ComplexMatrix(new ComplexNumber[,] {
            { new ComplexNumber(1,0), new ComplexNumber(0,0) },
            { new ComplexNumber(0,0), new ComplexNumber(Math.Cos(theta), Math.Sin(theta)) }
        });

    public static ComplexMatrix Rx(double theta) =>
        new ComplexMatrix(new ComplexNumber[,] {
            { new ComplexNumber(Math.Cos(theta/2),0), -I1 * new ComplexNumber(Math.Sin(theta/2),0) },
            { -I1 * new ComplexNumber(Math.Sin(theta/2),0), new ComplexNumber(Math.Cos(theta/2),0) }
        });

    public static ComplexMatrix Ry(double theta) =>
        new ComplexMatrix(new ComplexNumber[,] {
            { new ComplexNumber(Math.Cos(theta/2),0), new ComplexNumber(-Math.Sin(theta/2),0) },
            { new ComplexNumber(Math.Sin(theta/2),0), new ComplexNumber(Math.Cos(theta/2),0) }
        });

    public static ComplexMatrix Rz(double theta) =>
        new ComplexMatrix(new ComplexNumber[,] {
            { new ComplexNumber(Math.Cos(-theta/2), Math.Sin(-theta/2)), new ComplexNumber(0,0) },
            { new ComplexNumber(0,0), new ComplexNumber(Math.Cos(theta/2), Math.Sin(theta/2)) }
        });

    // === Single-qubit gate embedding ===
    public static ComplexMatrix EmbedSingleQubitGate(int n, int target, ComplexMatrix gate)
    {
        ComplexMatrix op = null;
        for (int i = 0; i < n; i++)
            op = (i == target) 
                ? (op == null ? gate : ComplexMatrix.TensorProduct(op, gate)) 
                : (op == null ? Identity(2) : ComplexMatrix.TensorProduct(op, Identity(2)));
        return op;
    }

    // === Controlled gate ===
    public static ComplexMatrix ControlledGate(int n, int control, int target, ComplexMatrix U)
    {
        var P0 = Projector(n, control, 0);
        var P1 = Projector(n, control, 1);
        var Uembed = EmbedSingleQubitGate(n, target, U);
        return P0 * Identity(1 << n) + P1 * Uembed;
    }

    // === Helpers ===
    public static ComplexMatrix Identity(int n)
    {
        var mat = new ComplexMatrix(n, n);
        for (int i = 0; i < n; i++)
            mat.Data[i, i] = new ComplexNumber(1,0);
        return mat;
    }

    public static ComplexMatrix Projector(int n, int qubit, int value)
    {
        var result = Identity(1);
        for (int i = 0; i < n; i++)
        {
            if (i == qubit)
            {
                var proj = (value == 0)
                    ? new ComplexMatrix(new ComplexNumber[,] { { new ComplexNumber(1,0), new ComplexNumber(0,0) }, { new ComplexNumber(0,0), new ComplexNumber(0,0) } })
                    : new ComplexMatrix(new ComplexNumber[,] { { new ComplexNumber(0,0), new ComplexNumber(0,0) }, { new ComplexNumber(0,0), new ComplexNumber(1,0) } });
                result = ComplexMatrix.TensorProduct(result, proj);
            }
            else
                result = ComplexMatrix.TensorProduct(result, Identity(2));
        }
        return result;
    }

    public static int[] ToBinary(int x, int n)
    {
        int[] bits = new int[n];
        for (int i = 0; i < n; i++)
            bits[n - 1 - i] = (x >> i) & 1;
        return bits;
    }

    public static int ToInt(int[] bits)
    {
        int val = 0;
        for (int i = 0; i < bits.Length; i++)
            val = (val << 1) | bits[i];
        return val;
    }

    // === SWAP Gate ===
    public static ComplexMatrix SwapGate(int n, int q1, int q2)
    {
        int dim = 1 << n;
        var mat = new ComplexMatrix(dim, dim);
        for (int basis = 0; basis < dim; basis++)
        {
            int[] bits = ToBinary(basis, n);
            int temp = bits[q1]; bits[q1] = bits[q2]; bits[q2] = temp;
            mat.Data[ToInt(bits), basis] = new ComplexNumber(1,0);
        }
        return mat;
    }

    // === Toffoli Gate ===
    public static ComplexMatrix ToffoliGate(int n, int c1, int c2, int target)
    {
        int dim = 1 << n;
        var mat = new ComplexMatrix(dim, dim);
        for (int basis = 0; basis < dim; basis++)
        {
            int[] bits = ToBinary(basis, n);
            int[] newBits = (int[])bits.Clone();
            if (bits[c1]==1 && bits[c2]==1) newBits[target] ^= 1;
            mat.Data[ToInt(newBits), basis] = new ComplexNumber(1,0);
        }
        return mat;
    }

    // === Fredkin Gate ===
    public static ComplexMatrix FredkinGate(int n, int control, int q1, int q2)
    {
        int dim = 1 << n;
        var mat = new ComplexMatrix(dim, dim);
        for (int basis = 0; basis < dim; basis++)
        {
            int[] bits = ToBinary(basis, n);
            if (bits[control] == 1)
            {
                int temp = bits[q1]; bits[q1] = bits[q2]; bits[q2] = temp;
            }
            mat.Data[ToInt(bits), basis] = new ComplexNumber(1,0);
        }
        return mat;
    }

    // === Measurement ===
    public static (ComplexMatrix M0, ComplexMatrix M1) Measurement(int n, int target)
    {
        int dim = 1 << n;
        var M0 = new ComplexMatrix(dim, dim);
        var M1 = new ComplexMatrix(dim, dim);
        for (int basis = 0; basis < dim; basis++)
        {
            int[] bits = ToBinary(basis, n);
            if (bits[target]==0) M0.Data[basis,basis] = new ComplexNumber(1,0);
            else M1.Data[basis,basis] = new ComplexNumber(1,0);
        }
        return (M0, M1);
    }
}

