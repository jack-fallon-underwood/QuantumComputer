using System;
using System.Linq;

public static class Binary
{
    public static char GetBit(string binary, int j)
    {
        if (j < 0 || j >= binary.Length)
            throw new ArgumentOutOfRangeException("Index out of range.");
        return binary[j];
    }

    public static string Complement(string binary)
    {
        return new string(binary.Select(c => c == '0' ? '1' : '0').ToArray());
    }

    public static int Parity(string binary)
    {
        int ones = binary.Count(c => c == '1');
        return ones % 2;
    }

    public static string XOR(string x, string y)
    {
        if (x.Length != y.Length)
            throw new ArgumentException("Strings must be the same length.");

        return new string(x.Zip(y, (a, b) => a == b ? '0' : '1').ToArray());
    }

    public static string Conjunction(string x, string y)
    {
        if (x.Length != y.Length)
            throw new ArgumentException("Strings must be the same length.");

        return new string(x.Zip(y, (a, b) => (a == '1' && b == '1') ? '1' : '0').ToArray());
    }

    public static int InnerProduct(string x, string y)
    {
        if (x.Length != y.Length)
            throw new ArgumentException("Strings must be the same length.");

        string conj = Conjunction(x, y);
        return Parity(conj);
    }

    public static int ToNumber(string binary)
    {
        return Convert.ToInt32(binary, 2);
    }

    public static string ToBinary(int number)
    {
        if (number < 0)
            throw new ArgumentException("Number must be non-negative.");
        return Convert.ToString(number, 2);
    }
}
