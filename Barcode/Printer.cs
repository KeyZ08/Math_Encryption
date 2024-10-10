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
}