
class BasisChecker
{
    public static bool IsNormalBasis(ComplexMatrix basis)
    {
        int cols = basis.Cols;
        for (int j = 0; j < cols; j++)
        {
            ComplexVector v = new ComplexVector(GetColumn(basis, j));
            if (Math.Abs(v.Norm() - 1.0) > 1e-10)
                return false;
        }
        return true;
    }

    public static bool IsOrthogonalBasis(ComplexMatrix basis)
    {
        int cols = basis.Cols;
        for (int i = 0; i < cols; i++)
        {
            ComplexVector vi = new ComplexVector(GetColumn(basis, i));
            for (int j = i + 1; j < cols; j++)
            {
                ComplexVector vj = new ComplexVector(GetColumn(basis, j));
                if (vi.InnerProduct(vj).Modulus() > 1e-10)
                    return false;
            }
        }
        return true;
    }

    public static bool IsOrthonormalBasis(ComplexMatrix basis)
    {
        return IsNormalBasis(basis) && IsOrthogonalBasis(basis);
    }

    private static ComplexNumber[] GetColumn(ComplexMatrix mat, int col)
    {
        int rows = mat.Rows;
        ComplexNumber[] column = new ComplexNumber[rows];
        for (int i = 0; i < rows; i++)
            column[i] = mat.Data[i, col];
        return column;
    }
}
