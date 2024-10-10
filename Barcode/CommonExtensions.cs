namespace Barcode;

public static class CommonExtensions
{
    public static bool[] ParseBits(this string bits)
        => bits.Select(c => c != '0').ToArray();

    public static bool[] ParseBits(this char[] bits)
        => bits.Select(c => c != '0').ToArray();
}