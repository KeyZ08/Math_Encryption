using System.Drawing;
using System.Drawing.Imaging;
using Barcode;

const int barcodeHeight = DigitBlocks.DigitHeight * 10;
const int barcodeWidth = DigitBlocks.DigitWidth + 1 + 3 + EAN13Codes.BitsCountPerDigit * 12 + 5 + 3;
const int boundsWidth = 10;

const int commonLineHeight = barcodeHeight;
const int standardLineHeight = barcodeHeight - (DigitBlocks.DigitHeight + 3);
//"460700952001"
//"871125300120"
//"590123412345"
var input = "460700952001".Select(x => int.Parse(x.ToString())).ToArray();

var bitmap = new Bitmap(barcodeWidth + boundsWidth * 2, barcodeHeight + boundsWidth * 2);
Printer.Fill(bitmap, Color.White);
var digitY = bitmap.Height - boundsWidth - DigitBlocks.DigitHeight;

var offsetX = boundsWidth;
var digitIndex = 0;

PrintDigit(input[digitIndex++], new Point(offsetX, digitY));
offsetX += 1 + DigitBlocks.DigitWidth;
PrintCommonLines(new Point(offsetX, boundsWidth));
offsetX += 3;

PrintLeftHalf(new Point(offsetX, boundsWidth));

offsetX += 6 * EAN13Codes.BitsCountPerDigit + 1;
PrintCommonLines(new Point(offsetX, boundsWidth));
offsetX += 3;

for (var i = 7; i < 12; i++)
{
    var digit = input[digitIndex++];
    var digitPosX = offsetX + 1;
    var bits = EAN13Codes.RCodes[digit];
    
    PrintStandardLines(bits, new Point(offsetX, boundsWidth));
    offsetX += bits.Length;
    PrintDigit(digit, new Point(digitPosX, digitY));
}

var checkSum = EAN13Codes.GetCheckSum(input);
PrintStandardLines(EAN13Codes.RCodes[checkSum], new Point(offsetX, boundsWidth));
PrintDigit(checkSum, new Point(offsetX + 1, digitY));

offsetX += EAN13Codes.BitsCountPerDigit;
PrintCommonLines(new Point(offsetX, boundsWidth));

bitmap.Save("../../../Barcodes/test.png", ImageFormat.Png);


void PrintCommonLines(Point position)
{
    Printer.PrintVerticalLine(position, commonLineHeight, bitmap);
    Printer.PrintVerticalLine(position + new Size(2, 0), commonLineHeight, bitmap);
}

void PrintDigit(int digit, Point position)
{
    Printer.PrintBlock(DigitBlocks.Digits[digit], position, bitmap);
}

void PrintLeftHalf(Point position)
{
    var structureCode = EAN13Codes.StructCode[input[0]];
    var localOffsetX = position.X; 
    for (var i = 1; i < 7; i++)
    {
        var digit = input[digitIndex++];
        var digitPosX = localOffsetX + 1;
        var bits = structureCode[i - 1] ? EAN13Codes.LCodes[digit] : EAN13Codes.GCodes[digit];

        PrintStandardLines(bits, position with { X = localOffsetX});
        localOffsetX += bits.Length;
        PrintDigit(digit, new Point(digitPosX, digitY));
    }
}

void PrintStandardLines(bool[] bits, Point position)
{
    var localOffsetX = position.X;
    foreach (var bit in bits)
        if (bit)
            Printer.PrintVerticalLine(position with { X = localOffsetX++ }, standardLineHeight, bitmap);
        else
            localOffsetX++;
}