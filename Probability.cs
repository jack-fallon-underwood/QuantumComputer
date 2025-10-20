public static class Probability
{

    public static bool IsProbabilityDistribution(double[] p, double tolerance = 1e-10)
    {
        double sum = 0;
        foreach (double x in p)
        {
            if (x < -tolerance) return false;  negative probabilities not allowed
            sum += x;
        }
        return Math.Abs(sum - 1.0) < tolerance;
    }
    public static (double ExpectedValue, double Dispersion) ExpectedValueAndDispersion(double[] values, double[] probabilities, double tolerance = 1e-10)
    {
        if (values.Length != probabilities.Length)
            throw new InvalidOperationException("Values and probability arrays must have the same length.");

        if (!IsProbabilityDistribution(probabilities, tolerance))
            throw new InvalidOperationException("Provided probabilities do not form a valid distribution.");

        double expected = 0;
        double expectedSquare = 0;

        for (int i = 0; i < values.Length; i++)
        {
            expected += values[i] * probabilities[i];
            expectedSquare += values[i] * values[i] * probabilities[i];
        }

        double dispersion = expectedSquare - expected * expected;
        return (expected, dispersion);
    }


}