// See https://aka.ms/new-console-template for more information
using System.Drawing;


public class Challenge
{
    public Bitmap image;
    private readonly string black = "ff000000";

    public Challenge(string file)
    {
        image = ConvertToBitmap(file);
    }

    private Bitmap ConvertToBitmap(string file)
    {
        Bitmap bitmap;
        using (Stream bmStream = System.IO.File.Open(file, System.IO.FileMode.Open))
        {
            Image image = Image.FromStream(bmStream);

            bitmap = new Bitmap(image);
        }
        return bitmap;
    }

    // Used in debugging
    public int CountBlack()
    {
        int x, y;
        int count = 0;

        // Loop through the images pixels to reset color.
        for (x = 0; x < image.Width; x++)
        {
            for (y = 0; y < image.Height; y++)
            {
                Color color = image.GetPixel(x, y);
                if (image.GetPixel(x, y).Name == black)
                {
                    count++;
                }
            }
        }

        return count;
    }

    public List<List<Point>> FindIslandCenters()
    {
        List<List<Point>> islands = new List<List<Point>>();

        bool[,] visited = new bool[image.Width, image.Height];

        for (int x = 0; x < image.Width; x++)
        {
            for (int y = 0; y < image.Height; y++)
            {
                if (image.GetPixel(x, y).Name == black && !visited[x, y])
                {
                    List<Point> island = new List<Point>();
                    DepthFirstSearch(image, x, y, visited, island);
                    islands.Add(island);
                }
            }
        }

        return islands;
    }

    private void DepthFirstSearch(Bitmap image, int x, int y, bool[,] visited, List<Point> island)
    {
        if (x < 0 || y < 0 || x >= image.Width || y >= image.Height)
            return;

        if (image.GetPixel(x, y).Name != black || visited[x, y])
            return;

        visited[x, y] = true;
        island.Add(new Point(x, y));

        DepthFirstSearch(image, x + 1, y, visited, island);
        DepthFirstSearch(image, x - 1, y, visited, island);
        DepthFirstSearch(image, x, y + 1, visited, island);
        DepthFirstSearch(image, x, y - 1, visited, island);
    }

    public Point FindCenter(List<Point> points)
    {
        double sumX = 0;
        double sumY = 0;

        foreach (Point point in points)
        {
            sumX += point.X;
            sumY += point.Y;
        }

        double centerX = sumX / points.Count;
        double centerY = sumY / points.Count;

        return new Point((int)Math.Round(centerX), (int)Math.Round(centerY));
    }

    public List<Point> FindMostCentralIsland(List<List<Point>> islands)
    {
        double minAvgDistance = image.Width + image.Height;
        List<Point> mostCentralIsland = null;

        for (int i = 0; i < islands.Count; i++)
        {
            double totalDistance = 0;
            for (int j = 0; j < islands.Count; j++)
            {
                if (i != j)
                {
                    double distance = CalculateDistance(islands[i], islands[j]);
                    totalDistance += distance;
                }
            }

            double avgDistance = totalDistance / (islands.Count - 1);
            if (avgDistance < minAvgDistance)
            {
                minAvgDistance = avgDistance;
                mostCentralIsland = islands[i];
            }
        }

        return mostCentralIsland;
    }

    private double CalculateDistance(List<Point> island1, List<Point> island2)
    {
        double minDistance = double.MaxValue;

        foreach (Point p1 in island1)
        {
            foreach (Point p2 in island2)
            {
                double distance = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
                if (distance < minDistance)
                    minDistance = distance;
            }
        }

        return minDistance;
    }

    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        //Testing object
        Challenge test = new Challenge("../../../Images/islands.png");
        //Console.WriteLine(test.CountBlack());

        List<List<Point>> islands = test.FindIslandCenters();

        Console.WriteLine("Number of islands: " + islands.Count);
       
        for (int i = 0; i < islands.Count; i++)
        {
            Point center = test.FindCenter(islands[i]);
            Console.WriteLine($"Centremost point of Island {i + 1}: ({center.X}, {center.Y})");
        }



        Console.WriteLine("End");
    }
}





