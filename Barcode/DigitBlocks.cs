namespace Barcode;

public static class DigitBlocks
{
    public const int DigitWidth = 5;
    public const int DigitHeight = 7;

    private static readonly bool[][] N1 = ToBits("00100" +
                                                 "01100" +
                                                 "00100" +
                                                 "00100" +
                                                 "00100" +
                                                 "00100" +
                                                 "01110");

    private static readonly bool[][] N2 = ToBits("01110" +
                                                 "10001" +
                                                 "00001" +
                                                 "00010" +
                                                 "00100" +
                                                 "01000" +
                                                 "11111");

    private static readonly bool[][] N3 = ToBits("11111" +
                                                 "00010" +
                                                 "00100" +
                                                 "00010" +
                                                 "00001" +
                                                 "10001" +
                                                 "01110");

    private static readonly bool[][] N4 = ToBits("00010" +
                                                 "00110" +
                                                 "01010" +
                                                 "10010" +
                                                 "11111" +
                                                 "00010" +
                                                 "00010");

    private static readonly bool[][] N5 = ToBits("11111" +
                                                 "10000" +
                                                 "11110" +
                                                 "00001" +
                                                 "00001" +
                                                 "10001" +
                                                 "01110");

    private static readonly bool[][] N6 = ToBits("00110" +
                                                 "01000" +
                                                 "10000" +
                                                 "11110" +
                                                 "10001" +
                                                 "10001" +
                                                 "01110");

    private static readonly bool[][] N7 = ToBits("11111" +
                                                 "00001" +
                                                 "00010" +
                                                 "00100" +
                                                 "01000" +
                                                 "01000" +
                                                 "01000");

    private static readonly bool[][] N8 = ToBits("01110" +
                                                 "10001" +
                                                 "10001" +
                                                 "01110" +
                                                 "10001" +
                                                 "10001" +
                                                 "01110");

    private static readonly bool[][] N9 = ToBits("01110" +
                                                 "10001" +
                                                 "10001" +
                                                 "01111" +
                                                 "00001" +
                                                 "00010" +
                                                 "01100");

    private static readonly bool[][] N0 = ToBits("01110" +
                                                 "10001" +
                                                 "10001" +
                                                 "10001" +
                                                 "10001" +
                                                 "10001" +
                                                 "01110");

    public static readonly Dictionary<int, bool[][]> Digits = new()
    {
        { 0, N0 },
        { 1, N1 },
        { 2, N2 },
        { 3, N3 },
        { 4, N4 },
        { 5, N5 },
        { 6, N6 },
        { 7, N7 },
        { 8, N8 },
        { 9, N9 }
    };

    private static bool[][] ToBits(string str)
    {
        return str.Chunk(DigitWidth)
            .Select(c => c.ParseBits())
            .ToArray();
    }
}