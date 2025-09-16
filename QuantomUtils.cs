public static class QuantumUtils
{
    public static ComplexNumber ExpectedValue(
        ComplexMatrix observable,
        List<ComplexMatrix> eigenvectors,
        ComplexMatrix state)
    {
        ComplexMatrix result = new ComplexMatrix(1, 1);
        ComplexMatrix bra = state.Dagger();
        result = bra * observable * state;
        return result.Data[0, 0];
    }

    public static ComplexNumber Dispersion(
        ComplexMatrix observable,
        List<ComplexMatrix> eigenvectors,
        ComplexMatrix state)
    {
        ComplexMatrix expectedMatrix = new ComplexMatrix(observable.Rows, observable.Cols);
        for (int i = 0; i < observable.Rows; i++)
            for (int j = 0; j < observable.Cols; j++)
                expectedMatrix.Data[i, j] = observable.Data[i, j];

        ComplexNumber expectedValue = ExpectedValue(observable, eigenvectors, state);

        // Compute (O - <O>)^2
        ComplexMatrix diff = new ComplexMatrix(observable.Rows, observable.Cols);
        for (int i = 0; i < observable.Rows; i++)
            for (int j = 0; j < observable.Cols; j++)
                diff.Data[i, j] = new ComplexNumber(
                    observable.Data[i, j].Real - (i == j ? expectedValue.Real : 0),
                    observable.Data[i, j].Imag - (i == j ? expectedValue.Imag : 0)
                );

        ComplexMatrix squared = diff * diff;
        ComplexMatrix bra = state.Dagger();
        ComplexMatrix result = bra * squared * state;
        return result.Data[0, 0];
    }

    public static ComplexMatrix Commutator(ComplexMatrix A, ComplexMatrix B)
{
    return (A * B) + (new ComplexNumber(-1, 0) * (B * A));
}


}
