using System.Drawing;

namespace Barcode;

public static class Printer
{
    public static void PrintBlock(bool[][] digit, Point position, Bitmap bitmap)
    {
        for (var h = 0; h < digit.Length; h++)
        for (var w = 0; w < digit[0].Length; w++)
            if (digit[h][w])
                bitmap.SetPixel(w + position.X, position.Y + h, Color.Black);
    }

    public static void PrintVerticalLine(Point startPosition, int height, Bitmap bitmap)
    {
        for (var h = 0; h < height; h++)
            bitmap.SetPixel(startPosition.X, startPosition.Y + h, Color.Black);
    }

    public static void Fill(Bitmap image, Color color)
    {
        for (var h = 0; h < image.Height; h++)
        for (var w = 0; w < image.Width; w++)
            image.SetPixel(w, h, color);
    }
}