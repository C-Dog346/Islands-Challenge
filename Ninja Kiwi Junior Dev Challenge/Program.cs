// See https://aka.ms/new-console-template for more information
using challenge;
using System.Drawing;

Console.WriteLine("Hello, World!");

//Testing object
Islands test = new Islands("../../../Images/islands.png");
Console.WriteLine(test.CountBlack());

Console.WriteLine("1");




namespace challenge
{
    public class Islands
    {
       public Bitmap image;

       public Islands(string file) 
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
                    if (image.GetPixel(x, y).Name == "ff000000")
                    {
                        count++;
                    }       
                }
            }

            return count;
        }
    }  
}




