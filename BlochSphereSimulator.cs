using System;
using System.Collections.Generic;

public class BlochSphereSimulator : QuantumCircuitSimulator
{
    public List<BlochCoordinate> InitialBlochStates { get; private set; }
    public List<BlochCoordinate> FinalBlochStates { get; private set; }

    public int NumberOfQubits { get; private set; }
    public ComplexMatrix InitialState { get; private set; }
    public ComplexMatrix? FinalState { get; private set; }

    public BlochSphereSimulator(int numQubits, string classicalState)
    {
        NumberOfQubits = numQubits;
        InitialBlochStates = new List<BlochCoordinate>();
        FinalBlochStates = new List<BlochCoordinate>();

        // Initialize quantum state
        int dim = 1 << numQubits;
        InitialState = new ComplexMatrix(dim, 1);

        // Convert classical state to quantum vector
        int index = 0;
        for (int i = 0; i < numQubits; i++)
        {
            if (classicalState[i] == '1')
                index |= 1 << (numQubits - 1 - i);
        }
        InitialState.Data[index, 0] = new ComplexNumber(1, 0);

        // Calculate initial Bloch coordinates for each qubit
        CalculateBlochStates(InitialState, InitialBlochStates);
    }

    public void ApplyCircuit(List<ComplexMatrix> segmentMatrices)
    {
        // Multiply all segment matrices to get total circuit matrix
        ComplexMatrix totalCircuit = GeneralGates.Identity(1 << NumberOfQubits);
        foreach (var seg in segmentMatrices)
        {
            totalCircuit = seg * totalCircuit;
        }

        // Apply circuit to initial state
        FinalState = totalCircuit * InitialState;

        // Calculate final Bloch coordinates for each qubit
        CalculateBlochStates(FinalState, FinalBlochStates);
    }

    private void CalculateBlochStates(ComplexMatrix state, List<BlochCoordinate> blochList)
    {
        blochList.Clear();

        for (int qubit = 0; qubit < NumberOfQubits; qubit++)
        {
            var coords = CalculateBlochCoordinate(state, qubit);
            blochList.Add(coords);
        }
    }

    private BlochCoordinate CalculateBlochCoordinate(ComplexMatrix state, int targetQubit)
    {
        // Calculate reduced density matrix for the target qubit by tracing out others
        var densityMatrix = CalculateReducedDensityMatrix(state, targetQubit);

        // Extract Bloch sphere coordinates from density matrix
        // � = (I + x*�_x + y*�_y + z*�_z) / 2
        // So: x = Tr(� * �_x), y = Tr(� * �_y), z = Tr(� * �_z)

        var sigmaX = GeneralGates.X;
        var sigmaY = GeneralGates.Y;
        var sigmaZ = GeneralGates.Z;

        var rhoX = densityMatrix * sigmaX;
        var rhoY = densityMatrix * sigmaY;
        var rhoZ = densityMatrix * sigmaZ;

        double x = rhoX.Trace().Real;
        double y = rhoY.Trace().Real;
        double z = rhoZ.Trace().Real;

        return new BlochCoordinate(x, y, z);
    }

    private ComplexMatrix CalculateReducedDensityMatrix(ComplexMatrix state, int targetQubit)
    {
        // For a pure state |��, the density matrix is � = |����|
        // Then we trace out all qubits except the target

        int n = NumberOfQubits;
        int dim = 1 << n;

        // Create 2x2 reduced density matrix for single qubit
        var reducedRho = new ComplexMatrix(2, 2);

        // Iterate through all basis states
        for (int i = 0; i < dim; i++)
        {
            for (int j = 0; j < dim; j++)
            {
                int[] bitsI = GeneralGates.ToBinary(i, n);
                int[] bitsJ = GeneralGates.ToBinary(j, n);

                // Check if all qubits except target match
                bool match = true;
                for (int q = 0; q < n; q++)
                {
                    if (q != targetQubit && bitsI[q] != bitsJ[q])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    int rowIdx = bitsI[targetQubit];
                    int colIdx = bitsJ[targetQubit];

                    ComplexNumber val = state.Data[i, 0] * state.Data[j, 0].Conjugate();
                    reducedRho.Data[rowIdx, colIdx] = reducedRho.Data[rowIdx, colIdx] + val;
                }
            }
        }

        return reducedRho;
    }

    public string GetStateDescription()
    {
        var desc = "=== Bloch Sphere States ===\n\n";

        desc += "INITIAL STATES:\n";
        for (int i = 0; i < NumberOfQubits; i++)
        {
            var coord = InitialBlochStates[i];
            desc += $"Qubit {i}: �={coord.Theta:F3}, �={coord.Phi:F3} | (x={coord.X:F3}, y={coord.Y:F3}, z={coord.Z:F3})\n";
        }

        desc += "\nFINAL STATES:\n";
        for (int i = 0; i < NumberOfQubits; i++)
        {
            var coord = FinalBlochStates[i];
            desc += $"Qubit {i}: �={coord.Theta:F3}, �={coord.Phi:F3} | (x={coord.X:F3}, y={coord.Y:F3}, z={coord.Z:F3})\n";
        }

        return desc;
    }
}

public class BlochCoordinate
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    // Spherical coordinates
    public double Theta { get; set; }  // Polar angle (0 to �)
    public double Phi { get; set; }    // Azimuthal angle (0 to 2�)
    public double R { get; set; }      // Radius (should be d 1)

    public BlochCoordinate(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;

        // Convert Cartesian to spherical
        R = Math.Sqrt(x * x + y * y + z * z);

        if (R > 1e-10)
        {
            Theta = Math.Acos(Math.Clamp(z / R, -1.0, 1.0));
            Phi = Math.Atan2(y, x);
            if (Phi < 0) Phi += 2 * Math.PI;
        }
        else
        {
            Theta = 0;
            Phi = 0;
        }
    }

    public override string ToString()
    {
        return $"(x={X:F3}, y={Y:F3}, z={Z:F3}) | �={Theta:F3}, �={Phi:F3}, r={R:F3}";
    }
}