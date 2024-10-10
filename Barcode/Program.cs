using System.Drawing;
using System.Drawing.Imaging;
using Barcode;

const int barcodeHeight = DigitBlocks.DigitHeight * 10;
const int barcodeWidth = DigitBlocks.DigitWidth + 1 + 3 + EAN13Codes.BitsCountPerDigit * 12 + 5 + 3;
const int boundsWidth = 10;

var bitmap = new Bitmap(barcodeWidth + boundsWidth * 2, barcodeHeight + boundsWidth * 2);
for (var h = 0; h < bitmap.Height; h++)
for (var w = 0; w < bitmap.Width; w++)
    bitmap.SetPixel(w, h, Color.White);

var offsetX = 0;
for (var i = 0; i < 10; i++)
{
    Printer.PrintBlock(DigitBlocks.Digits[i], new Point(boundsWidth + offsetX, boundsWidth), bitmap);
    offsetX += DigitBlocks.DigitWidth + 1;
}

bitmap.Save("../../../Barcodes/test.png", ImageFormat.Png);