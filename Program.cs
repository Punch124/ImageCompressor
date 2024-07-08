using System.Drawing;


namespace ImageCompressor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string relativeImagePath = Path.Combine("Images", "GRUtcAdWMAAnYF7.jpeg");
            string inputImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativeImagePath);

            string outputCompressedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "compressed_image.dat");
            Bitmap bitmap = new Bitmap(inputImagePath);
            int width = bitmap.Width;
            int height = bitmap.Height;

            byte[][] bitPlanes = new byte[8][];
            for (int i = 0; i < 8; i++)
                bitPlanes[i] = new byte[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    byte pixelValue = pixel.R;
                    for (int bit = 0; bit < 8; bit++)
                    {
                        bitPlanes[bit][y * width + x] = (byte)((pixelValue >> bit) & 1);
                    }
                }
            }
            using (BinaryWriter writer = new BinaryWriter(File.Open(outputCompressedPath, FileMode.Create))) {
                writer.Write(width);
                writer.Write(height);

                for (int bit = 0; bit < 8; bit++)
                {
                    byte[] compresedPlane = CompressRLE(bitPlanes[bit]);
                    writer.Write(compresedPlane.Length);
                    writer.Write(compresedPlane);
                }
            }
            Console.WriteLine("Compression completed!");
        }   
        static byte[] CompressRLE(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    int length = data.Length;
                    for (int i = 0; i < length; i++)
                    {
                        byte value = data[i];
                        int runLength = 1;
                        while (i +1 < length && data[i + 1] == value) 
                        {
                            runLength++;
                            i++;
                        }

                        writer.Write((byte)runLength);
                        writer.Write(value);
                    }
                }
               
                    return ms.ToArray();
            }
        }
    }
}
