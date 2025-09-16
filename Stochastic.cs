class StochasticMatrix
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
        if (state.Rows != A.Rows || state.Cols != 1)
            throw new ArgumentException("State vector must match matrix size.");
        if (n < 0)
            throw new ArgumentException("Number of states must be non-negative.");

        var states = new List<ComplexMatrix>();
        ComplexMatrix current = state;
        for (int i = 0; i < n; i++)
        {
            states.Add(current);
            current = A * current;
        }
        return states;
    }

    public static ComplexMatrix SequenceOnBasisVector(List<ComplexMatrix> matrices, int size)
    {
        var init = new ComplexMatrix(size, 1);
        init.Data[0, 0] = new ComplexNumber(1, 0);
        for (int i = 1; i < size; i++)
            init.Data[i, 0] = new ComplexNumber(0, 0);

        foreach (var A in matrices)
        {
            if (A.Rows != size || A.Cols != size)
                throw new ArgumentException("All matrices must be square and match the vector size.");
            if (!IsColumnStochastic(A))
                throw new ArgumentException("All matrices must be column-stochastic.");
        }

        return StateAfterSequence(matrices, init);
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


public static ComplexMatrix StateAfterNStepsColumnNormalized(ComplexMatrix A, ComplexMatrix state, int n)
{
    CheckRealColumnNormalized(A);
    if (state.Rows != A.Rows || state.Cols != 1)
        throw new ArgumentException("State vector must match matrix size.");
    if (n < 0) throw new ArgumentException("Number of steps must be non-negative.");

    ComplexMatrix result = state;
    for (int i = 0; i < n; i++)
        result = A * result;
    return result;
}

public static ComplexMatrix SequenceOnBasisVectorColumnNormalized(List<ComplexMatrix> matrices, int size)
{
    var state = new ComplexMatrix(size, 1);
    state.Data[0, 0] = new ComplexNumber(1, 0);

    for (int i = 1; i < size; i++)
        state.Data[i, 0] = new ComplexNumber(0, 0);

    foreach (var A in matrices)
    {
        CheckRealColumnNormalized(A);
        if (A.Rows != size) throw new ArgumentException("Matrix size mismatch.");
        state = A * state;
    }

    return state;
}



}
