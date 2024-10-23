using System.Collections;

namespace QRCode.Extensions;

public static class BitArrayExtensions
{
    public static int CopyTo(this BitArray source, BitArray destination,
        int sourceOffset, int destinationOffset, int count)
    {
        for (var i = 0; i < count; i++)
            destination[destinationOffset + i] = source[sourceOffset + i];
        return destinationOffset + count;
    }
}