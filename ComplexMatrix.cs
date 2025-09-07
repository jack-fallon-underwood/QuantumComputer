class ComplexMatrix
{
    public ComplexNumber[,] Data { get; private set; }
    public int Rows => Data.GetLength(0);
    public int Cols => Data.GetLength(1);

    public ComplexMatrix(int rows, int cols)
    {
        Data = new ComplexNumber[rows, cols];
    }

    public ComplexMatrix(ComplexNumber[,] initialData)
    {
        Data = initialData;
    }

    // 1) Conjugate
    public ComplexMatrix Conjugate()
    {
        var result = new ComplexNumber[Rows, Cols];
        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Cols; j++)
                result[i, j] = Data[i, j].Conjugate();
        return new ComplexMatrix(result);
    }

    // 2) Transpose
    public ComplexMatrix Transpose()
    {
        var result = new ComplexNumber[Cols, Rows];
        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Cols; j++)
                result[j, i] = Data[i, j];
        return new ComplexMatrix(result);
    }

    // 3) Dagger (adjoint)
    public ComplexMatrix Dagger()
    {
        return this.Conjugate().Transpose();
    }

    // 4) Trace
    public ComplexNumber Trace()
    {
        if (Rows != Cols)
            throw new InvalidOperationException("Trace is only defined for square matrices.");

        ComplexNumber sum = new ComplexNumber(0, 0);
        for (int i = 0; i < Rows; i++)
            sum += Data[i, i];

        return sum;
    }

    // 5) Matrix addition
    public static ComplexMatrix operator +(ComplexMatrix a, ComplexMatrix b)
    {
        if (a.Rows != b.Rows || a.Cols != b.Cols)
            throw new InvalidOperationException("Matrices must be the same size to add.");

        var result = new ComplexNumber[a.Rows, a.Cols];
        for (int i = 0; i < a.Rows; i++)
            for (int j = 0; j < a.Cols; j++)
                result[i, j] = a.Data[i, j] + b.Data[i, j];

        return new ComplexMatrix(result);
    }

    // 6) Scalar multiplication
    public static ComplexMatrix operator *(ComplexNumber scalar, ComplexMatrix m)
    {
        var result = new ComplexNumber[m.Rows, m.Cols];
        for (int i = 0; i < m.Rows; i++)
            for (int j = 0; j < m.Cols; j++)
                result[i, j] = scalar * m.Data[i, j];

        return new ComplexMatrix(result);
    }

    // 7) Matrix multiplication
    public static ComplexMatrix operator *(ComplexMatrix a, ComplexMatrix b)
    {
        if (a.Cols != b.Rows)
            throw new InvalidOperationException("Matrix A columns must equal Matrix B rows.");

        var result = new ComplexNumber[a.Rows, b.Cols];
        for (int i = 0; i < a.Rows; i++)
        {
            for (int j = 0; j < b.Cols; j++)
            {
                ComplexNumber sum = new ComplexNumber(0, 0);
                for (int k = 0; k < a.Cols; k++)
                {
                    sum += a.Data[i, k] * b.Data[k, j];
                }
                result[i, j] = sum;
            }
        }
        return new ComplexMatrix(result);
    }

    public override string ToString()
    {
        string s = "";
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
                s += Data[i, j] + "\t";
            s += "\n";
        }
        return s;
    }

//     // Check if the matrix is symmetric: A^T == A
//     public bool IsSymmetric()
//     {
//         if (Rows != Cols) return false;

//         for (int i = 0; i < Rows; i++)
//         {
//             for (int j = 0; j <= i; j++) // check only lower triangle
//             {
//                 if (!Data[i, j].Equals(Data[j, i]))
//                     return false;
//             }
//         }
//         return true;
//     }

//     // Check if the matrix is orthogonal: A^T * A == I (real matrices)
//     public bool IsOrthogonal(double tolerance = 1e-10)
//     {
//         if (Rows != Cols) return false;

//         ComplexMatrix product = this.Transpose() * this;
//         for (int i = 0; i < Rows; i++)
//         {
//             for (int j = 0; j < Cols; j++)
//             {
//                 double expected = (i == j) ? 1.0 : 0.0;
//                 if (Math.Abs(product.Data[i, j].Real - expected) > tolerance ||
//                     Math.Abs(product.Data[i, j].Imag) > tolerance)
//                     return false;
//             }
//         }
//         return true;
//     }

//     // Check if the matrix is Hermitian: A† == A
//     public bool IsHermitian(double tolerance = 1e-10)
//     {
//         if (Rows != Cols) return false;

//         ComplexMatrix dagger = this.Dagger();
//         for (int i = 0; i < Rows; i++)
//         {
//             for (int j = 0; j < Cols; j++)
//             {
//                 if (Math.Abs(dagger.Data[i, j].Real - Data[i, j].Real) > tolerance ||
//                     Math.Abs(dagger.Data[i, j].Imag - Data[i, j].Imag) > tolerance)
//                     return false;
//             }
//         }
//         return true;
//     }

//     // Check if the matrix is Unitary: A† * A == I
//     public bool IsUnitary(double tolerance = 1e-10)
//     {
//         if (Rows != Cols) return false;

//         ComplexMatrix product = this.Dagger() * this;
//         for (int i = 0; i < Rows; i++)
//         {
//             for (int j = 0; j < Cols; j++)
//             {
//                 double expected = (i == j) ? 1.0 : 0.0;
//                 if (Math.Abs(product.Data[i, j].Real - expected) > tolerance ||
//                     Math.Abs(product.Data[i, j].Imag) > tolerance)
//                     return false;
//             }
//         }
//         return true;
//     }
    
//     public bool IsEqual(ComplexMatrix other, double tolerance = 1e-10)
// {
//     if (this.Rows != other.Rows || this.Cols != other.Cols)
//         return false;

//     for (int i = 0; i < Rows; i++)
//     {
//         for (int j = 0; j < Cols; j++)
//         {
//             if (Math.Abs(this.Data[i, j].Real - other.Data[i, j].Real) > tolerance ||
//                 Math.Abs(this.Data[i, j].Imag - other.Data[i, j].Imag) > tolerance)
//                 return false;
//         }
//     }

//     return true;
// }

}
