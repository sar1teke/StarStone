using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace A_Star_Sekiz_Tas
{
    public partial class Form1 : Form
    {
        private Panel panelMatris1;
        private Panel panelMatris2;
        private NumericUpDown numericUpDownBoyut;
        private RichTextBox richTextBox1;

        public Form1()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            panelMatris1 = panel1;
            panelMatris1.AutoScroll = true;

            panelMatris2 = panel2;
            panelMatris2.AutoScroll = true;

            numericUpDownBoyut = numericUpDown1;
            numericUpDownBoyut.Minimum = 1;
            numericUpDownBoyut.Maximum = 10;
            numericUpDownBoyut.ValueChanged += NumericUpDownBoyut_ValueChanged;

            richTextBox1 = richTextBoxSolution;

            DrawMatrices();
        }

        private void NumericUpDownBoyut_ValueChanged(object sender, EventArgs e)
        {
            DrawMatrices();
        }

        private void DrawMatrices()
        {
            int boyut = (int)numericUpDownBoyut.Value;

            DrawMatrix(panelMatris1, boyut);
            DrawMatrix(panelMatris2, boyut);
        }

        private void DrawMatrix(Panel panelMatris, int boyut)
        {
            panelMatris.Controls.Clear();
            int cellSize = Math.Min(panelMatris.Width, panelMatris.Height) / boyut;

            for (int i = 0; i < boyut; i++)
            {
                for (int j = 0; j < boyut; j++)
                {
                    TextBox txtCell = new TextBox();
                    txtCell.Size = new Size(cellSize, cellSize);
                    txtCell.Location = new Point(j * cellSize, i * cellSize);
                    txtCell.Multiline = true;
                    txtCell.BorderStyle = BorderStyle.FixedSingle;
                    txtCell.Font = new Font(txtCell.Font.FontFamily, 12f);
                    txtCell.Anchor = AnchorStyles.None;

                    panelMatris.Controls.Add(txtCell);
                }
            }
        }

        private void buttonSolve_Click(object sender, EventArgs e)
        {
            SolveAStar();
        }

        private void SolveAStar()
        {
            int[] initialState = GetMatrixValues(panelMatris1);
            int[] goalState = GetMatrixValues(panelMatris2);

            List<int[]> solution = AStar(initialState, goalState);

            if (solution.Count == 0)
            {
                MessageBox.Show("Uygun yol bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                string solutionText = "Çözüm:\n";
                foreach (var state in solution)
                {
                    solutionText += MatrixToString(state) + "\n";
                }
                richTextBox1.Clear();
                richTextBox1.AppendText(solutionText);
            }
        }

        private int[] GetMatrixValues(Panel panelMatris)
        {
            List<int> values = new List<int>();

            foreach (Control control in panelMatris.Controls)
            {
                if (control is TextBox textBox)
                {
                    int value;
                    if (int.TryParse(textBox.Text, out value))
                    {
                        values.Add(value);
                    }
                }
            }

            return values.ToArray();
        }

        private string MatrixToString(int[] state)
        {
            string result = "";
            int boyut = (int)Math.Sqrt(state.Length);
            for (int i = 0; i < state.Length; i++)
            {
                result += state[i] + " ";
                if ((i + 1) % boyut == 0)
                {
                    result += "\n";
                }
            }
            return result;
        }

        private List<int[]> AStar(int[] initialState, int[] goalState)
        {
            List<Node> openList = new List<Node>();
            HashSet<Node> closedList = new HashSet<Node>();

            Node startNode = new Node(initialState, 0, CalculateHeuristic(initialState, goalState), null);
            openList.Add(startNode);

            while (openList.Count > 0)
            {
                Node currentNode = openList.OrderBy(node => node.F).First();

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                if (IsEqual(currentNode.State, goalState))
                {
                    return ReconstructPath(currentNode);
                }

                foreach (var neighborState in GetNeighborStates(currentNode.State))
                {
                    Node neighborNode = new Node(neighborState, currentNode.G + 1, CalculateHeuristic(neighborState, goalState), currentNode);

                    if (!closedList.Any(node => IsEqual(node.State, neighborNode.State)))
                    {
                        if (!openList.Any(node => IsEqual(node.State, neighborNode.State)) || neighborNode.F < openList.First(node => IsEqual(node.State, neighborNode.State)).F)
                        {
                            openList.Add(neighborNode);
                        }
                    }
                }
            }

            return new List<int[]>();
        }

        private int CalculateHeuristic(int[] currentState, int[] goalState)
        {
            int distance = 0;
            for (int i = 0; i < currentState.Length; i++)
            {
                if (currentState[i] != 0)
                {
                    int currentRow = i / (int)Math.Sqrt(currentState.Length);
                    int currentCol = i % (int)Math.Sqrt(currentState.Length);
                    int goalIndex = Array.IndexOf(goalState, currentState[i]);
                    int goalRow = goalIndex / (int)Math.Sqrt(goalState.Length);
                    int goalCol = goalIndex % (int)Math.Sqrt(goalState.Length);
                    distance += (currentRow - goalRow) * (currentRow - goalRow) + (currentCol - goalCol) * (currentCol - goalCol);
                }
            }
            return distance;
        }

        private List<int[]> GetNeighborStates(int[] currentState)
        {
            int zeroIndex = Array.IndexOf(currentState, 0);

            List<int[]> neighborStates = new List<int[]>();

            if (zeroIndex - (int)Math.Sqrt(currentState.Length) >= 0)
            {
                int[] newState = (int[])currentState.Clone();
                newState[zeroIndex] = newState[zeroIndex - (int)Math.Sqrt(currentState.Length)];
                newState[zeroIndex - (int)Math.Sqrt(currentState.Length)] = 0;
                neighborStates.Add(newState);
            }
            if (zeroIndex + (int)Math.Sqrt(currentState.Length) < currentState.Length)
            {
                int[] newState = (int[])currentState.Clone();
                newState[zeroIndex] = newState[zeroIndex + (int)Math.Sqrt(currentState.Length)];
                newState[zeroIndex + (int)Math.Sqrt(currentState.Length)] = 0;
                neighborStates.Add(newState);
            }
            if (zeroIndex % (int)Math.Sqrt(currentState.Length) != 0)
            {
                int[] newState = (int[])currentState.Clone();
                newState[zeroIndex] = newState[zeroIndex - 1];
                newState[zeroIndex - 1] = 0;
                neighborStates.Add(newState);
            }
            if ((zeroIndex + 1) % (int)Math.Sqrt(currentState.Length) != 0)
            {
                int[] newState = (int[])currentState.Clone();
                newState[zeroIndex] = newState[zeroIndex + 1];
                newState[zeroIndex + 1] = 0;
                neighborStates.Add(newState);
            }

            return neighborStates;
        }

        private List<int[]> ReconstructPath(Node node)
        {
            List<int[]> path = new List<int[]>();
            while (node != null)
            {
                path.Insert(0, node.State);
                node = node.Parent;
            }
            return path;
        }

        private bool IsEqual(int[] state1, int[] state2)
        {
            for (int i = 0; i < state1.Length; i++)
            {
                if (state1[i] != state2[i])
                {
                    return false;
                }
            }
            return true;
        }
    }

    class Node
    {
        public int[] State { get; set; }
        public int G { get; set; }
        public int H { get; set; }
        public int F { get { return G + H; } }
        public Node Parent { get; set; }

        public Node(int[] state, int g, int h, Node parent)
        {
            State = state;
            G = g;
            H = h;
            Parent = parent;
        }
    }
}
