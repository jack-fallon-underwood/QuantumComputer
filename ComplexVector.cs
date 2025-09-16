public class ComplexVector : ComplexMatrix
{
    public int Length => Rows; // since this is always n×1

    public ComplexVector(int size) : base(size, 1) { }

    public ComplexVector(ComplexNumber[] values) : base(values.Length, 1)
    {
        for (int i = 0; i < values.Length; i++)
            Data[i, 0] = values[i];
    }

    public double Norm()
{
    ComplexNumber inner = this.InnerProduct(this);
    return Math.Sqrt(inner.Real);
}

    // Normalized vector (unit norm)
    public ComplexVector Normalize()
    {
        double norm = this.Norm();
        if (norm == 0)
            throw new InvalidOperationException("Cannot normalize the zero vector.");

        ComplexNumber[,] result = new ComplexNumber[Rows, 1];
        for (int i = 0; i < Rows; i++)
            result[i, 0] = new ComplexNumber(Data[i, 0].Real / norm, Data[i, 0].Imag / norm);

        return new ComplexVector(ToArray(result));
    }

    // Inner product ⟨v|w⟩ = v† w
    public ComplexNumber InnerProduct(ComplexVector other)
    {
        if (this.Length != other.Length)
            throw new InvalidOperationException("Vectors must have the same dimension.");

        ComplexMatrix dagger = this.Dagger(); 
        ComplexMatrix product = dagger * other; 
        return product.Data[0, 0];
    }

  
    private static ComplexNumber[] ToArray(ComplexNumber[,] data)
    {
        int n = data.GetLength(0);
        ComplexNumber[] arr = new ComplexNumber[n];
        for (int i = 0; i < n; i++)
            arr[i] = data[i, 0];
        return arr;
    }
}
