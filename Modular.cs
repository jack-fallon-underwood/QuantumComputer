using System;

public static class Modular
{
    public static int Mod(int a, int n)
    {
        if (n <= 0) throw new ArgumentException("Modulus must be positive.");
        int result = a % n;
        return result < 0 ? result + n : result;
    }

    public static int Add(int a, int b, int n)
    {
        return Mod(a + b, n);
    }

    public static int Multiply(int a, int b, int n)
    {
        return Mod(a * b, n);
    }

    public static int Power(int a, int b, int n)
    {
        if (n <= 0) throw new ArgumentException("Modulus must be positive.");
        int result = 1;
        a = Mod(a, n);
        while (b > 0)
        {
            if ((b & 1) == 1)
                result = (result * a) % n;
            a = (a * a) % n;
            b >>= 1;
        }
        return result;
    }

      // Extended Euclidean Algorithm for Modular Division
    private static (int gcd, int x, int y) ExtendedGCD(int a, int b)
    {
        if (b == 0)
            return (a, 1, 0);

        var (gcd, x1, y1) = ExtendedGCD(b, a % b);
        int x = y1;
        int y = x1 - (a / b) * y1;
        return (gcd, x, y);
    }

    // Modular Inverse for Modular Division
    public static int ModInverse(int b, int n)
    {
        var (gcd, x, _) = ExtendedGCD(b, n);
        if (gcd != 1)
            throw new InvalidOperationException($"No modular inverse: {b} and {n} are not coprime.");

        return (x % n + n) % n; // ensure positive
    }

    // Modular Division: a / b mod n
    public static int Divide(int a, int b, int n)
    {
        int inv = ModInverse(b, n);
        return (a * inv) % n;
    }

    public static int GCD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    public static bool AreCoprime(int a, int b)
    {
        return GCD(a, b) == 1;
    }

    public static int Totient(int n)
    {
        if (n <= 0) throw new ArgumentException("Input must be positive.");
        if (n > 10_000) throw new ArgumentException("n too large for naive totient calculation.");
        
        int result = 1;
        for (int i = 2; i < n; i++)
        {
            if (AreCoprime(i, n))
                result++;
        }
        return result;
    }

    public static bool FermatsLittleTheorem(int a, int p)
    {
        if (p <= 1) throw new ArgumentException("p must be prime.");
        return Power(a, p, p) == a % p;
    }

    public static bool EulersTheorem(int a, int n)
    {
        if (!AreCoprime(a, n))
            return false;

        int phi = Totient(n);
        return Power(a, phi, n) == 1;
    }
}
