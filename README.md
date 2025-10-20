# QuantumComputer# Quantum Computing Simulation Toolkit (C#)

This project is a collection of C# scripts designed to simulate quantum circuits and perform related quantum and classical computations. The main program, `QuantumCircuitSimulator`, provides an interactive console-based tool to build and run custom quantum circuits.

The collection is supported by a comprehensive library of classes for linear algebra, quantum mechanics, and classical helper utilities.

## Acknowledgment

This project was developed with instruction from and based on exercises in the textbook **"Quantum Computing"** by **Noson S. Yanofsky**, as part of his graduate course taught at Brooklyn College.

---

## 1. Main Program: `QuantumCircuitSimulator`

This is the main executable program. It provides an interactive console interface to build and simulate a quantum circuit step-by-step.

### How it Works

1.  **Initialize:** The user specifies the total number of qubits ($n$) in the register.
2.  **Set Initial State:** The user provides a classical bitstring (e.g., `010`) to define the initial state vector (e.g., $|010\rangle$).
3.  **Build Circuit:** The user defines the circuit in "segments" (vertical slices).
    * In each segment, the user can add one or more gates that act in parallel.
    * The simulator constructs the unitary matrix for each segment by taking the tensor product of the individual gate matrices (or identity matrices for qubits with no gate).
4.  **Simulate:** The program multiplies all segment matrices to get the final circuit unitary $U_{circuit}$.
5.  **Calculate Final State:** It applies the total circuit to the initial state: $|\psi_{final}\rangle = U_{circuit} |\psi_{initial}\rangle$.
6.  **Display Results:**
    * The complete final state vector is printed, showing the complex amplitude for each basis state.
    * The measurement probabilities ($|\text{amplitude}|^2$) for each basis state are displayed.

### Supported Gates

* **Single-Qubit:** `X`, `Y`, `Z`, `H`
* **Parametrized:** `Rx`, `Ry`, `Rz`, `Phase` (all require a $\theta$ parameter in radians)
* **Multi-Qubit:** `CNOT`, `TOFFOLI` (CCNOT), `FREDKIN` (CSWAP)

---

## 2. Simulator Walkthrough: Creating a Bell State

Let's create the Bell state $|\Phi^+\rangle = \frac{1}{\sqrt{2}}(|00\rangle + |11\rangle)$. This requires a 2-qubit system, starting in state $|00\rangle$.

1.  Apply a **Hadamard (H)** gate to qubit 0.
2.  Apply a **CNOT** gate with control qubit 0 and target qubit 1.

### Example Console Session

```text
=== Quantum Circuit Simulator ===
Enter number of qubits (n): 2
Enter starting classical state of 2 qubits (0 or 1, e.g., 010):
00
Enter number of segments in the circuit: 2

--- Segment 1 ---
Enter gate name (X, Y, Z, H, Rx, Ry, Rz, Phase, CNOT, TOFFOLI, FREDKIN) or DONE:
H
Enter qubit index(es) the gate acts on (comma-separated for multiple qubits):
0
Enter gate name (...) or DONE:
DONE

--- Segment 2 ---
Enter gate name (...) or DONE:
CNOT
Enter control qubit index: 0
Enter target qubit index: 1
Enter gate name (...) or DONE:
DONE

=== Final quantum state vector ===
|00>: 0.7071067811865475 + 0i
|01>: 0 + 0i
|10>: 0 + 0i
|11>: 0.7071067811865475 + 0i

Enter qubits to measure (comma-separated):
0,1

=== Measurement probabilities ===
State |00>: Probability 0.5000
State |11>: Probability 0.5000

Simulation complete.
```


## 3. Core Library Classes

This collection includes several classes that provide the building blocks for the simulator.

### `ComplexNumber.cs`
- **Purpose:** Implements a complex number with **Real** and **Imag** properties.
- **Features:**
  - Standard arithmetic operators: `+`, `-`, `*`, `/`.
  - Methods: `Conjugate()`, `Modulus()`.
  - Includes a **PolarComplex** struct for conversions to and from polar/exponential notation.

### `ComplexMatrix.cs`
- **Purpose:** Core class for matrix operations, storing data as a 2D array of **ComplexNumber**.
  
- **Key Operations:**
  - Matrix arithmetic (addition, multiplication, scalar multiplication).
  - Methods:
    - `Conjugate()`
    - `Transpose()`
    - `Dagger()` (Hermitian conjugate).
    - `TensorProduct(A, B)`: Computes the tensor product $A \otimes B$.

- **Matrix Properties:**
  - `IsSymmetric()`
  - `IsOrthogonal()`
  - `IsHermitian()`
  - `IsUnitary()`
  - `EigenCheck(lambda, v)`: Verifies if $A|v\rangle = \lambda|v\rangle$.

### `ComplexVector.cs`
- **Purpose:** Inherits from `ComplexMatrix` as a specialization for $n \times 1$ column vectors.
  
- **Vector Operations:**
  - `Norm()`: Calculates the Euclidean norm $\sqrt{\langle v | v \rangle}$.
  - `Normalize()`: Returns a unit vector.
  - `InnerProduct(other)`: Calculates the inner product $\langle this | other \rangle$.

### `GeneralGates.cs`
- **Purpose:** Defines standard quantum gates (e.g., `X`, `H`, `Rx(θ)`) as static `ComplexMatrix` instances.
  
- **Constructors:**
  - `EmbedSingleQubitGate`: Applies a 1-qubit gate to a specific qubit in an $n$-qubit system using tensor products.
  - `ControlledGate`, `ToffoliGate`, `FredkinGate`: Build multi-qubit gates.

### `QuantumStates.cs`
- **Purpose:** Provides helper methods for creating and analyzing quantum states.
  
- **Key Methods:**
  - `KetFromBits(string bits)`: Creates a basis state vector (e.g., $|01\rangle$).
  - `UniformKet(int n)`: Creates a uniform superposition state over $n$ qubits.

### `QuantumUtils.cs`
- **Purpose:** Contains physics-related quantum calculations.
  
- **Key Functions:**
  - `ExpectedValue(O, ψ)`: Calculates the expectation value $\langle\psi|O|\psi\rangle$.
  - `Dispersion(O, ψ)`: Calculates the variance $(\Delta O)^2$ of an observable.
  - `Commutator(A, B)`: Calculates the commutator $[A, B] = AB - BA$.

### `Stochastic.cs`
- **Purpose:** Utility class for operations related to classical stochastic matrices.
  
- **Features:** 
  - Checks if the matrix is column stochastic.
  - Operations for state evolution (e.g., `StateAfterNSteps`).

### `BasisChecker.cs`
- **Purpose:** Analyzes a set of basis vectors (represented as columns in a `ComplexMatrix`).
  
- **Properties:**
  - Checks if the basis is `IsNormalBasis`, `IsOrthogonalBasis`, or `IsOrthonormalBasis`.

### `Binary.cs`
- **Purpose:** Static helper class for bitstring manipulations.
  
- **Functions:**
  - Includes methods for `Parity`, `XOR`, `Conjunction`, `InnerProduct`, and type conversions.

### `Probability.cs`
- **Purpose:** Helper functions for classical probability distributions.
  
- **Key Functions:**
  - `IsProbabilityDistribution`, `ExpectedValueAndDispersion`.

### `Modular.cs`
- **Purpose:** Library for modular arithmetic.
  
- **Functions:**
  - `Add`, `Multiply`, `Power`, `ModInverse`, `Totient`.


## 4. Requirements & Setup

### Dependencies
- A C# compiler / .NET SDK (e.g., .NET 6 or later).
- All `.cs` files from this project.

### Compilation and Execution

#### Using .NET CLI:
```bash
# Create a new console project (if needed)
dotnet new console -n QuantumSimulator

# Copy all .cs files into the new directory
# (overwriting the default Program.cs)

# Build the project
dotnet build

# Run the simulator
dotnet run
```
