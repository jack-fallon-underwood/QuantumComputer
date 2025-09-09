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
