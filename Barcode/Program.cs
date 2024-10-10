using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

const int numsWeight = 5; //ширина цифр px
var digits = new[] //цифры попиксельно
    {
        "01110100011000110001100011000101110",
        "00100011000010000100001000010001110",
        "01110100010000100010001000100011111",
        "11111000100010000010000011000101110",
        "00010001100101010010111110001000010",
        "11111100001111000001000011000101110",
        "00110010001000011110100011000101110",
        "11111000010001000100010000100001000",
        "01110100011000101110100011000101110",
        "01110100011000101111000010001001100"
    }
    .Select(x => new BitArray(x.Select(c => c != '0').ToArray())).ToArray();

var bitmap = new Bitmap(200, 100);
for (var h = 0; h < bitmap.Height; h++)
for (var w = 0; w < bitmap.Width; w++)
    bitmap.SetPixel(w, h, Color.White);

var offsetX = 0;
foreach (var digit in digits)
{
    PrintDigit(digit, new Point(offsetX, 0));
    offsetX += numsWeight + 1;
}

bitmap.Save("../../../Barcodes/test.png", ImageFormat.Png);

void PrintDigit(BitArray digit, Point position)
{
    var printedBitsCount = 0;
    var offsetY = 0;
    while (printedBitsCount < digit.Count)
    {
        for (var i = 0; i < numsWeight; i++)
        {
            if (digit[printedBitsCount])
                bitmap.SetPixel(i + position.X, position.Y + offsetY, Color.Black);

            printedBitsCount++;
        }
        offsetY++;
    }
}