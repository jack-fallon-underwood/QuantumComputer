public class ComplexMatrix
{
    public ComplexNumber[,] Data { get; private set; }
    public int Rows => Data.GetLength(0);
    public int Cols => Data.GetLength(1);

    public ComplexMatrix(int rows, int cols)
{
    Data = new ComplexNumber[rows, cols];
    for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++)
            Data[i, j] = new ComplexNumber(0, 0);
}

    public ComplexMatrix(ComplexNumber[,] initialData)
    {
        Data = initialData;
    }

    
    public ComplexMatrix Conjugate()
    {
        var result = new ComplexNumber[Rows, Cols];
        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Cols; j++)
                result[i, j] = Data[i, j].Conjugate();
        return new ComplexMatrix(result);
    }

    
    public ComplexMatrix Transpose()
    {
        var result = new ComplexNumber[Cols, Rows];
        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Cols; j++)
                result[j, i] = Data[i, j];
        return new ComplexMatrix(result);
    }

    
    public ComplexMatrix Dagger()
    {
        return this.Conjugate().Transpose();
    }

   
    public ComplexNumber Trace()
    {
        if (Rows != Cols)
            throw new InvalidOperationException("Trace is only defined for square matrices.");

        ComplexNumber sum = new ComplexNumber(0, 0);
        for (int i = 0; i < Rows; i++)
            sum += Data[i, i];

        return sum;
    }

    
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

   
    public static ComplexMatrix operator *(ComplexNumber scalar, ComplexMatrix m)
    {
        var result = new ComplexNumber[m.Rows, m.Cols];
        for (int i = 0; i < m.Rows; i++)
            for (int j = 0; j < m.Cols; j++)
                result[i, j] = scalar * m.Data[i, j];

        return new ComplexMatrix(result);
    }

    
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

        //  A^T == A
        public bool IsSymmetric()
        {
            if (Rows != Cols) return false;

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j <= i; j++) 
                {
                    if (!Data[i, j].Equals(Data[j, i]))
                        return false;
                }
            }
            return true;
        }

        // A^T * A == I
        public bool IsOrthogonal(double tolerance = 1e-10)
        {
            if (Rows != Cols) return false;

            ComplexMatrix product = this.Transpose() * this;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    double expected = (i == j) ? 1.0 : 0.0;
                    if (Math.Abs(product.Data[i, j].Real - expected) > tolerance ||
                        Math.Abs(product.Data[i, j].Imag) > tolerance)
                        return false;
                }
            }
            return true;
        }

        //A† == A
        public bool IsHermitian(double tolerance = 1e-10)
        {
            if (Rows != Cols) return false;

            ComplexMatrix dagger = this.Dagger();
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    if (Math.Abs(dagger.Data[i, j].Real - Data[i, j].Real) > tolerance ||
                        Math.Abs(dagger.Data[i, j].Imag - Data[i, j].Imag) > tolerance)
                        return false;
                }
            }
            return true;
        }

        // A† * A == I
        public bool IsUnitary(double tolerance = 1e-10)
        {
            if (Rows != Cols) return false;

            ComplexMatrix product = this.Dagger() * this;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    double expected = (i == j) ? 1.0 : 0.0;
                    if (Math.Abs(product.Data[i, j].Real - expected) > tolerance ||
                        Math.Abs(product.Data[i, j].Imag) > tolerance)
                        return false;
                }
            }
            return true;
        }

        public bool IsEqual(ComplexMatrix other, double tolerance = 1e-10)
    {
        if (this.Rows != other.Rows || this.Cols != other.Cols)
            return false;

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                if (Math.Abs(this.Data[i, j].Real - other.Data[i, j].Real) > tolerance ||
                    Math.Abs(this.Data[i, j].Imag - other.Data[i, j].Imag) > tolerance)
                    return false;
            }
        }

        return true;
    }

    public bool EigenCheck(ComplexNumber lambda, ComplexVector v, double tolerance = 1e-10)
    {
        if (this.Rows != this.Cols)
            throw new InvalidOperationException("Eigenvalues are only defined for square matrices.");

        if (this.Cols != v.Length)
            throw new InvalidOperationException("Matrix and vector dimensions must match.");

        // A * v
        ComplexMatrix Av = this * v;

        // Compute λ * v
        ComplexMatrix Lv = new ComplexMatrix(v.Rows, 1);
        for (int i = 0; i < v.Rows; i++)
            Lv.Data[i, 0] = lambda * v.Data[i, 0];

      
        for (int i = 0; i < v.Rows; i++)
        {
            ComplexNumber diff = Av.Data[i, 0] - Lv.Data[i, 0];
            if (Math.Abs(diff.Real) > tolerance || Math.Abs(diff.Imag) > tolerance)
                return false;
        }

        return true;
    }
    
//static version of EigenCheck   
    public static bool EigenCheck(ComplexMatrix A, ComplexNumber lambda, ComplexVector v, double tolerance = 1e-10)
    {
        if (A.Rows != A.Cols)
            throw new InvalidOperationException("Eigenvalues are only defined for square matrices.");

        if (A.Cols != v.Length)
            throw new InvalidOperationException("Matrix and vector dimensions must match.");

        //  A * v
        ComplexMatrix Av = A * v;

        // Compute λ * v
        ComplexMatrix Lv = new ComplexMatrix(v.Rows, 1);
        for (int i = 0; i < v.Rows; i++)
            Lv.Data[i, 0] = lambda * v.Data[i, 0];

       
        for (int i = 0; i < v.Rows; i++)
        {
            ComplexNumber diff = Av.Data[i, 0] - Lv.Data[i, 0];
            if (Math.Abs(diff.Real) > tolerance || Math.Abs(diff.Imag) > tolerance)
                return false;
        }

        return true;
    }




    public static ComplexMatrix TensorProduct(ComplexMatrix A, ComplexMatrix B)
    {
        int m = A.Rows;
        int n = A.Cols;
        int p = B.Rows;
        int q = B.Cols;

        var result = new ComplexNumber[m * p, n * q];

        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                ComplexNumber a = A.Data[i, j];

                for (int bi = 0; bi < p; bi++)
                {
                    for (int bj = 0; bj < q; bj++)
                    {
                        result[i * p + bi, j * q + bj] = a * B.Data[bi, bj];
                    }
                }
            }
        }

        return new ComplexMatrix(result);
    }



}
