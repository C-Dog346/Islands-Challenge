// See https://aka.ms/new-console-template for more information
using System.Drawing;
public class Challenge
{
    public Bitmap image;
    private readonly string black = "ff000000";
    private double maxBound;

    public Challenge(string file)
    {
        image = ConvertToBitmap(file);
        maxBound = image.Width * image.Height;
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

        // 2D Array used for storing visited pixel coordinates
        bool[,] visited = new bool[image.Width, image.Height];

        // Builds a list of islands 
        for (int x = 0; x < image.Width; x++)
        {
            for (int y = 0; y < image.Height; y++)
            {
                // Pixel that is black and not ivisted yet
                if (image.GetPixel(x, y).Name == black && !visited[x, y])
                {
                    // DFS Solution (Stack overflow error limited - pretty big issue for larger images)
                    //List<Point> island = new List<Point>();
                    //DepthFirstSearch(image, x, y, visited, island);

                    // BFS Solution (Memory limited - barely an issue)
                    List<Point> island = BFS(image, x, y, visited);
                    
                    islands.Add(island);
                }
            }
        }

        return islands;
    }

    private void DepthFirstSearch(Bitmap image, int x, int y, bool[,] visited, List<Point> island)
    {
        // If out of bounds
        if (x < 0 || y < 0 || x >= image.Width || y >= image.Height)
            return;

        // If already visited or white
        if (image.GetPixel(x, y).Name != black || visited[x, y])
            return;

        visited[x, y] = true;
        island.Add(new Point(x, y));

        // Check right, left, up and down for more valid pixels
        DepthFirstSearch(image, x + 1, y, visited, island);
        DepthFirstSearch(image, x - 1, y, visited, island);
        DepthFirstSearch(image, x, y + 1, visited, island);
        DepthFirstSearch(image, x, y - 1, visited, island);
    }

    private List<Point> BFS(Bitmap image, int startX, int startY, bool[,] visited)
    {
        List<Point> island = new List<Point>();
        Queue<Point> queue = new Queue<Point>();
        int[] xPositions = { 1, -1, 0, 0 };
        int[] yPositions = { 0, 0, 1, -1 };

        // Queue starting point
        visited[startX, startY] = true;
        queue.Enqueue(new Point(startX, startY));

        while (queue.Count > 0)
        {
            Point current = queue.Dequeue();
            island.Add(current);

            for (int i = 0; i < 4; i++)
            {
                int newX = current.X + xPositions[i];
                int newY = current.Y + yPositions[i];

                if (newX >= 0 && newX < image.Width && newY >= 0 && newY < image.Height &&
                    image.GetPixel(newX, newY).Name == black &&
                    !visited[newX, newY])
                {
                    visited[newX, newY] = true;
                    queue.Enqueue(new Point(newX, newY));
                }
            }
        }

        return island;
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

        // Average the all the pixel coordinates to find the average coordinate AKA the center
        double centerX = sumX / points.Count;
        double centerY = sumY / points.Count;

        // Math.Round for more accurate rounding to integers (will round up when appropriate instead of always down)
        return new Point((int)Math.Round(centerX), (int)Math.Round(centerY));
    }

    public List<Point> FindMostCentralIsland(List<List<Point>> islands)
    {
        double minAvgDistance = maxBound;
        List<Point> mostCentralIsland = null;

        // Edge case handling
        if (islands.Count == 1)
            return islands[0];

        // Go through every island and calculate distance to other islands
        for (int i = 0; i < islands.Count; i++)
        {
            // Total distance to all islands for a given island
            double totalDistance = 0;
            for (int j = 0; j < islands.Count; j++)
            {
                if (i != j)
                {
                    double distance = CalculateDistance(islands[i], islands[j]);
                    totalDistance += distance;
                }
            }

            // Total distance averaged 
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
        double minDistance = maxBound;

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

    public void MarkIslands(List<List<Point>> islands, Point centralIsland)
    {
        for (int i = 0; i < islands.Count; i++)
        {
            Point center = FindCenter(islands[i]);
            if (center.X == centralIsland.X && center.Y == centralIsland.Y)
                image.SetPixel(center.X, center.Y, Color.Red);
            else
                image.SetPixel(center.X, center.Y, Color.Yellow);
        }
    }

    public void SaveImage(string fileName)
    {
        image.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
    }

    static void Main(string[] args)
    {
        Console.WriteLine("Please select which image you would like to process:");
        string[] imageList = Directory.GetFiles("../../../Images");
        for (int i = 1; i < imageList.Length; i++)
        {
            Console.WriteLine($"{i}. {imageList[i].Replace("../../../Images\\", "")}");
        }

        Console.WriteLine($"");
        // Testing object
        Challenge test = new Challenge("../../../Images/islands.png");
        //Console.WriteLine(test.CountBlack());

        List<List<Point>> islands = test.FindIslandCenters();

        Console.WriteLine("Number of islands: " + islands.Count);

        if (islands.Count > 0)
        {
            // All island centers
            for (int i = 0; i < islands.Count; i++)
            {
                Point center = test.FindCenter(islands[i]);
                Console.WriteLine($"Centermost point of Island {i + 1}: ({center.X}, {center.Y})");
            }

            // Find Center island's center point
            Point centralIsland = test.FindCenter(test.FindMostCentralIsland(islands));
            Console.WriteLine($"Centermost Island: ({centralIsland.X}, {centralIsland.Y})");

            // Print smaller towers, skipping over the main tower
            for (int i = 0; i < islands.Count; i++)
            {
                Point center = test.FindCenter(islands[i]);
                if (!(center.X == centralIsland.X && center.Y == centralIsland.Y))
                    Console.WriteLine($"Smaller Communication towers point of Island {i + 1}: ({center.X}, {center.Y})");
            }

            Console.WriteLine($"Main Communication Tower: ({centralIsland.X}, {centralIsland.Y})");

            // Save image with marked towers
            test.MarkIslands(test.FindIslandCenters(), centralIsland);
            // CHANGE NAME OF IMAGE TO SELECT YOUR IMAGE
            test.SaveImage("../../../Images/islands.png");

            Console.WriteLine("Saving Image with Communication Towers marked as \"Marked Islands\"");
            Console.WriteLine("Small Communication Towers marked in YELLOW");
            Console.WriteLine("Main Communication Tower marked in RED");

            Console.WriteLine("End");
        }
        else
        {
            Console.WriteLine("No islands in image. Challenge not possible.");
        }
    }
}





