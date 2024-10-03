namespace Archivers;

public class NBitStream
{
    private readonly Stream stream;

    private int bitsInCurrent;
    private int bitsLeftInCurrent;
    private long current;

    public NBitStream(Stream stream, int bitsInChunk = 8)
    {
        this.stream = stream;
        BitsInChunk = bitsInChunk;
    }

    public int BitsInChunk { get; private set; }

    public void SetBitsInChunk(int count)
    {
        BitsInChunk = count;
    }

    /// <summary>
    /// Побитовое чтение
    /// </summary>
    /// <returns>Число, представляющее собой прочитанные биты</returns>
    public long Read()
    {
        var bitsCount = 0;
        long result = 0;

        while (bitsCount != BitsInChunk)
        {
            if (bitsLeftInCurrent != 0)
            {
                //записываем биты в результат
                result <<= 1;
                result |= (current >> (bitsLeftInCurrent - 1)) & 0x1;
                bitsLeftInCurrent--;
                bitsCount++;
                continue;
            }

            current = stream.ReadByte();
            if (current == -1)
                return -1;
            bitsLeftInCurrent = 8;
        }

        return result;
    }

    /// <summary>
    /// Записывает число (представление битов) побитово.
    /// </summary>
    public void Write(long value)
    {
        if (value < 0) throw new Exception($"Не возможное значение {nameof(value)}");

        const int uLongBitsCount = 64;
        var v = (ulong)value;

        bitsLeftInCurrent = GetBitsCount(value);
        if (bitsLeftInCurrent < BitsInChunk)
            bitsLeftInCurrent = BitsInChunk;

        while (bitsLeftInCurrent > 0)
        {
            //обрезаем лишние левые биты
            var leftShift = uLongBitsCount - bitsLeftInCurrent;
            var acurrent = v << leftShift;
            acurrent >>= leftShift;

            //обрезаем лишние правые биты
            var neededBitsCount = Math.Clamp(8 - bitsInCurrent, 0, bitsLeftInCurrent);

            var rightShift = bitsLeftInCurrent - neededBitsCount;
            acurrent >>= rightShift;

            current <<= neededBitsCount;
            current |= (long)acurrent;

            bitsLeftInCurrent -= neededBitsCount;
            bitsInCurrent += neededBitsCount;

            if (bitsInCurrent != 8) continue;

            stream.WriteByte((byte)current);
            current = 0;
            bitsInCurrent = 0;
        }
    }

    private static int GetBitsCount(long number)
    {
        var count = 0;
        while (number != 0)
        {
            number >>= 1;
            count++;
        }

        return count;
    }

    /// <summary>
    /// Обязательно нужно вызвать по окончании записи
    /// </summary>
    public void EndWrite()
    {
        if (bitsInCurrent == 0 && current == 0)
            return;

        current <<= 8 - bitsInCurrent;
        stream.WriteByte((byte)current);
    }
}