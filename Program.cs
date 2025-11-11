using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Main Menu ===");
        Console.WriteLine("1) Quantum Circuit Simulator");
        Console.WriteLine("2) RSA Encryption/Decryption Demo");
        Console.WriteLine("3) RSA Authentication (Signing) Demo");
        Console.WriteLine("4) One-Time Pad Demo");
        Console.WriteLine("5) Diffie-Hellman Demo");
        Console.WriteLine("Enter your choice: ");
        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                QuantumCircuitSimulator.Run();
                break;

            case "2":
                Cryptography.Demo_RSA_ShowItWorks();
                break;

            case "3":
                Cryptography.Demo_RSA_Authentication();
                break;

            case "4":
                Cryptography.Demo_OneTimePad();
                break;

            case "5":
                Cryptography.Demo_DiffieHellman();
                break;

            default:
                Console.WriteLine("Invalid choice. Exiting.");
                break;
        }
    }
}
