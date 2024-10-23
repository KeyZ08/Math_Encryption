using System.Drawing;
using System.Drawing.Imaging;
using QRCodeGenerator = QRCode.QR.QRCodeGenerator;

namespace QRCode;

internal static class Program
{
    private const string PlanedText = "Я пришел тестировать этот длинный текст в моем QR коде 10-й версии!";

    private static void Main(string[] args)
    {
        var data = QRCodeGenerator.GenerateQrCode(PlanedText);
        var matrix = data.ModuleMatrix;
        var size = data.ModuleMatrix.Count;

        var bitmap = new Bitmap(size, size);
        Fill(bitmap, Color.White);
        for (var y = 0; y < size; y++)
        for (var x = 0; x < size; x++)
            if (matrix[x][y])
                bitmap.SetPixel(x, y, Color.Black);
        bitmap.Save("../../../test4.png", ImageFormat.Png);
    }

    private static void Fill(Bitmap image, Color color)
    {
        for (var h = 0; h < image.Height; h++)
        for (var w = 0; w < image.Width; w++)
            image.SetPixel(w, h, color);
    }
}