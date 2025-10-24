using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace QuantumBlochSphere
{
    public partial class MainWindow : Window
    {
        private List<GateOperation> circuitGates = new List<GateOperation>();

        public MainWindow()
        {
            InitializeComponent();
            cmbGateType.SelectionChanged += CmbGateType_SelectionChanged;
        }

        private void CmbGateType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (cmbGateType.SelectedItem as ComboBoxItem)?.Content.ToString();

            // Show/hide controls based on gate type
            bool isRotation = selected?.Contains("Rotation") == true || selected?.StartsWith("R") == true;
            bool isCNOT = selected?.Contains("CNOT") == true;

            lblRotationAngle.Visibility = isRotation ? Visibility.Visible : Visibility.Collapsed;
            txtRotationAngle.Visibility = isRotation ? Visibility.Visible : Visibility.Collapsed;

            lblControlQubit.Visibility = isCNOT ? Visibility.Visible : Visibility.Collapsed;
            txtControlQubit.Visibility = isCNOT ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BtnAddGate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var gateType = (cmbGateType.SelectedItem as ComboBoxItem)?.Content.ToString();
                int targetQubit = int.Parse(txtTargetQubit.Text);
                int controlQubit = -1;
                double angle = 0;

                if (gateType?.Contains("CNOT") == true)
                {
                    controlQubit = int.Parse(txtControlQubit.Text);
                }

                if (gateType?.Contains("Rotation") == true || gateType?.StartsWith("R") == true)
                {
                    angle = double.Parse(txtRotationAngle.Text);
                }

                circuitGates.Add(new GateOperation
                {
                    GateType = gateType ?? "H",
                    TargetQubit = targetQubit,
                    ControlQubit = controlQubit,
                    Angle = angle
                });

                UpdateCircuitDescription();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding gate: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            circuitGates.Clear();
            canvasBlochSpheres.Children.Clear();
            UpdateCircuitDescription();
        }

        private void UpdateCircuitDescription()
        {
            if (circuitGates.Count == 0)
            {
                txtCircuitDescription.Text = "(No gates added)";
                return;
            }

            var desc = "";
            for (int i = 0; i < circuitGates.Count; i++)
            {
                var gate = circuitGates[i];
                desc += $"{i + 1}. {gate.GateType}";
                if (gate.ControlQubit >= 0)
                    desc += $" (Control: {gate.ControlQubit}, Target: {gate.TargetQubit})";
                else
                    desc += $" (Qubit {gate.TargetQubit})";

                if (gate.Angle != 0)
                    desc += $" θ={gate.Angle:F3}";

                desc += "\n";
            }
            txtCircuitDescription.Text = desc;
        }

        private void BtnSimulate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int numQubits = int.Parse(txtNumQubits.Text);
                string initialState = txtInitialState.Text;

                if (initialState.Length != numQubits)
                {
                    MessageBox.Show($"Initial state length ({initialState.Length}) must match number of qubits ({numQubits})",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Create simulator
                var simulator = new BlochSphereSimulator(numQubits, initialState);

                // Build circuit matrices
                var segmentMatrices = new List<ComplexMatrix>();
                int dim = 1 << numQubits;

                foreach (var gate in circuitGates)
                {
                    ComplexMatrix gateMatrix = null;

                    switch (gate.GateType)
                    {
                        case "Hadamard (H)":
                            gateMatrix = GeneralGates.EmbedSingleQubitGate(numQubits, gate.TargetQubit, GeneralGates.H);
                            break;
                        case "Pauli X":
                            gateMatrix = GeneralGates.EmbedSingleQubitGate(numQubits, gate.TargetQubit, GeneralGates.X);
                            break;
                        case "Pauli Y":
                            gateMatrix = GeneralGates.EmbedSingleQubitGate(numQubits, gate.TargetQubit, GeneralGates.Y);
                            break;
                        case "Pauli Z":
                            gateMatrix = GeneralGates.EmbedSingleQubitGate(numQubits, gate.TargetQubit, GeneralGates.Z);
                            break;
                        case "Rx (Rotation X)":
                            gateMatrix = GeneralGates.EmbedSingleQubitGate(numQubits, gate.TargetQubit, GeneralGates.Rx(gate.Angle));
                            break;
                        case "Ry (Rotation Y)":
                            gateMatrix = GeneralGates.EmbedSingleQubitGate(numQubits, gate.TargetQubit, GeneralGates.Ry(gate.Angle));
                            break;
                        case "Rz (Rotation Z)":
                            gateMatrix = GeneralGates.EmbedSingleQubitGate(numQubits, gate.TargetQubit, GeneralGates.Rz(gate.Angle));
                            break;
                        case "CNOT":
                            gateMatrix = GeneralGates.ControlledGate(numQubits, gate.ControlQubit, gate.TargetQubit, GeneralGates.X);
                            break;
                    }

                    if (gateMatrix != null)
                    {
                        segmentMatrices.Add(gateMatrix);
                    }
                }

                // Apply circuit
                simulator.ApplyCircuit(segmentMatrices);

                // Visualize
                DrawBlochSpheres(simulator);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Simulation error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DrawBlochSpheres(BlochSphereSimulator simulator)
        {
            canvasBlochSpheres.Children.Clear();

            int numQubits = simulator.NumberOfQubits;
            double sphereRadius = 100;
            double spacing = 250;
            double startY = 50;

            for (int i = 0; i < numQubits; i++)
            {
                double yOffset = startY + (i * spacing * 2);

                // Draw initial state
                DrawSingleBlochSphere(50, yOffset, sphereRadius, simulator.InitialBlochStates[i],
                    $"Qubit {i} - Initial State", Colors.LightBlue);

                // Draw final state
                DrawSingleBlochSphere(450, yOffset, sphereRadius, simulator.FinalBlochStates[i],
                    $"Qubit {i} - Final State", Colors.LightGreen);
            }
        }

        private void DrawSingleBlochSphere(double centerX, double centerY, double radius,
            BlochCoordinate coord, string label, Color stateColor)
        {
            // Title
            var title = new TextBlock
            {
                Text = label,
                Foreground = Brushes.White,
                FontSize = 16,
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(title, centerX - 50);
            Canvas.SetTop(title, centerY - radius - 30);
            canvasBlochSpheres.Children.Add(title);

            // Draw sphere outline (circle in 2D projection)
            var sphere = new Ellipse
            {
                Width = radius * 2,
                Height = radius * 2,
                Stroke = Brushes.Gray,
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Color.FromArgb(20, 100, 100, 100))
            };
            Canvas.SetLeft(sphere, centerX - radius);
            Canvas.SetTop(sphere, centerY - radius);
            canvasBlochSpheres.Children.Add(sphere);

            // Draw axes
            DrawAxis(centerX, centerY, radius, 0, Colors.Red, "X");      // X-axis (right)
            DrawAxis(centerX, centerY, radius, 180, Colors.Red, "-X");   // -X-axis (left)
            DrawAxis(centerX, centerY, radius, 90, Colors.Green, "Y");   // Y-axis (up)
            DrawAxis(centerX, centerY, radius, 270, Colors.Green, "-Y"); // -Y-axis (down)

            // Z-axis (vertical line)
            var zAxis = new Line
            {
                X1 = centerX,
                Y1 = centerY - radius,
                X2 = centerX,
                Y2 = centerY + radius,
                Stroke = new SolidColorBrush(Colors.Blue),
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection { 4, 2 }
            };
            canvasBlochSpheres.Children.Add(zAxis);

            // Z labels
            AddLabel(centerX, centerY - radius - 15, "Z", Colors.Blue);
            AddLabel(centerX, centerY + radius + 5, "-Z", Colors.Blue);

            // Draw state vector
            double x = coord.X * radius;
            double y = -coord.Y * radius; // Negative because canvas Y increases downward
            double z = coord.Z * radius;

            // Project 3D to 2D (simple orthographic projection)
            double projX = centerX + x;
            double projY = centerY + y + z * 0.5; // Simple projection mixing y and z

            // State vector line
            var stateLine = new Line
            {
                X1 = centerX,
                Y1 = centerY,
                X2 = projX,
                Y2 = projY,
                Stroke = new SolidColorBrush(stateColor),
                StrokeThickness = 3
            };
            canvasBlochSpheres.Children.Add(stateLine);

            // State point
            var statePoint = new Ellipse
            {
                Width = 12,
                Height = 12,
                Fill = new SolidColorBrush(stateColor),
                Stroke = Brushes.White,
                StrokeThickness = 2
            };
            Canvas.SetLeft(statePoint, projX - 6);
            Canvas.SetTop(statePoint, projY - 6);
            canvasBlochSpheres.Children.Add(statePoint);

            // Coordinate info
            var coordText = new TextBlock
            {
                Text = $"x={coord.X:F3}, y={coord.Y:F3}, z={coord.Z:F3}\n" +
                       $"θ={coord.Theta:F3}, φ={coord.Phi:F3}, r={coord.R:F3}",
                Foreground = Brushes.LightGray,
                FontSize = 11
            };
            Canvas.SetLeft(coordText, centerX - radius);
            Canvas.SetTop(coordText, centerY + radius + 15);
            canvasBlochSpheres.Children.Add(coordText);
        }

        private void DrawAxis(double centerX, double centerY, double radius, double angleDeg, Color color, string label)
        {
            double angleRad = angleDeg * Math.PI / 180.0;
            double endX = centerX + radius * Math.Cos(angleRad);
            double endY = centerY + radius * Math.Sin(angleRad);

            var line = new Line
            {
                X1 = centerX,
                Y1 = centerY,
                X2 = endX,
                Y2 = endY,
                Stroke = new SolidColorBrush(color),
                StrokeThickness = 1.5,
                StrokeDashArray = new DoubleCollection { 4, 2 }
            };
            canvasBlochSpheres.Children.Add(line);

            // Label
            double labelX = centerX + (radius + 15) * Math.Cos(angleRad);
            double labelY = centerY + (radius + 15) * Math.Sin(angleRad);
            AddLabel(labelX, labelY, label, color);
        }

        private void AddLabel(double x, double y, string text, Color color)
        {
            var label = new TextBlock
            {
                Text = text,
                Foreground = new SolidColorBrush(color),
                FontWeight = FontWeights.Bold,
                FontSize = 12
            };
            Canvas.SetLeft(label, x - 10);
            Canvas.SetTop(label, y - 10);
            canvasBlochSpheres.Children.Add(label);
        }
    }

    public class GateOperation
    {
        public string GateType { get; set; } = "";
        public int TargetQubit { get; set; }
        public int ControlQubit { get; set; } = -1;
        public double Angle { get; set; }
    }
}
