using System.Drawing;

namespace Barcode;

public static class Printer
{
    // ReSharper disable once InconsistentNaming
    public static Bitmap PrintBarcodeEAN13(IList<int> input)
    {
        const int barcodeHeight = DigitBlocks.DigitHeight * 10;
        const int barcodeWidth = DigitBlocks.DigitWidth + 1 + 3 + EAN13Codes.BitsCountPerDigit * 12 + 5 + 3;
        const int boundsWidth = 10;

        const int standardLineHeight = barcodeHeight - (DigitBlocks.DigitHeight + 3);

        var bitmap = new Bitmap(barcodeWidth + boundsWidth * 2, barcodeHeight + boundsWidth * 2);
        Fill(bitmap, Color.White);
        var digitPosY = bitmap.Height - boundsWidth - DigitBlocks.DigitHeight;

        var offsetX = boundsWidth;
        var digitIndex = 0;

        PrintDigit(input[digitIndex++], new Point(offsetX, digitPosY));
        offsetX += 1 + DigitBlocks.DigitWidth;

        PrintCommonLines(new Point(offsetX, boundsWidth));
        offsetX += 3;

        PrintLeftHalf(new Point(offsetX, boundsWidth));
        offsetX += 6 * EAN13Codes.BitsCountPerDigit + 1;

        PrintCommonLines(new Point(offsetX, boundsWidth));
        offsetX += 3;

        PrintRightHalf(new Point(offsetX, boundsWidth));
        offsetX += 6 * EAN13Codes.BitsCountPerDigit;

        PrintCommonLines(new Point(offsetX, boundsWidth));
        return bitmap;


        void PrintDigit(int digit, Point position)
        {
            PrintBlock(DigitBlocks.Digits[digit], position, bitmap);
        }

        void PrintCommonLines(Point position)
        {
            PrintVerticalLine(position, barcodeHeight, bitmap);
            PrintVerticalLine(position + new Size(2, 0), barcodeHeight, bitmap);
        }

        void PrintLeftHalf(Point position)
        {
            var structureCode = EAN13Codes.StructCode[input[0]];
            var totalOffsetX = position.X;
            for (var i = 1; i < 7; i++)
            {
                var digit = input[digitIndex++];
                var bits = structureCode[i - 1] ? EAN13Codes.LCodes[digit] : EAN13Codes.GCodes[digit];
                PrintDigitWithLines(digit, bits, position with { X = totalOffsetX });
                totalOffsetX += EAN13Codes.BitsCountPerDigit;
            }
        }

        void PrintDigitWithLines(int digit, bool[] bits, Point position)
        {
            PrintStandardLines(bits, position);
            PrintDigit(digit, new Point(position.X + 1, digitPosY));
        }

        void PrintStandardLines(bool[] bits, Point position)
        {
            var totalOffsetX = position.X;
            foreach (var bit in bits)
                if (bit)
                    PrintVerticalLine(position with { X = totalOffsetX++ }, standardLineHeight, bitmap);
                else
                    totalOffsetX++;
        }

        void PrintRightHalf(Point position)
        {
            var totalOffsetX = position.X;
            for (var i = 7; i < 13; i++)
            {
                var digit = input[digitIndex++];
                var bits = EAN13Codes.RCodes[digit];
                PrintDigitWithLines(digit, bits, position with { X = totalOffsetX });
                totalOffsetX += EAN13Codes.BitsCountPerDigit;
            }
        }
    }

    private static void Fill(Bitmap image, Color color)
    {
        for (var h = 0; h < image.Height; h++)
        for (var w = 0; w < image.Width; w++)
            image.SetPixel(w, h, color);
    }

    private static void PrintBlock(bool[][] digit, Point position, Bitmap bitmap)
    {
        for (var h = 0; h < digit.Length; h++)
        for (var w = 0; w < digit[0].Length; w++)
            if (digit[h][w])
                bitmap.SetPixel(w + position.X, position.Y + h, Color.Black);
    }

    private static void PrintVerticalLine(Point startPosition, int height, Bitmap bitmap)
    {
        for (var h = 0; h < height; h++)
            bitmap.SetPixel(startPosition.X, startPosition.Y + h, Color.Black);
    }
}