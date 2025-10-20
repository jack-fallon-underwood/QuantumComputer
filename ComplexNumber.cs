public class ComplexNumber
{
    public double Real { get; set; }
    public double Imag { get; set; }

    public ComplexNumber(double real, double imag)
    {
        Real = real;
        Imag = imag;
    }


    public ComplexNumber Conjugate()
    {
        return new ComplexNumber(Real, -Imag);
    }


    public double Modulus()
    {
        return Math.Sqrt(Real * Real + Imag * Imag);
    }


    public ComplexNumber Normalized()
    {
        double mod = Modulus();
        if (mod == 0)
            throw new InvalidOperationException("Cannot normalize the zero complex number.");
        return new ComplexNumber(Real / mod, Imag / mod);
    }

    public static double ModuliDifference(ComplexNumber a, ComplexNumber b)
    {
        double sumOfModuli = a.Modulus() + b.Modulus();
        double modulusOfSum = (a + b).Modulus();
        return sumOfModuli - modulusOfSum;
    }

    public PolarComplex Gen2Polar()
    {
        double r = Modulus();
        double theta = Math.Atan2(Imag, Real);
        return new PolarComplex(r, theta);
    }

    public PolarComplex Gen2Exp()
    {
        return Gen2Polar();
    }

    public override string ToString()
    {
        return $"{Real} {(Imag >= 0 ? "+" : "-")} {Math.Abs(Imag)}i";
    }

    public static ComplexNumber operator +(ComplexNumber a, ComplexNumber b)
    {
        return new ComplexNumber(a.Real + b.Real, a.Imag + b.Imag);
    }

    public static ComplexNumber operator -(ComplexNumber a, ComplexNumber b)
    {
        return new ComplexNumber(a.Real - b.Real, a.Imag - b.Imag);
    }

    public static ComplexNumber operator *(ComplexNumber a, ComplexNumber b)
    {
        double real = a.Real * b.Real - a.Imag * b.Imag;
        double imag = a.Real * b.Imag + a.Imag * b.Real;
        return new ComplexNumber(real, imag);
    }

    public static ComplexNumber operator /(ComplexNumber a, ComplexNumber b)
    {
        double denominator = b.Real * b.Real + b.Imag * b.Imag;
        if (denominator == 0)
            throw new DivideByZeroException("Cannot divide by zero complex number.");
        double real = (a.Real * b.Real + a.Imag * b.Imag) / denominator;
        double imag = (a.Imag * b.Real - a.Real * b.Imag) / denominator;
        return new ComplexNumber(real, imag);
    }

   public static ComplexNumber operator -(ComplexNumber a) => new ComplexNumber(-a.Real, -a.Imag);
public static ComplexNumber operator *(ComplexNumber a, double scalar) => new ComplexNumber(a.Real * scalar, a.Imag * scalar);
public static ComplexNumber operator *(double scalar, ComplexNumber a) => a * scalar;




}

// Polar/Exponential complex number
public struct PolarComplex
{
    public double Magnitude { get; set; } // r
    public double Angle { get; set; }     // theta in radians

    public PolarComplex(double r, double theta)
    {
        Magnitude = r;
        Angle = theta;
    }

    public ComplexNumber Polar2Gen()
    {
        double real = Magnitude * Math.Cos(Angle);
        double imag = Magnitude * Math.Sin(Angle);
        return new ComplexNumber(real, imag);
    }

    public PolarComplex Polar2Exp()
    {
        return new PolarComplex(Magnitude, Angle);
    }

    public PolarComplex Exp2Polar()
    {
        return new PolarComplex(Magnitude, Angle);
    }

    public ComplexNumber Exp2Gen()
    {
        return Polar2Gen();
    }

    public string ToPolarString()
    {
        return $"{Magnitude} ∠ {Angle} rad";
    }

    public string ToExpString()
    {
        return $"{Magnitude} * e^(i{Angle})";
    }
}