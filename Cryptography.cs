using System;

public static class Cryptography
{
   
    private static bool IsPrime(int n)
    {
        if (n <= 1) return false;
        if (n <= 3) return true;
        if (n % 2 == 0) return false;
        int r = (int)Math.Sqrt(n);
        for (int i = 3; i <= r; i += 2)
            if (n % i == 0) return false;
        return true;
    }

    private static int PickRandomPrime(Random rng, int low = 101, int high = 997)
    {
        // ensure low is odd
        if (low % 2 == 0) low++;
        int attempts = 0;
        while (attempts < 20000)
        {
            int candidate = rng.Next(low, high + 1);
            if (candidate % 2 == 0) candidate++; // make odd
            if (candidate > high) candidate = low + (candidate - low) % (high - low + 1);
            if (IsPrime(candidate)) return candidate;
            attempts++;
        }
        throw new Exception("Couldn't find a prime in the given range (increase range).");
    }

    // --- 1) Alice in RSA: generate keys, output N and e, ask for C and decode it ---
    // Returns (N, e, d, p, q) so other code can use private key if desired.
    public static (int N, int e, int d, int p, int q) AliceGenerateRSAAndDecode()
    {
        var rng = new Random();
        int p = PickRandomPrime(rng, 101, 400); 
        int q = PickRandomPrime(rng, 401, 800); 
        if (q == p) q = PickRandomPrime(rng, 401, 997);

        int N = Modular.Multiply(p, q, int.MaxValue); // not modular usage; product fits in int for these sizes
        int phi = (p - 1) * (q - 1);

        int e = 3;
        while (e < phi)
        {
            if (Modular.AreCoprime(e, phi)) break;
            e += 2;
        }

        int d = Modular.ModInverse(e, phi);
        Console.WriteLine($"[Alice] Generated RSA keys. N = {N}, e = {e} (private d hidden).");
        Console.WriteLine("[Alice] Enter integer ciphertext C to decode (e.g., numeric ciphertext):");
        string input = Console.ReadLine() ?? "";
        if (!int.TryParse(input.Trim(), out int C))
        {
            Console.WriteLine("[Alice] Invalid input; expected integer ciphertext. Aborting decode.");
            return (N, e, d, p, q);
        }

        // Decrypt: m = C^d mod N
        int M = Modular.Power(C, d, N);
        Console.WriteLine($"[Alice] Decrypted ciphertext C={C} -> M={M}");
        return (N, e, d, p, q);
    }

    // --- 2) Bob in RSA: accepts N, e and M and returns C ---
    public static int BobEncryptRSA(int N, int e, int M)
    {
        // C = M^e mod N
        return Modular.Power(M, e, N);
    }

    // --- 3)
    public static void Demo_RSA_ShowItWorks()
    {
        Console.WriteLine("=== RSA Demo ===");
        var rng = new Random();
        int p = PickRandomPrime(rng, 101, 400);
        int q = PickRandomPrime(rng, 401, 800);
        if (p == q) q = PickRandomPrime(rng, 401, 997);
        int N = p * q;
        int phi = (p - 1) * (q - 1);
        int e = 3;
        while (e < phi && !Modular.AreCoprime(e, phi)) e += 2;
        int d = Modular.ModInverse(e, phi);
        Console.WriteLine($"[Demo] Alice publishes (N={N}, e={e}). Alice keeps d private.");
        int M = rng.Next(2, N - 1);
        Console.WriteLine($"[Demo] Bob chooses message M={M} and encrypts it.");
        int C = BobEncryptRSA(N, e, M);
        Console.WriteLine($"[Demo] Bob sends ciphertext C={C} to Alice.");
        int decrypted = Modular.Power(C, d, N);
        Console.WriteLine($"[Demo] Alice decrypts C with d and recovers M'={decrypted}.");
        Console.WriteLine(decrypted == M ? "[Demo] Success: decrypted == original message." : "[Demo] Failure: mismatch.");
    }

    // --- 4) Alice in Authentication RSA protocol (signing)
    public static (int N, int e, int d, int M, int signature) AliceAuthSign()
    {
        var rng = new Random();
        int p = PickRandomPrime(rng, 101, 400);
        int q = PickRandomPrime(rng, 401, 800);
        if (p == q) q = PickRandomPrime(rng, 401, 997);

        int N = p * q;
        int phi = (p - 1) * (q - 1);

        int e = 3;
        while (e < phi && !Modular.AreCoprime(e, phi)) e += 2;
        int d = Modular.ModInverse(e, phi);

        Console.WriteLine($"[AliceAuth] Generated key pair. Public (N={N}, e={e}). Private d is kept secret.");
        Console.WriteLine("[AliceAuth] Enter integer message M to sign:");
        string inM = Console.ReadLine() ?? "";
        if (!int.TryParse(inM.Trim(), out int M))
        {
            Console.WriteLine("[AliceAuth] Invalid integer; aborting signing.");
            return (N, e, d, 0, 0);
        }

        int signature = Modular.Power(M, d, N);
        Console.WriteLine($"[AliceAuth] Signature s = {signature} (sent to verifier along with M).");
        return (N, e, d, M, signature);
    }

    // --- 5) Bob verify signature
    public static bool BobVerifyRSASignature(int N, int e, int M, int signature)
    {
        int recovered = Modular.Power(signature, e, N);
        Console.WriteLine($"[BobVerify] computed s^e mod N = {recovered}. Expected M = {M}.");
        return recovered == M;
    }

    // --- 6) 
    public static void Demo_RSA_Authentication()
    {
        Console.WriteLine("=== RSA Authentication (Signing) Demo ===");
        var (N, e, d, M, s) = AliceAuthSign();
        if (N == 0) return; // aborted signing
        Console.WriteLine("[DemoAuth] Bob verifying signature...");
        bool ok = BobVerifyRSASignature(N, e, M, s);
        Console.WriteLine(ok ? "[DemoAuth] Signature valid." : "[DemoAuth] Signature INVALID.");
    }

    // --- 7) One-time pad encrypt
    public static byte[] OneTimePadEncrypt(byte[] pad, byte[] message)
    {
        if (pad == null || message == null) throw new ArgumentNullException("pad/message");
        if (pad.Length != message.Length) throw new ArgumentException("Pad and message must be the same length.");
        byte[] cipher = new byte[message.Length];
        for (int i = 0; i < message.Length; i++)
            cipher[i] = (byte)(message[i] ^ pad[i]);
        return cipher;
    }

    // --- 8) One-time pad decrypt
    public static byte[] OneTimePadDecrypt(byte[] pad, byte[] cipher)
    {
        return OneTimePadEncrypt(pad, cipher);
    }

    // --- 9)
    public static void Demo_OneTimePad()
    {
        Console.WriteLine("=== One-Time Pad Demo ===");
        string message = "HELLO"; 
        byte[] mbytes = System.Text.Encoding.ASCII.GetBytes(message);
        var rng = new Random();
        byte[] pad = new byte[mbytes.Length];
        rng.NextBytes(pad);

        byte[] cipher = OneTimePadEncrypt(pad, mbytes);
        Console.WriteLine($"Plaintext: {message}");
        Console.WriteLine($"Cipher (bytes): {BitConverter.ToString(cipher)}");

        byte[] recovered = OneTimePadDecrypt(pad, cipher);
        string recoveredText = System.Text.Encoding.ASCII.GetString(recovered);
        Console.WriteLine($"Recovered plaintext: {recoveredText}");
        Console.WriteLine(recoveredText == message ? "[OTP Demo] Success" : "[OTP Demo] Failure");
    }

    // --- 10) Diffie-Hellman user function ---
    public static int DiffieHellmanUser(int p, int g, int bPrimePublic, int mySecret)
    {
        // other public value is g^b' mod p (we assume bPrimePublic is actually the numeric b' (private) OR otherPublic)
        // To follow your wording strictly: if bPrimePublic is b' (private of other), we compute otherPublic = g^b' mod p, then shared = otherPublic^mySecret mod p.
        // But commonly we pass other's public value; here we'll treat bPrimePublic as the other's private exponent and compute public then shared.
        int otherPublic = Modular.Power(g, bPrimePublic, p);
        int shared = Modular.Power(otherPublic, mySecret, p);
        return shared;
    }


    public static int DH_GeneratePublic(int p, int g, int mySecret) => Modular.Power(g, mySecret, p);
    public static int DH_ComputeShared(int p, int mySecret, int otherPublic) => Modular.Power(otherPublic, mySecret, p);

    // --- 11) 
    public static void Demo_DiffieHellman()
    {
        Console.WriteLine("=== Diffie-Hellman Demo ===");

        int p = 30803;          
        int g = 2;               
        var rng = new Random();

        int a = rng.Next(50, 500); 
        int b = rng.Next(50, 500); 

        int A = DH_GeneratePublic(p, g, a); 
        int B = DH_GeneratePublic(p, g, b); 

        int sharedA = DH_ComputeShared(p, a, B);
        int sharedB = DH_ComputeShared(p, b, A);

        Console.WriteLine($"Alice private a={a}, public A={A}");
        Console.WriteLine($"Bob private b={b}, public B={B}");
        Console.WriteLine($"Alice computes shared = {sharedA}");
        Console.WriteLine($"Bob   computes shared = {sharedB}");
        Console.WriteLine(sharedA == sharedB ? "[DH Demo] Success: shared secrets match." : "[DH Demo] Failure: mismatch.");
    }

  
}
