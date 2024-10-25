using System;
using System.Collections.Generic;
using System.Linq;

public class Node
{
    public int X { get; set; }
    public int Y { get; set; }
    public float G { get; set; }
    public float H { get; set; }
    public float F { get; set; }
    public Node Parent { get; set; }

    public Node(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public class AStar
{
    private int _gridWidth;
    private int _gridHeight;

    public List<Node> OpenList { get; set; }
    public List<Node> ClosedList { get; set; }

    public AStar(int gridWidth, int gridHeight)
    {
        if (gridWidth <= 0 || gridHeight <= 0)
        {
            throw new ArgumentException("Grid dimensions must be positive integers.");
        }

        _gridWidth = gridWidth;
        _gridHeight = gridHeight;
        OpenList = new List<Node>();
        ClosedList = new List<Node>();
    }

    public List<Node> FindPath(Node start, Node end)
    {
        Console.WriteLine("A* Arama Başladı...");

        OpenList.Add(start);

        while (OpenList.Count > 0)
        {
            Node current = OpenList.OrderBy(n => n.F).First();
            ClosedList.Add(current);
            Console.WriteLine("Kapalı Listeye Eklendi: ({0}, {1})", current.X, current.Y);

            // Hedef düğüme ulaşıldığında kontrol edilir
            if (current == end)
            {
                Console.WriteLine("Hedefe Ulaşıldı!");
                return ReconstructPath(current);
            }

            OpenList.Remove(current);

            foreach (Node neighbor in GetNeighbors(current))
            {
                if (IsWithinGrid(neighbor.X, neighbor.Y)) // Check if neighbor is within grid bounds
                {
                    float g = current.G + GetCost(current, neighbor);
                    float h = GetHeuristic(neighbor, end); // Consider adjusting your heuristic function
                    float f = g + h;

                    if (!OpenList.Contains(neighbor) || g < neighbor.G) // Tie-breaking rule
                    {
                        neighbor.G = g;
                        neighbor.H = h;
                        neighbor.F = f;
                        neighbor.Parent = current;

                        if (!OpenList.Contains(neighbor))
                        {
                            OpenList.Add(neighbor);
                            Console.WriteLine("Açık Listeye Eklendi: ({0}, {1})", neighbor.X, neighbor.Y);
                        }
                    }
                }
            }
        }

        Console.WriteLine("A* Arama Tamamlandı. Yol Bulunamadı.");
        return null;
    }

    private bool IsWithinGrid(int x, int y)
    {
        return x >= 0 && x < _gridWidth && y >= 0 && y < _gridHeight;
    }

    private List<Node> ReconstructPath(Node end)
    {
        Console.WriteLine("Yol Oluşturuluyor...");

        List<Node> path = new List<Node>();

        Node current = end;
        while (current != null)
        {
            path.Add(current);
            current = current.Parent;
        }

        path.Reverse();
        Console.WriteLine("Yol Oluşturuldu!");
        return path;
    }

    private float GetCost(Node start, Node end)
    {
        // Adjust this function based on your specific movement cost calculation
        // (e.g., diagonals might cost more than straight movements)
        return Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y);
    }

    private float GetHeuristic(Node start, Node end)
    {
        // Implement your preferred heuristic function here
        // (e.g., Manhattan Distance, Euclidean Distance, etc.)
        return Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y);
    }

    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        if (node.Y > 0)
        {
            Node upNeighbor = new Node(node.X, node.Y - 1);
            neighbors.Add(upNeighbor);
        }

        // Aşağı komşu
        if (node.Y < _gridHeight - 1)
        {
            Node downNeighbor = new Node(node.X, node.Y + 1);
            neighbors.Add(downNeighbor);
        }

        // Sol komşu
        if (node.X > 0)
        {
            Node leftNeighbor = new Node(node.X - 1, node.Y);
            neighbors.Add(leftNeighbor);
        }

        // Sağ komşu
        if (node.X < _gridWidth - 1)
        {
            Node rightNeighbor = new Node(node.X + 1, node.Y);
            neighbors.Add(rightNeighbor);
        }

        return neighbors;
    }
}

class Program
{
    static void Main(string[] args)
    {
        int gridWidth = 2;
        int gridHeight = 2;

        Node start = new Node(0, 0);
        Node end = new Node(1, 1); // Corrected end node coordinates

        try
        {
            AStar aStar = new AStar(gridWidth, gridHeight);
            List<Node> path = aStar.FindPath(start, end);

            if (path != null)
            {
                Console.WriteLine("Yol bulundu!");
                foreach (Node node in path)
                {
                    Console.WriteLine("({0}, {1})", node.X, node.Y);
                }
            }
            else
            {
                Console.WriteLine("Yol bulunamadı!");
            }
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine("Hata: {0}", ex.Message);
        }
    }
}

