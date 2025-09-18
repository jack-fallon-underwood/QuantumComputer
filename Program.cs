using System;

class Program
{
    static void Main(string[] args)
    {
        // Build the 8x8 starting matrix
        ComplexMatrix A = new ComplexMatrix(8, 8);

        // Row 0
        A.Data[0, 0] = new ComplexNumber(7.0 / 8.0, 0);
        A.Data[0, 2] = new ComplexNumber(6.0 / 7.0, 0);
        A.Data[0, 4] = new ComplexNumber(1.0 / 4.0, 0);
        A.Data[0, 6] = new ComplexNumber(4.0 / 5.0, 0);

        // Row 1
        A.Data[1, 0] = new ComplexNumber(1.0 / 8.0, 0);
        A.Data[1, 3] = new ComplexNumber(1.0 / 2.0, 0);

        // Row 2
        A.Data[2, 2] = new ComplexNumber(1.0 / 7.0, 0);
        A.Data[2, 4] = new ComplexNumber(1.0 / 2.0, 0);

        // Row 3
        A.Data[3, 7] = new ComplexNumber(1.0 / 3.0, 0);

        // Row 4
        // all zeros

        // Row 5
        A.Data[5, 3] = new ComplexNumber(1.0 / 2.0, 0);
        A.Data[5, 5] = new ComplexNumber(1.0, 0);
        A.Data[5, 6] = new ComplexNumber(2.0 / 3.0, 0); // assuming you meant 2/3 instead of "0.2/3"

        // Row 6
        A.Data[6, 1] = new ComplexNumber(2.0 / 5.0, 0);
        A.Data[6, 4] = new ComplexNumber(1.0 / 4.0, 0);

        // Row 7
        A.Data[7, 1] = new ComplexNumber(3.0 / 5.0, 0);
        A.Data[7, 6] = new ComplexNumber(1.0 / 5.0, 0);

        // Now test Power(A, n)
        int n = 2; // raise to the 3rd power
        ComplexMatrix result = Stochastic.Power(A, n);

        Console.WriteLine($"A^{n}:");
        PrintMatrix(result);
    }

    static void PrintMatrix(ComplexMatrix M)
    {
        for (int i = 0; i < M.Rows; i++)
        {
            for (int j = 0; j < M.Cols; j++)
            {
                Console.Write($"{M.Data[i, j].Real:F4} ");
            }
            Console.WriteLine();
        }
    }
}
