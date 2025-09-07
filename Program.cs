using System;



class Program
{
    static void Main()
    {
        ComplexNumber z = new ComplexNumber(3, 4);
        Console.WriteLine("General: " + z);

        Console.WriteLine("Conjugate: " + z.Conjugate());
        Console.WriteLine("Modulus: " + z.Modulus());
        Console.WriteLine("Normalized: " + z.Normalized());

        PolarComplex polar = z.Gen2Polar();
        Console.WriteLine("Polar: " + polar.ToPolarString());

        PolarComplex exp = z.Gen2Exp();
        Console.WriteLine("Exponential: " + exp.ToExpString());

        ComplexNumber backToGen = polar.Polar2Gen();
        Console.WriteLine("Back to General: " + backToGen);
    }
}
