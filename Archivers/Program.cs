// ReSharper disable InconsistentNaming

namespace Archivers;

internal static class Program
{
    private const string prePathFile = "../../../Files/";
    private const string targetPathFileLZW = "../../../Files/LZW/";
    private const string targetPathFileHaffman = "../../../Files/Haffman/";

    private static async Task Main(string[] args)
    {
        // await LZW_Examples();
        await Haffman_Examples();
    }

    private static async Task Haffman_Examples()
    {
        Console.WriteLine("Text Example");
        
        await Haffman_Example("Original", "txt");
        Console.WriteLine();
        
        Console.WriteLine("Image Example");

        Console.WriteLine("Solid");
        await Haffman_Example("Solid", "png");
        Console.WriteLine("Original");
        await Haffman_Example("Original", "jpg");
        Console.WriteLine("Very High");
        await Haffman_Example("very high", "jpg");
        Console.WriteLine();
        
        Console.WriteLine("Video Example");
        
        await Haffman_Example("Original", "mp4");
        Console.WriteLine();
    }

    private static async Task LZW_Examples()
    {
        Console.WriteLine("Text Example");
        await LZW_Example("Original", "txt");
        Console.WriteLine();

        Console.WriteLine("Image Example");
        Console.WriteLine("Solid");
        await LZW_Example("Solid", "png");
        Console.WriteLine("Original");
        await LZW_Example("Original", "jpg");
        Console.WriteLine("Very High");
        await LZW_Example("very high", "jpg");
        Console.WriteLine();

        Console.WriteLine("Video Example");
        await LZW_Example("Original", "mp4");
        Console.WriteLine();
    }

    private static async Task LZW_Example(string fileName, string fileExtension)
    {
        var input = prePathFile + $"{fileName}.{fileExtension}";
        var inputCopy = targetPathFileLZW + $"{fileName}.{fileExtension}";
        Directory.CreateDirectory(targetPathFileLZW);
        await CopyFile(input, inputCopy);

        input = inputCopy;
        var compress = targetPathFileLZW + $"{fileName}_Compress.{fileExtension}";
        var decompress = targetPathFileLZW + $"{fileName}_Decompress.{fileExtension}";

        LZWApply(input, compress, LZWOperation.Compress);
        LZWApply(compress, decompress, LZWOperation.Decompress);


        await using var input1 = new FileStream(input, FileMode.Open, FileAccess.Read);
        await using var output1 = new FileStream(compress, FileMode.Open, FileAccess.Read);

        Console.WriteLine($"Коэффициент сжатия = {(float)input1.Length / output1.Length}");
        Console.WriteLine($"Сжатие в процентах = {((float)input1.Length - output1.Length) / input1.Length * 100} %");
    }

    private static void LZWApply(string fileInput, string fileOutput, LZWOperation operation)
    {
        var startTime = DateTime.UtcNow;

        using var input = new FileStream(fileInput, FileMode.Open, FileAccess.Read);
        using var output = new FileStream(fileOutput, FileMode.OpenOrCreate, FileAccess.Write);

        if (operation == LZWOperation.Compress)
        {
            LZW.Compress(input, output);
            Console.WriteLine($"До сжатия: {input.Length}");
            Console.WriteLine($"После сжатия: {output.Length}");

            var endTime = DateTime.UtcNow;
            Console.WriteLine($"Время сжатия: {(endTime - startTime).TotalMilliseconds} ms");
        }
        else if (operation == LZWOperation.Decompress)
        {
            LZW.Decompress(input, output);
            Console.WriteLine($"После разархивирования: {output.Length}");

            var endTime = DateTime.UtcNow;
            Console.WriteLine($"Время разархивирования: {(endTime - startTime).TotalMilliseconds} ms");
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
        }
    }

    private static async Task Haffman_Example(string fileName, string fileExtension)
    {
        var input = prePathFile + $"{fileName}.{fileExtension}";
        var inputCopy = targetPathFileHaffman + $"{fileName}.{fileExtension}";
        Directory.CreateDirectory(targetPathFileHaffman);
        await CopyFile(input, inputCopy);

        input = inputCopy;
        // var compress = targetPathFileHaffman + $"{fileName}_Compress.{fileExtension}";
        var decompress = targetPathFileHaffman + $"{fileName}_Decompress.{fileExtension}";
        await Haffman_Apply(input, decompress);
    }

    private static async Task Haffman_Apply(string fileInput, string fileOutput)
    {
        await using var input = new FileStream(fileInput, FileMode.Open, FileAccess.Read);
        await using var output = new FileStream(fileOutput, FileMode.OpenOrCreate, FileAccess.Write);

        Console.WriteLine($"До сжатия: {input.Length}");

        var startTime = DateTime.UtcNow;
        var huffmanTree = new Haffman.HuffmanTree();
        huffmanTree.Build(input);
        input.Position = 0;
        var endTime = DateTime.UtcNow;
        Console.WriteLine($"Время построения древа: {(endTime - startTime).TotalMilliseconds} ms");

        startTime = DateTime.UtcNow;
        var encoded = huffmanTree.Encode(input);
        endTime = DateTime.UtcNow;
        Console.WriteLine($"После сжатия: {(float)encoded.Length / 8}");
        Console.WriteLine($"Время сжатия: {(endTime - startTime).TotalMilliseconds} ms");

        startTime = DateTime.UtcNow;
        huffmanTree.Decode(encoded, output);
        endTime = DateTime.UtcNow;

        Console.WriteLine($"После разархивирования: {output.Length}");
        Console.WriteLine($"Время разархивирования: {(endTime - startTime).TotalMilliseconds} ms");
    }

    private static async Task CopyFile(string inputPath, string outputPath)
    {
        await using var input = new FileStream(inputPath, FileMode.Open, FileAccess.Read);
        await using var output = new FileStream(outputPath, FileMode.OpenOrCreate, FileAccess.Write);

        var buffer = new byte[1024];
        while (await input.ReadAsync(buffer) is { } sourceCount and > 0)
            await output.WriteAsync(buffer, 0, sourceCount);
    }

    private enum LZWOperation
    {
        Compress,
        Decompress
    }
}