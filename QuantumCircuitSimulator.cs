using System;
using System.Collections.Generic;

public class QuantumCircuitSimulator
{
    // Main method moved to WPF App - keep this class for inheritance
    public static void MainConsole()
    {
        Console.WriteLine("=== Quantum Circuit Simulator ===");

        // 1. Number of qubits
        Console.Write("Enter number of qubits (n): ");
        int n = int.Parse(Console.ReadLine() ?? "1");

        // 2. Starting classical state
        Console.WriteLine($"Enter starting classical state of {n} qubits (0 or 1, e.g., 010):");
        string stateStr = Console.ReadLine() ?? "";
        if (stateStr.Length != n)
        {
            Console.WriteLine("Error: State length does not match number of qubits.");
            return;
        }

        // Convert classical state to quantum vector |state>
        int dim = 1 << n;
        var initialState = new ComplexMatrix(dim, 1);
        int index = 0;
        for (int i = 0; i < n; i++)
            if (stateStr[i] == '1') index |= 1 << (n - 1 - i);
        initialState.Data[index, 0] = new ComplexNumber(1, 0);

        // 3. Number of segments (vertical slices)
        Console.Write("Enter number of segments in the circuit: ");
        int segments = int.Parse(Console.ReadLine() ?? "1");

        // 4. Process each segment
        var segmentMatrices = new List<ComplexMatrix>();
        for (int s = 0; s < segments; s++)
        {
            Console.WriteLine($"\n--- Segment {s + 1} ---");
            var segmentOps = new List<ComplexMatrix>();

            while (true)
            {
                Console.WriteLine("Enter gate name (X, Y, Z, H, Rx, Ry, Rz, Phase, CNOT, TOFFOLI, FREDKIN) or DONE to finish this segment:");
                string gate = Console.ReadLine()?.Trim();
                if (gate == null || gate.ToUpper() == "DONE") break;

                ComplexMatrix g = null;

                if (gate.ToUpper() == "CNOT")
                {
                    Console.Write("Enter control qubit index: ");
                    int c = int.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Enter target qubit index: ");
                    int t = int.Parse(Console.ReadLine() ?? "0");
                    g = GeneralGates.ControlledGate(n, c, t, GeneralGates.X);
                }
                else if (gate.ToUpper() == "TOFFOLI")
                {
                    Console.Write("Enter first control qubit index: ");
                    int c1 = int.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Enter second control qubit index: ");
                    int c2 = int.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Enter target qubit index: ");
                    int t = int.Parse(Console.ReadLine() ?? "0");
                    g = GeneralGates.ToffoliGate(n, c1, c2, t);
                }
                else if (gate.ToUpper() == "FREDKIN")
                {
                    Console.Write("Enter control qubit index: ");
                    int c = int.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Enter first swap qubit index: ");
                    int q1 = int.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Enter second swap qubit index: ");
                    int q2 = int.Parse(Console.ReadLine() ?? "0");
                    g = GeneralGates.FredkinGate(n, c, q1, q2);
                }
                else
                {
                    // Ask which qubits
                    Console.WriteLine("Enter qubit index(es) the gate acts on (comma-separated for multiple qubits):");
                    string[] qubitsStr = Console.ReadLine()?.Split(',') ?? new string[0];
                    var qubits = new List<int>();
                    foreach (var q in qubitsStr) if (int.TryParse(q.Trim(), out int qi)) qubits.Add(qi);

                    // Special parameters for rotations / phase
                    double param = 0;
                    if (gate.StartsWith("R") || gate.StartsWith("Phase", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Enter parameter theta (in radians):");
                        param = double.Parse(Console.ReadLine() ?? "0");
                    }

                    // Build single gate
                    switch (gate.ToUpper())
                    {
                        case "X": g = GeneralGates.X; break;
                        case "Y": g = GeneralGates.Y; break;
                        case "Z": g = GeneralGates.Z; break;
                        case "H": g = GeneralGates.H; break;
                        case "RX": g = GeneralGates.Rx(param); break;
                        case "RY": g = GeneralGates.Ry(param); break;
                        case "RZ": g = GeneralGates.Rz(param); break;
                        case "PHASE": g = GeneralGates.Phase(param); break;
                        default:
                            Console.WriteLine("Unknown gate.");
                            continue;
                    }

                    // Embed the gate into the n-qubit system
                    foreach (var q in qubits)
                    {
                        g = GeneralGates.EmbedSingleQubitGate(n, q, g);
                    }
                }

                if (g != null) segmentOps.Add(g);
            }

            // Multiply all gates in this segment (left to right)
            ComplexMatrix segmentMat = GeneralGates.Identity(1 << n);
            foreach (var g in segmentOps) segmentMat = g * segmentMat;

            segmentMatrices.Add(segmentMat);
        }

        // Multiply all segment matrices to get total circuit matrix
        ComplexMatrix totalCircuit = GeneralGates.Identity(1 << n);
        foreach (var seg in segmentMatrices) totalCircuit = seg * totalCircuit;

        // Apply circuit to initial state
        ComplexMatrix finalState = totalCircuit * initialState;

        Console.WriteLine("\n=== Final quantum state vector ===");
        for (int i = 0; i < finalState.Rows; i++)
        {
            Console.WriteLine($"|{Convert.ToString(i, 2).PadLeft(n, '0')}>: {finalState.Data[i, 0]}");
        }

        // Ask for measurement
        Console.WriteLine("\nEnter qubits to measure (comma-separated):");
        string[] measureQStr = Console.ReadLine()?.Split(',') ?? new string[0];
        var measureQ = new List<int>();
        foreach (var q in measureQStr) if (int.TryParse(q.Trim(), out int qi)) measureQ.Add(qi);

        Console.WriteLine("\n=== Measurement probabilities ===");
        for (int i = 0; i < finalState.Rows; i++)
        {
            int[] bits = GeneralGates.ToBinary(i, n);
            bool relevant = true;
            foreach (var q in measureQ)
                if (bits[q] != 1) relevant = false;

            double prob = finalState.Data[i, 0].Modulus();
            prob *= prob; // square modulus
            if (prob > 1e-12)
            {
                Console.WriteLine($"State |{Convert.ToString(i, 2).PadLeft(n, '0')}>: Probability {prob:F4}");
            }
        }

        Console.WriteLine("\nSimulation complete.");
    }
}
