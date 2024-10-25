using System;
using System.Collections.Generic;

public class AStar
{
    // Düğüm sınıfı, her bir hücreyi temsil eder.
    public class Node
    {
        public int x, y; // Hücrenin koordinatları
        public double f, g, h; // A*, f = g + h
        public Node parent; // Ebeveyn düğüm

        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    // A* algoritması
    public static List<Node> FindPath(Node start, Node goal, Func<Node, Node, double> heuristic)
    {
        List<Node> openSet = new List<Node>(); // Açık küme
        HashSet<Node> closedSet = new HashSet<Node>(); // Kapalı küme

        openSet.Add(start); // Başlangıç düğümünü açık kümeye ekle

        while (openSet.Count > 0)
        {
            // Açık kümedeki en düşük f değerine sahip düğümü al
            Node current = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].f < current.f || (openSet[i].f == current.f && openSet[i].h < current.h))
                {
                    current = openSet[i];
                }
            }

            // Hedefe ulaşıldıysa yol bulundu
            if (current == goal)
            {
                List<Node> path = new List<Node>();
                while (current != null)
                {
                    path.Add(current);
                    current = current.parent;
                }
                path.Reverse();
                return path;
            }

            openSet.Remove(current);
            closedSet.Add(current);

            // Komşu düğümleri kontrol et
            foreach (Node neighbor in GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                double tentative_gScore = current.g + 1; // G değeri, başlangıç düğümünden geçen yol uzunluğu

                if (!openSet.Contains(neighbor) || tentative_gScore < neighbor.g)
                {
                    neighbor.parent = current;
                    neighbor.g = tentative_gScore;
                    neighbor.h = heuristic(neighbor, goal); // H değeri, seçilen heuristiğe göre tahmin edilen uzaklık
                    neighbor.f = neighbor.g + neighbor.h;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        // Hedefe ulaşılamadı
        return null;
    }

    // Düğümün komşularını döndüren yardımcı bir fonksiyon
    private static List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>
        {
            // Örnek olarak 4 yönlü hareketi destekleyen bir örnek
            // Farklı hareket tipleri veya engel kontrolü burada yapılabilir
            new Node(node.x - 1, node.y),
            new Node(node.x + 1, node.y),
            new Node(node.x, node.y - 1),
            new Node(node.x, node.y + 1)
        };
        return neighbors;
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Başlangıç ve hedef düğümleri oluştur
        AStar.Node start = new AStar.Node(0, 0);
        AStar.Node goal = new AStar.Node(4, 4);

        // Manhattan mesafesi heuristiği kullanarak A* algoritmasını çalıştır
        List<AStar.Node> path = AStar.FindPath(start, goal, (a, b) => Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y));

        // Yolu ekrana yazdır
        if (path != null)
        {
            foreach (var node in path)
            {
                Console.WriteLine("(" + node.x + "," + node.y + ")");
            }
        }
        else
        {
            Console.WriteLine("Yol bulunamadı.");
        }
    }
}