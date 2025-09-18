public class Stochastic
{
    public static bool IsBooleanColumnStochastic(ComplexMatrix m)
    {
        for (int j = 0; j < m.Cols; j++)
        {
            int colSum = 0;
            for (int i = 0; i < m.Rows; i++)
            {
                var val = m.Data[i, j];
                if (!(val.Imag == 0 && (val.Real == 0 || val.Real == 1)))
                    return false; // must be 0 or 1
                colSum += (int)val.Real;
            }
            if (colSum != 1) return false; // one "1" per column
        }
        return true;
    }

    public static bool IsColumnStochastic(ComplexMatrix m)
    {
        for (int j = 0; j < m.Cols; j++)
        {
            double colSum = 0;
            for (int i = 0; i < m.Rows; i++)
            {
                var val = m.Data[i, j];
                if (val.Imag != 0 || val.Real < 0) return false;
                colSum += val.Real;
            }
            if (Math.Abs(colSum - 1.0) > 1e-9) return false;
        }
        return true;
    }

    public static bool IsColumnNormalized(ComplexMatrix m)
    {
        for (int j = 0; j < m.Cols; j++)
        {
            double colSum = 0;
            for (int i = 0; i < m.Rows; i++)
            {
                var val = m.Data[i, j];
                if (val.Imag != 0) return false;
                colSum += Math.Abs(val.Real);
            }
            if (Math.Abs(colSum - 1.0) > 1e-9) return false;
        }
        return true;
    }

    // Potentially move to ComplexMatrix.cs
    public static ComplexMatrix Power(ComplexMatrix A, int n)
    {
        if (A.Rows != A.Cols)
            throw new InvalidOperationException("Matrix must be square.");
        if (n < 1) throw new ArgumentException("Exponent must be positive.");

        ComplexMatrix result = Identity(A.Rows);
        ComplexMatrix baseMatrix = A;

        while (n > 0)
        {
            if ((n & 1) == 1) result = result * baseMatrix;
            baseMatrix = baseMatrix * baseMatrix;
            n >>= 1;
        }

        return result;
    }

    public static ComplexMatrix ApplySequence(
        List<ComplexMatrix> matrices, ComplexMatrix vector)
    {
        ComplexMatrix result = vector;
        foreach (var M in matrices)
            result = M * result;
        return result;
    }

    //Also potentially mpve
    public static ComplexMatrix Identity(int n)
    {
        var id = new ComplexNumber[n, n];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                id[i, j] = (i == j) ? new ComplexNumber(1, 0) : new ComplexNumber(0, 0);
        return new ComplexMatrix(id);
    }

    public static ComplexMatrix StateAfterNSteps(ComplexMatrix A, ComplexMatrix state, int n)
    {
        if (A.Rows != A.Cols) throw new ArgumentException("Matrix must be square.");
        if (!IsBooleanColumnStochastic(A)) throw new ArgumentException("Matrix must be a boolean column-stochastic matrix.");
        if (state.Rows != A.Rows || state.Cols != 1) throw new ArgumentException("State vector must match matrix size.");
        if (n < 0) throw new ArgumentException("Number of steps must be a positive whole number.");

        ComplexMatrix result = state;
        for (int i = 0; i < n; i++)
            result = A * result;

        return result;
    }

    public static ComplexMatrix StateAfterSequence(List<ComplexMatrix> matrices, ComplexMatrix state)
    {
        ComplexMatrix result = state;
        foreach (var A in matrices)
        {
            if (A.Rows != result.Rows) throw new ArgumentException("Matrix and vector size mismatch.");
            if (!IsBooleanColumnStochastic(A)) throw new ArgumentException("Matrix must be a boolean column-stochastic matrix.");
            result = A * result;
        }
        return result;
    }

    //  A^n for column-stochastic matrix
    public static ComplexMatrix PowerCSM(ComplexMatrix A, int n)
    {
        if (!IsColumnStochastic(A))
            throw new ArgumentException("Matrix must be column-stochastic.");

        return Power(A, n);
    }



    // Textbook Input might be wrong 
 public static List<ComplexMatrix> FirstNStates(ComplexMatrix A, ComplexMatrix state, int n)
{
    if (A.Rows != A.Cols)
        throw new ArgumentException("Matrix must be square.");
    if (!IsColumnStochastic(A))
        throw new ArgumentException("Matrix must be column-stochastic.");
    if (n < 0)
        throw new ArgumentException("Number of states must be non-negative.");

    // Generate default state vector if null
    if (state == null)
    {
        state = new ComplexMatrix(A.Rows, 1);
        state.Data[0, 0] = new ComplexNumber(1, 0);
        for (int i = 1; i < A.Rows; i++)
            state.Data[i, 0] = new ComplexNumber(0, 0);
    }
    else if (state.Rows != A.Rows || state.Cols != 1)
    {
        throw new ArgumentException("State vector must match matrix size.");
    }

    var states = new List<ComplexMatrix>();
    ComplexMatrix current = state;
    for (int i = 0; i < n; i++)
    {
        states.Add(current);
        current = A * current;
    }
    return states;
}


        public static ComplexMatrix SequenceOnBasisVector(List<ComplexMatrix> matrices)
{
    if (matrices == null || matrices.Count == 0)
        throw new ArgumentException("Matrix sequence cannot be null or empty.");

    int size = matrices[0].Rows;
    var state = new ComplexMatrix(size, 1);
    state.Data[0, 0] = new ComplexNumber(1, 0);
    for (int i = 1; i < size; i++)
        state.Data[i, 0] = new ComplexNumber(0, 0);

    foreach (var A in matrices)
    {
        if (A.Rows != size || A.Cols != size)
            throw new ArgumentException("All matrices must be square and match the vector size.");
        if (!IsColumnStochastic(A))
            throw new ArgumentException("All matrices must be column-stochastic.");
    }

    return StateAfterSequence(matrices, state);
}


    private static void CheckRealColumnNormalized(ComplexMatrix A)
    {
        if (A.Rows != A.Cols) throw new ArgumentException("Matrix must be square.");
        if (!IsColumnNormalized(A))
            throw new ArgumentException("Matrix must be column-normalized.");
    }

    public static ComplexMatrix PowerColumnNormalized(ComplexMatrix A, int n)
    {
        CheckRealColumnNormalized(A);
        return Power(A, n);
    }

//Textbook might be wrong 
public static ComplexMatrix StateAfterNStepsColumnNormalized(ComplexMatrix A, ComplexMatrix state, int n)
{
    CheckRealColumnNormalized(A);

    if (n < 0)
        throw new ArgumentException("Number of steps must be non-negative.");

    // Generate default state vector if null
    if (state == null)
    {
        state = new ComplexMatrix(A.Rows, 1);
        state.Data[0, 0] = new ComplexNumber(1, 0);
        for (int i = 1; i < A.Rows; i++)
            state.Data[i, 0] = new ComplexNumber(0, 0);
    }
    else if (state.Rows != A.Rows || state.Cols != 1)
    {
        throw new ArgumentException("State vector must match matrix size.");
    }

    ComplexMatrix result = state;
    for (int i = 0; i < n; i++)
        result = A * result;

    return result;
}


public static ComplexMatrix SequenceOnBasisVectorColumnNormalized(List<ComplexMatrix> matrices)
{
    if (matrices == null || matrices.Count == 0)
        throw new ArgumentException("Matrix list cannot be null or empty.");

    int size = matrices[0].Rows;

    var state = new ComplexMatrix(size, 1);
    state.Data[0, 0] = new ComplexNumber(1, 0); // [1,0,...,0]^T
    for (int i = 1; i < size; i++)
        state.Data[i, 0] = new ComplexNumber(0, 0);

    foreach (var A in matrices)
    {
        CheckRealColumnNormalized(A);
        if (A.Rows != size || A.Cols != size)
            throw new ArgumentException("All matrices must be square and match the initial vector size.");
        state = A * state;
    }

    return state;
}




    // public static ComplexMatrix NormalizeVector(ComplexMatrix v)
    // {
    //     double norm = 0;
    //     for (int i = 0; i < v.Rows; i++)
    //         norm += Math.Abs(v.Data[i, 0].Real * v.Data[i, 0].Real + v.Data[i, 0].Imag * v.Data[i, 0].Imag);

    //     norm = Math.Sqrt(norm);
    //     var result = new ComplexMatrix(v.Rows, 1);
    //     for (int i = 0; i < v.Rows; i++)
    //         result.Data[i, 0] = new ComplexNumber(v.Data[i, 0].Real / norm, v.Data[i, 0].Imag / norm);
    //     return result;
    // }

    // public static ComplexMatrix PhaseNormalizedVector(ComplexMatrix v, double r)
    // {
    //     var normalized = NormalizeVector(v);
    //     var result = new ComplexMatrix(v.Rows, 1);
    //     for (int i = 0; i < v.Rows; i++)
    //     {
    //         double magnitude = Math.Sqrt(normalized.Data[i, 0].Real * normalized.Data[i, 0].Real +
    //                                      normalized.Data[i, 0].Imag * normalized.Data[i, 0].Imag);
    //         double phase = Math.Atan2(normalized.Data[i, 0].Imag, normalized.Data[i, 0].Real);
    //         double newPhase = Math.Pow(phase, r);
    //         result.Data[i, 0] = new ComplexNumber(magnitude * Math.Cos(newPhase), magnitude * Math.Sin(newPhase));
    //     }
    //     return result;
    // }

    // public static ComplexMatrix KeyToBra(int key, int dimension)
    // {
    //     var bra = new ComplexMatrix(dimension, 1);
    //     for (int i = 0; i < dimension; i++)
    //         bra.Data[i, 0] = new ComplexNumber(i == key ? 1 : 0, 0);
    //     return bra;
    // }

    // public static int BraToKey(ComplexMatrix bra)
    // {
    //     for (int i = 0; i < bra.Rows; i++)
    //         if (bra.Data[i, 0].Real != 0) return i;
    //     throw new ArgumentException("Bra does not represent a valid key");
    // }

    // public static ComplexMatrix KetBraMatrix(ComplexMatrix ket, ComplexMatrix bra)
    // {
    //     var result = new ComplexMatrix(ket.Rows, bra.Rows);
    //     for (int i = 0; i < ket.Rows; i++)
    //         for (int j = 0; j < bra.Rows; j++)
    //             result.Data[i, j] = new ComplexNumber(
    //                 ket.Data[i, 0].Real * bra.Data[j, 0].Real - ket.Data[i, 0].Imag * bra.Data[j, 0].Imag,
    //                 ket.Data[i, 0].Real * bra.Data[j, 0].Imag + ket.Data[i, 0].Imag * bra.Data[j, 0].Real
    //             );
    //     return result;
    // }



}
