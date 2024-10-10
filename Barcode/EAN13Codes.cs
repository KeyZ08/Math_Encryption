namespace Barcode;

// ReSharper disable once InconsistentNaming
public static class EAN13Codes
{
    public const int BitsCountPerDigit = 7;
    
    public static readonly bool[][] StructCode = new[] //L = 1, G = 0, это 6 первых цифр штрихкода 
    {
        "111111", "110100", "110010", "110001", "101100",
        "100110", "100011", "101010", "101001", "100101"
    }.Select(x => x.ParseBits()).ToArray();

    public static readonly bool[][] LCodes = new[] //как кодируются числа (индексы массива) в биты
    {
        "0001101", "0011001", "0010011", "0111101", "0100011",
        "0110001", "0101111", "0111011", "0110111", "0001011"
    }.Select(x => x.ParseBits()).ToArray();

    public static readonly bool[][] RCodes = LCodes // тот же самый LCode, но инвертирован
        .Select(x => x.Select(b => !b).ToArray())
        .ToArray();

    public static readonly bool[][] GCodes = RCodes // тот же самый RCode, но перевернут
        .Select(x => x.Reverse().ToArray())
        .ToArray();
    
}